using Delegates.Infrastructure.Entities.Inventory;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Entities.Orders;
using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Enums.Orders;
using Delegates.Infrastructure.Enums.UserManagement;
using Delegates.Infrastructure.Exceptions;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices;
using Delegates.Interface.IServices.Order;
using Delegates.Interface.IServices.OrderItem;
using Delegates.Interface.IServices.OrderStatusHistory;
using Delegates.Interface.IServices.OrderWarehouseTask;
using Delegates.Interface.IServices.PostponeOrder;
using Delegates.Interface.IServices.PostponeOrderCompany;
using Delegates.Interface.IServices.RegisterDevice;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CreateOrderDto = Delegates.Interface.IServices.Order.CreateOrderDto;

namespace Delegates.Application.Services.Orders
{
    public class OrderService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IPushNotificationService pushNotificationService) : IOrderService
    {
        public async Task<OrderReadDto> CreateAsync(CreateOrderDto dto)
        {
            var clientExists = await unitOfWork.GetRepository<Client, int>().TableNoTracking.AnyAsync(x => x.Id == dto.ClientId);
            if (!clientExists)
                throw new BadRequestException(["العميل المحدد غير موجود"]);

            if (dto.SubWarehouseTasks.Any(t => t.WarehouseId == dto.MainWarehouseTask.WarehouseId))
                throw new BadRequestException(["لا يمكن أن يكون المخزن الفرعي هو نفسه المخزن الرئيسي"]);

            await ValidateWarehouseTaskAsync(dto.MainWarehouseTask);
            foreach (var subTask in dto.SubWarehouseTasks)
                await ValidateSubWarehouseTaskStockAsync(subTask);

            var order = new Order
            {
                Code = await GenerateOrderCodeAsync(),
                ClientId = dto.ClientId,
                MainWarehouseId = dto.MainWarehouseTask.WarehouseId,
                PaymentMethod = dto.PaymentMethod,
                Status = OrderStatus.Inquiry
            };
            auditHelper.SetCreated(order);

            order.WarehouseTasks.Add(BuildWarehouseTask(dto.MainWarehouseTask, isMain: true));
            foreach (var subTask in dto.SubWarehouseTasks)
                order.WarehouseTasks.Add(BuildWarehouseTask(subTask, isMain: false));

            var statusEntry = new OrderStatusHistory { Status = OrderStatus.Inquiry };
            auditHelper.SetCreated(statusEntry);
            order.StatusHistory.Add(statusEntry);

            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            await ordersRepo.AddAsync(order);
            await unitOfWork.SaveChangesAsync();

            foreach (var task in order.WarehouseTasks)
                await pushNotificationService.NotifyUserAsync(task.DelegateId, "طلب جديد", $"تم إسناد طلب رقم {order.Code} إليك");

            return await GetByIdAsync(order.Id);
        }

        public async Task<OrderReadDto> ApproveAsync(int id)
        {
            await TryApproveInternalAsync(id, throwOnInsufficientStock: true);
            return await GetByIdAsync(id);
        }

        private async Task<bool> TryApproveInternalAsync(int orderId, bool throwOnInsufficientStock)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var stockRepo = unitOfWork.GetRepository<ProductStock, int>();

            var order = await ordersRepo.TableNoTracking
                .Include(x => x.WarehouseTasks).ThenInclude(t => t.Items)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new EntityNotFoundException(nameof(Order), orderId);

            if (order.Status != OrderStatus.Inquiry && order.Status != OrderStatus.Postponed)
            {
                if (throwOnInsufficientStock)
                    throw new BadRequestException(["لا يمكن اعتماد الطلب في حالته الحالية"]);
                return false;
            }

            var mainTask = order.WarehouseTasks.First(t => t.IsMainWarehouse);
            var subTasks = order.WarehouseTasks.Where(t => !t.IsMainWarehouse).ToList();

            var warehouseIds = order.WarehouseTasks.Select(t => t.WarehouseId).Distinct().ToList();
            var productIds = order.WarehouseTasks.SelectMany(t => t.Items).Select(i => i.ProductId).Distinct().ToList();

            var stocks = await stockRepo.TableNoTracking
                .Where(x => warehouseIds.Contains(x.WarehouseId) && productIds.Contains(x.ProductId))
                .ToListAsync();

            var stockLookup = stocks.ToDictionary(x => (x.ProductId, x.WarehouseId));

            ProductStock GetOrCreateStock(int productId, int warehouseId)
            {
                if (stockLookup.TryGetValue((productId, warehouseId), out var existing))
                    return existing;

                var created = new ProductStock { ProductId = productId, WarehouseId = warehouseId, Quantity = 0 };
                stockLookup[(productId, warehouseId)] = created;
                return created;
            }

            foreach (var subTask in subTasks)
            {
                foreach (var item in subTask.Items)
                {
                    var subStock = GetOrCreateStock(item.ProductId, subTask.WarehouseId);
                    if (subStock.Quantity < item.Quantity)
                    {
                        if (throwOnInsufficientStock)
                            throw new BadRequestException([$"الكمية غير متوفرة حاليًا في المخزن الفرعي رقم {subTask.WarehouseId} للمنتج رقم {item.ProductId}"]);
                        return false;
                    }

                    subStock.Quantity -= item.Quantity;
                    GetOrCreateStock(item.ProductId, mainTask.WarehouseId).Quantity += item.Quantity;
                }
            }

            foreach (var item in mainTask.Items)
            {
                var mainStock = GetOrCreateStock(item.ProductId, mainTask.WarehouseId);
                if (mainStock.Quantity < item.Quantity)
                {
                    if (throwOnInsufficientStock)
                        throw new BadRequestException([$"الكمية غير متوفرة بالكامل في المخزن الرئيسي للمنتج رقم {item.ProductId} حتى بعد النقل (المتاح: {mainStock.Quantity})"]);
                    return false;
                }
            }

            foreach (var item in mainTask.Items)
                GetOrCreateStock(item.ProductId, mainTask.WarehouseId).Quantity -= item.Quantity;

            foreach (var stock in stockLookup.Values)
            {
                if (stock.Id == 0)
                {
                    auditHelper.SetCreated(stock);
                    await stockRepo.AddAsync(stock);
                }
                else
                {
                    auditHelper.SetUpdated(stock);
                    stockRepo.Update(stock);
                }
            }

            order.Status = OrderStatus.Approved;
            auditHelper.SetUpdated(order);

            var statusEntry = new OrderStatusHistory { Status = OrderStatus.Approved };
            auditHelper.SetCreated(statusEntry);
            order.StatusHistory.Add(statusEntry);

            ordersRepo.Update(order);
            await unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<OrderReadDto> PostponeAsync(int id, PostponeOrderDto dto)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var order = await ordersRepo.TableNoTracking
                .Include(x => x.WarehouseTasks).ThenInclude(t => t.Items)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException(nameof(Order), id);

            if (order.Status != OrderStatus.Inquiry)
                throw new BadRequestException(["الطلب ليس في حالة استفسار، لا يمكن تأجيله"]);

            foreach (var companyDto in dto.Companies)
            {
                var companyExists = await unitOfWork.GetRepository<Company, int>().TableNoTracking.AnyAsync(x => x.Id == companyDto.CompanyId);
                if (!companyExists)
                    throw new BadRequestException([$"الشركة رقم {companyDto.CompanyId} غير موجودة"]);

                foreach (var productId in companyDto.ProductIds)
                {
                    var productExists = await unitOfWork.GetRepository<Product, int>().TableNoTracking.AnyAsync(x => x.Id == productId);
                    if (!productExists)
                        throw new BadRequestException([$"المنتج رقم {productId} غير موجود"]);
                }
            }

            order.DepositAmount = dto.DepositAmount;
            order.DepositPaymentMethod = dto.DepositPaymentMethod;
            order.Status = OrderStatus.Postponed;
            auditHelper.SetUpdated(order);

            foreach (var companyDto in dto.Companies)
            {
                var postponedCompany = new OrderPostponedCompany { CompanyId = companyDto.CompanyId };
                auditHelper.SetCreated(postponedCompany);

                foreach (var productId in companyDto.ProductIds)
                {
                    var shortageQuantity = await CalculateShortageQuantityAsync(order, productId);

                    var postponedProduct = new OrderPostponedCompanyProduct { ProductId = productId, Quantity = shortageQuantity };
                    auditHelper.SetCreated(postponedProduct);
                    postponedCompany.Products.Add(postponedProduct);
                }

                order.PostponedCompanies.Add(postponedCompany);
            }

            var statusEntry = new OrderStatusHistory { Status = OrderStatus.Postponed };
            auditHelper.SetCreated(statusEntry);
            order.StatusHistory.Add(statusEntry);

            ordersRepo.Update(order);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        private async Task<int> CalculateShortageQuantityAsync(Order order, int productId)
        {
            var mainTask = order.WarehouseTasks.First(t => t.IsMainWarehouse);
            var subTasks = order.WarehouseTasks.Where(t => !t.IsMainWarehouse).ToList();

            var mainRequiredQuantity = mainTask.Items.Where(i => i.ProductId == productId).Sum(i => i.Quantity);
            if (mainRequiredQuantity == 0)
                return 0;

            var subTransferQuantity = subTasks.SelectMany(t => t.Items).Where(i => i.ProductId == productId).Sum(i => i.Quantity);

            var mainStock = await unitOfWork.GetRepository<ProductStock, int>().TableNoTracking
                .Where(x => x.ProductId == productId && x.WarehouseId == mainTask.WarehouseId)
                .Select(x => (int?)x.Quantity)
                .FirstOrDefaultAsync() ?? 0;

            var shortage = mainRequiredQuantity - subTransferQuantity - mainStock;
            return Math.Max(shortage, 0);
        }

        public async Task<OrderReadDto> CompletePurchaseAsync(int orderId, CompletePurchaseDto dto)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var order = await ordersRepo.TableNoTracking
                .Include(x => x.PostponedCompanies).ThenInclude(c => c.Products)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new EntityNotFoundException(nameof(Order), orderId);

            if (order.Status != OrderStatus.Postponed)
                throw new BadRequestException(["الطلب ليس في حالة مؤجل، لا يمكن تسجيل شراء له"]);

            var linkedProductIds = order.PostponedCompanies
                .SelectMany(c => c.Products)
                .Select(p => p.ProductId)
                .ToHashSet();

            foreach (var item in dto.Items)
            {
                if (!linkedProductIds.Contains(item.ProductId))
                    throw new BadRequestException([$"المنتج رقم {item.ProductId} غير مرتبط بأي شركة مؤجلة لهذا الطلب"]);
            }

            var stockRepo = unitOfWork.GetRepository<ProductStock, int>();

            foreach (var item in dto.Items)
            {
                var stock = await stockRepo.TableNoTracking
                    .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseId == order.MainWarehouseId);

                if (stock is null)
                {
                    stock = new ProductStock { ProductId = item.ProductId, WarehouseId = order.MainWarehouseId, Quantity = item.ReceivedQuantity };
                    auditHelper.SetCreated(stock);
                    await stockRepo.AddAsync(stock);
                }
                else
                {
                    stock.Quantity += item.ReceivedQuantity;
                    auditHelper.SetUpdated(stock);
                    stockRepo.Update(stock);
                }
            }

            await unitOfWork.SaveChangesAsync();
            unitOfWork.ClearTracking();

            await TryApproveInternalAsync(orderId, throwOnInsufficientStock: false);

            return await GetByIdAsync(orderId);
        }

        private async Task ValidateWarehouseTaskAsync(CreateOrderWarehouseTaskDto task)
        {
            var warehouseExists = await unitOfWork.GetRepository<Warehouse, int>().TableNoTracking.AnyAsync(x => x.Id == task.WarehouseId);
            if (!warehouseExists)
                throw new BadRequestException([$"المخزن رقم {task.WarehouseId} غير موجود"]);

            var delegateUser = await unitOfWork.GetRepository<ApplicationUser, int>().TableNoTracking
                .FirstOrDefaultAsync(x => x.Id == task.DelegateId);
            if (delegateUser is null || delegateUser.UserType != UserType.Delegate)
                throw new BadRequestException([$"المندوب رقم {task.DelegateId} غير موجود"]);

            var productsRepo = unitOfWork.GetRepository<Product, int>();
            foreach (var item in task.Items)
            {
                var productExists = await productsRepo.TableNoTracking.AnyAsync(x => x.Id == item.ProductId);
                if (!productExists)
                    throw new BadRequestException([$"المنتج رقم {item.ProductId} غير موجود"]);
            }
        }

        private async Task ValidateSubWarehouseTaskStockAsync(CreateOrderWarehouseTaskDto subTask)
        {
            await ValidateWarehouseTaskAsync(subTask);

            var stockRepo = unitOfWork.GetRepository<ProductStock, int>();
            foreach (var item in subTask.Items)
            {
                var stock = await stockRepo.TableNoTracking
                    .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseId == subTask.WarehouseId);

                var available = stock?.Quantity ?? 0;
                if (available < item.Quantity)
                    throw new BadRequestException([$"الكمية غير متوفرة في المخزن الفرعي رقم {subTask.WarehouseId} للمنتج رقم {item.ProductId} (المتاح: {available})"]);
            }
        }

        private OrderWarehouseTask BuildWarehouseTask(CreateOrderWarehouseTaskDto dto, bool isMain)
        {
            var task = new OrderWarehouseTask
            {
                WarehouseId = dto.WarehouseId,
                DelegateId = dto.DelegateId,
                IsMainWarehouse = isMain,
                Notes = dto.Notes
            };
            auditHelper.SetCreated(task);

            foreach (var item in dto.Items)
            {
                var taskItem = new OrderWarehouseTaskItem { ProductId = item.ProductId, Quantity = item.Quantity };
                auditHelper.SetCreated(taskItem);
                task.Items.Add(taskItem);
            }

            return task;
        }

        public async Task<OrderReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Order, int>();

            var order = await repo.TableNoTracking
                .Include(x => x.Client)
                .Include(x => x.MainWarehouse)
                .Include(x => x.WarehouseTasks).ThenInclude(t => t.Warehouse)
                .Include(x => x.WarehouseTasks).ThenInclude(t => t.Delegate)
                .Include(x => x.WarehouseTasks).ThenInclude(t => t.Items).ThenInclude(i => i.Product)
                .Include(x => x.StatusHistory)
                .Include(x => x.PostponedCompanies).ThenInclude(c => c.Company)
                .Include(x => x.PostponedCompanies).ThenInclude(c => c.Products).ThenInclude(p => p.Product)
                .Include(x => x.ContactLogs)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException(nameof(Order), id);

            return new OrderReadDto
            {
                Id = order.Id,
                Code = order.Code,
                ClientId = order.ClientId,
                ClientName = order.Client.Name,
                ClientPhoneNumber = order.Client.PhoneNumber,
                MainWarehouseId = order.MainWarehouseId,
                MainWarehouseName = order.MainWarehouse.Name,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                WarehouseTasks = order.WarehouseTasks.Select(t => new OrderWarehouseTaskReadDto
                {
                    WarehouseId = t.WarehouseId,
                    WarehouseName = t.Warehouse.Name,
                    IsMainWarehouse = t.IsMainWarehouse,
                    DelegateId = t.DelegateId,
                    DelegateName = t.Delegate.FullName,
                    Notes = t.Notes,
                    Items = t.Items.Select(i => new OrderItemReadDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity
                    }).ToList()
                }).ToList(),
                StatusHistory = order.StatusHistory
                    .OrderBy(h => h.CreatedOn)
                    .Select(h => new OrderStatusHistoryReadDto { Status = h.Status, ChangedBy = h.CreatedBy, ChangedOn = h.CreatedOn })
                    .ToList(),
                CreatedBy = order.CreatedBy,
                CreatedOn = order.CreatedOn,
                DepositAmount = order.DepositAmount,
                DepositPaymentMethod = order.DepositPaymentMethod,
                PostponedCompanies = order.PostponedCompanies.Select(c => new PostponedCompanyReadDto
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.Company.Name,
                    Products = c.Products.Select(p => new PostponedProductReadDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.Product.Name,
                        CatalogNumber = p.Product.CatalogNumber,
                        Quantity = p.Quantity
                    }).ToList()
                }).ToList(),
                ContactLogs = order.ContactLogs
                    .OrderBy(c => c.CreatedOn)
                    .Select(c => new OrderContactLogReadDto { Notes = c.Notes, ContactedBy = c.CreatedBy, ContactedOn = c.CreatedOn })
                    .ToList(),
                CancellationReason = order.CancellationReason,
                DeliveryPostponeReason = order.DeliveryPostponeReason,
                PaymentReceivedMethod = order.PaymentReceivedMethod,
            };
        }

        public async Task<DataTablePaginationResponseDto<OrderListDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<Order, int>();
            return await repo.GetPaginationListFromBodyAsync(request, ListSelector);
        }

        public async Task<OrderReadDto> AddContactLogAsync(int orderId, AddOrderContactLogDto dto)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var orderExists = await ordersRepo.TableNoTracking.AnyAsync(x => x.Id == orderId);
            if (!orderExists)
                throw new EntityNotFoundException(nameof(Order), orderId);

            var contactLog = new OrderContactLog { OrderId = orderId, Notes = dto.Notes };
            auditHelper.SetCreated(contactLog);

            var contactLogsRepo = unitOfWork.GetRepository<OrderContactLog, int>();
            await contactLogsRepo.AddAsync(contactLog);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(orderId);
        }

        public async Task<DataTablePaginationResponseDto<OrderListDto>> GetInquiriesAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<Order, int>();
            List<Expression<Func<Order, bool>>> criterias = [x => x.Status == OrderStatus.Inquiry];

            return await repo.GetPaginationListFromBodyAsync(request, ListSelector, criterias: criterias);
        }

        public async Task<DataTablePaginationResponseDto<OrderListDto>> GetMyApprovedOrdersAsync(DataTablePaginationRequestDto request)
        {
            var currentUserId = auditHelper.GetCurrentUserId();
            var repo = unitOfWork.GetRepository<Order, int>();
            List<Expression<Func<Order, bool>>> criterias =
                [x => x.Status == OrderStatus.Approved && x.CreatedBy == currentUserId];

            return await repo.GetPaginationListFromBodyAsync(request, ListSelector, criterias: criterias);
        }

        public async Task<DataTablePaginationResponseDto<OrderListDto>> GetMyPostponedOrdersAsync(DataTablePaginationRequestDto request)
        {
            var currentUserId = auditHelper.GetCurrentUserId();
            var repo = unitOfWork.GetRepository<Order, int>();
            List<Expression<Func<Order, bool>>> criterias =
                [x => x.Status == OrderStatus.Postponed && x.CreatedBy == currentUserId];

            return await repo.GetPaginationListFromBodyAsync(request, ListSelector, criterias: criterias);
        }

        public async Task<OrderReadDto> DeliverAsync(int orderId, DeliverOrderDto dto)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var order = await ordersRepo.TableNoTracking
                .Include(x => x.WarehouseTasks)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new EntityNotFoundException(nameof(Order), orderId);

            if (order.Status != OrderStatus.Approved)
                throw new BadRequestException(["لا يمكن تسليم الطلب في حالته الحالية"]);

            var mainTask = order.WarehouseTasks.First(t => t.IsMainWarehouse);
            EnsureCurrentUserIsAssignedDelegate(mainTask);

            order.PaymentReceivedMethod = dto.PaymentReceivedMethod;
            order.Status = OrderStatus.Delivered;
            auditHelper.SetUpdated(order);

            var statusEntry = new OrderStatusHistory { Status = OrderStatus.Delivered };
            auditHelper.SetCreated(statusEntry);
            order.StatusHistory.Add(statusEntry);

            ordersRepo.Update(order);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(orderId);
        }

        public async Task<OrderReadDto> CancelAsync(int orderId, CancelOrderDto dto)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var order = await ordersRepo.TableNoTracking
                .Include(x => x.WarehouseTasks)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new EntityNotFoundException(nameof(Order), orderId);

            if (order.Status != OrderStatus.Approved && order.Status != OrderStatus.DeliveryPostponed)
                throw new BadRequestException(["لا يمكن إلغاء الطلب في حالته الحالية"]);

            var mainTask = order.WarehouseTasks.First(t => t.IsMainWarehouse);
            EnsureCurrentUserIsAssignedDelegate(mainTask);

            order.CancellationReason = dto.Reason;
            order.Status = OrderStatus.Cancelled;
            auditHelper.SetUpdated(order);

            var statusEntry = new OrderStatusHistory { Status = OrderStatus.Cancelled };
            auditHelper.SetCreated(statusEntry);
            order.StatusHistory.Add(statusEntry);

            ordersRepo.Update(order);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(orderId);
        }

        public async Task<OrderReadDto> PostponeDeliveryAsync(int orderId, PostponeDeliveryDto dto)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var order = await ordersRepo.TableNoTracking
                .Include(x => x.WarehouseTasks)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new EntityNotFoundException(nameof(Order), orderId);

            if (order.Status != OrderStatus.Approved)
                throw new BadRequestException(["لا يمكن تأجيل تسليم الطلب في حالته الحالية"]);

            var mainTask = order.WarehouseTasks.First(t => t.IsMainWarehouse);
            EnsureCurrentUserIsAssignedDelegate(mainTask);

            order.DeliveryPostponeReason = dto.Reason;
            order.Status = OrderStatus.DeliveryPostponed;
            auditHelper.SetUpdated(order);

            var statusEntry = new OrderStatusHistory { Status = OrderStatus.DeliveryPostponed };
            auditHelper.SetCreated(statusEntry);
            order.StatusHistory.Add(statusEntry);

            ordersRepo.Update(order);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(orderId);
        }

        public async Task<OrderReadDto> ReassignDelegateAsync(int orderId, ReassignDelegateDto dto)
        {
            var ordersRepo = unitOfWork.GetRepository<Order, int>();
            var order = await ordersRepo.TableNoTracking
                .Include(x => x.WarehouseTasks)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new EntityNotFoundException(nameof(Order), orderId);

            if (order.Status != OrderStatus.Cancelled && order.Status != OrderStatus.DeliveryPostponed)
                throw new BadRequestException(["لا يمكن تحويل الطلب لمندوب آخر في حالته الحالية"]);

            var newDelegate = await unitOfWork.GetRepository<ApplicationUser, int>().TableNoTracking
                .FirstOrDefaultAsync(x => x.Id == dto.NewDelegateId);
            if (newDelegate is null || newDelegate.UserType != UserType.Delegate)
                throw new BadRequestException(["المندوب الجديد المحدد غير موجود"]);

            var mainTask = order.WarehouseTasks.First(t => t.IsMainWarehouse);
            mainTask.DelegateId = dto.NewDelegateId;
            auditHelper.SetUpdated(mainTask);

            order.Status = OrderStatus.Approved;
            auditHelper.SetUpdated(order);

            var statusEntry = new OrderStatusHistory { Status = OrderStatus.Approved };
            auditHelper.SetCreated(statusEntry);
            order.StatusHistory.Add(statusEntry);

            ordersRepo.Update(order);
            await unitOfWork.SaveChangesAsync();

            await pushNotificationService.NotifyUserAsync(dto.NewDelegateId, "طلب جديد", $"تم إسناد طلب رقم {order.Code} إليك");

            return await GetByIdAsync(orderId);
        }

        public async Task<DataTablePaginationResponseDto<OrderListDto>> GetMyDelegateOrdersAsync(DataTablePaginationRequestDto request)
        {
            var currentUserId = int.Parse(auditHelper.GetCurrentUserId()!);
            var repo = unitOfWork.GetRepository<Order, int>();

            List<Expression<Func<Order, bool>>> criterias =
            [
                x => (x.Status == OrderStatus.Approved || x.Status == OrderStatus.DeliveryPostponed)
             && x.WarehouseTasks.Any(t => t.IsMainWarehouse && t.DelegateId == currentUserId)
            ];

            return await repo.GetPaginationListFromBodyAsync(request, ListSelector, criterias: criterias);
        }

        public async Task<DataTablePaginationResponseDto<CancelledOrderListDto>> GetCancelledOrdersAsync(DataTablePaginationRequestDto request)
        {
            var currentUserId = auditHelper.GetCurrentUserId();
            var repo = unitOfWork.GetRepository<Order, int>();
            List<Expression<Func<Order, bool>>> criterias =
                [x => x.Status == OrderStatus.Cancelled && x.CreatedBy == currentUserId];

            return await repo.GetPaginationListFromBodyAsync(request, CancelledSelector, criterias: criterias);
        }

        public async Task<DataTablePaginationResponseDto<DeliveryPostponedOrderListDto>> GetDeliveryPostponedOrdersAsync(DataTablePaginationRequestDto request)
        {
            var currentUserId = auditHelper.GetCurrentUserId();
            var repo = unitOfWork.GetRepository<Order, int>();
            List<Expression<Func<Order, bool>>> criterias =
                [x => x.Status == OrderStatus.DeliveryPostponed && x.CreatedBy == currentUserId];

            return await repo.GetPaginationListFromBodyAsync(request, DeliveryPostponedSelector, criterias: criterias);
        }

        private static readonly Dictionary<string, string> AdminOrderColumnMap = new()
{
    { "ClientName", "Client.Name" },
    { "ClientPhoneNumber", "Client.PhoneNumber" },
    { "ClientCode", "Client.Code" }
};

        private static readonly Expression<Func<Order, AdminOrderListDto>> AdminListSelector = x => new AdminOrderListDto
        {
            Id = x.Id,
            Code = x.Code,
            ClientName = x.Client.Name,
            ClientPhoneNumber = x.Client.PhoneNumber,
            MainWarehouseName = x.MainWarehouse.Name,
            MainDelegateName = x.WarehouseTasks.Where(t => t.IsMainWarehouse).Select(t => t.Delegate.FullName).FirstOrDefault() ?? "",
            Status = x.Status,
            CreatedById = x.CreatedBy,
            CreatedOn = x.CreatedOn
        };

        public async Task<DataTablePaginationResponseDto<AdminOrderListDto>> GetAllOrdersAdminAsync(
            DataTablePaginationRequestDto request, int? delegateId, int? companyId)
        {
            var repo = unitOfWork.GetRepository<Order, int>();

            List<Expression<Func<Order, bool>>> criterias = new();
            if (delegateId.HasValue)
                criterias.Add(x => x.WarehouseTasks.Any(t => t.IsMainWarehouse && t.DelegateId == delegateId.Value));
            if (companyId.HasValue)
                criterias.Add(x => x.PostponedCompanies.Any(c => c.CompanyId == companyId.Value));

            var result = await repo.GetPaginationListFromBodyAsync(request, AdminListSelector, criterias: criterias, columnMap: AdminOrderColumnMap);

            await AttachOrderCreatedByNamesAsync(result.Data);
            return result;
        }

        public async Task<List<OrderStatusSummaryDto>> GetOrdersStatusSummaryAsync(
            DataTablePaginationRequestDto request, int? delegateId, int? companyId)
        {
            var repo = unitOfWork.GetRepository<Order, int>();
            var query = repo.TableNoTracking.ApplyDataTableFiltering(request, AdminOrderColumnMap);

            if (delegateId.HasValue)
                query = query.Where(x => x.WarehouseTasks.Any(t => t.IsMainWarehouse && t.DelegateId == delegateId.Value));
            if (companyId.HasValue)
                query = query.Where(x => x.PostponedCompanies.Any(c => c.CompanyId == companyId.Value));

            return await query
                .GroupBy(x => x.Status)
                .Select(g => new OrderStatusSummaryDto { Status = g.Key, Count = g.Count() })
                .ToListAsync();
        }

        private async Task AttachOrderCreatedByNamesAsync(List<AdminOrderListDto> items)
        {
            var ids = items
                .Select(x => x.CreatedById)
                .Where(x => !string.IsNullOrWhiteSpace(x) && int.TryParse(x, out _))
                .Select(int.Parse)
                .Distinct()
                .ToList();

            if (ids.Count == 0) return;

            var names = await unitOfWork.GetRepository<ApplicationUser, int>().TableNoTrackingWithNoFilter
                .Where(x => ids.Contains(x.Id))
                .Select(x => new { x.Id, x.FullName })
                .ToDictionaryAsync(x => x.Id, x => x.FullName);

            foreach (var item in items)
            {
                if (int.TryParse(item.CreatedById, out var id) && names.TryGetValue(id, out var name))
                    item.CreatedByName = name;
            }
        }

        private void EnsureCurrentUserIsAssignedDelegate(OrderWarehouseTask mainTask)
        {
            var currentUserId = int.Parse(auditHelper.GetCurrentUserId()!);
            if (mainTask.DelegateId != currentUserId)
                throw new BadRequestException(["هذا الطلب غير مسند إليك"]);
        }

        private static readonly Expression<Func<Order, CancelledOrderListDto>> CancelledSelector = x => new CancelledOrderListDto
        {
            Id = x.Id,
            Code = x.Code,
            ClientName = x.Client.Name,
            DelegateName = x.WarehouseTasks.Where(t => t.IsMainWarehouse).Select(t => t.Delegate.FullName).FirstOrDefault() ?? "",
            CancellationReason = x.CancellationReason ?? "",
            CancelledOn = x.LastModifiedOn
        };

        private static readonly Expression<Func<Order, DeliveryPostponedOrderListDto>> DeliveryPostponedSelector = x => new DeliveryPostponedOrderListDto
        {
            Id = x.Id,
            Code = x.Code,
            ClientName = x.Client.Name,
            DelegateName = x.WarehouseTasks.Where(t => t.IsMainWarehouse).Select(t => t.Delegate.FullName).FirstOrDefault() ?? "",
            PostponeReason = x.DeliveryPostponeReason ?? "",
            PostponedOn = x.LastModifiedOn
        };

        private static readonly Expression<Func<Order, OrderListDto>> ListSelector = x => new OrderListDto
        {
            Id = x.Id,
            Code = x.Code,
            ClientName = x.Client.Name,
            ClientPhoneNumber = x.Client.PhoneNumber,
            Status = x.Status,
            CreatedOn = x.CreatedOn
        };


        private async Task<string> GenerateOrderCodeAsync()
        {
            var repo = unitOfWork.GetRepository<Order, int>();
            var count = await repo.TableNoTrackingWithNoFilter.CountAsync();
            return $"O-{count + 1:D5}";
        }
    }
}
