using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices;
using Delegates.Interface.IServices.Order;
using Delegates.Interface.IServices.PostponeOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CreateOrderDto = Delegates.Interface.IServices.Order.CreateOrderDto;

namespace Delegates.Web.Controllers.Orders
{
    [Authorize]
    public class OrdersController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<OrderReadDto>> Create(CreateOrderDto dto)
            => Ok(await serviceManager.OrderService.CreateAsync(dto));

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,CustomerService,Delegate")]
        public async Task<ActionResult<OrderReadDto>> GetById(int id)
            => Ok(await serviceManager.OrderService.GetByIdAsync(id));

        [HttpPost("GetAll")]
        [Authorize(Roles = "Admin,CustomerService")]
        public async Task<ActionResult<DataTablePaginationResponseDto<OrderListDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.OrderService.GetPaginationListAsync(request));

        [HttpPost("{id}/Approve")]
        [Authorize(Roles = "Admin,CustomerService")]
        public async Task<ActionResult<OrderReadDto>> Approve(int id)
            => Ok(await serviceManager.OrderService.ApproveAsync(id));

        [HttpPost("{id}/Postpone")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<OrderReadDto>> Postpone(int id, PostponeOrderDto dto)
            => Ok(await serviceManager.OrderService.PostponeAsync(id, dto));

        [HttpPost("{id}/CompletePurchase")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderReadDto>> CompletePurchase(int id, CompletePurchaseDto dto)
            => Ok(await serviceManager.OrderService.CompletePurchaseAsync(id, dto));

        [HttpPost("{id}/Contact")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<OrderReadDto>> AddContact(int id, AddOrderContactLogDto dto)
            => Ok(await serviceManager.OrderService.AddContactLogAsync(id, dto));

        [HttpPost("Inquiries")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<DataTablePaginationResponseDto<OrderListDto>>> GetInquiries(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.OrderService.GetInquiriesAsync(request));

        [HttpPost("MyApproved")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<DataTablePaginationResponseDto<OrderListDto>>> GetMyApproved(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.OrderService.GetMyApprovedOrdersAsync(request));

        [HttpPost("MyPostponed")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<DataTablePaginationResponseDto<OrderListDto>>> GetMyPostponed(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.OrderService.GetMyPostponedOrdersAsync(request));

        [HttpPost("{id}/Deliver")]
        [Authorize(Roles = "Delegate")]
        public async Task<ActionResult<OrderReadDto>> Deliver(int id, DeliverOrderDto dto)
            => Ok(await serviceManager.OrderService.DeliverAsync(id, dto));

        [HttpPost("{id}/Cancel")]
        [Authorize(Roles = "Delegate")]
        public async Task<ActionResult<OrderReadDto>> Cancel(int id, CancelOrderDto dto)
            => Ok(await serviceManager.OrderService.CancelAsync(id, dto));

        [HttpPost("{id}/PostponeDelivery")]
        [Authorize(Roles = "Delegate")]
        public async Task<ActionResult<OrderReadDto>> PostponeDelivery(int id, PostponeDeliveryDto dto)
            => Ok(await serviceManager.OrderService.PostponeDeliveryAsync(id, dto));

        [HttpPost("MyDelegateOrders")]
        [Authorize(Roles = "Delegate")]
        public async Task<ActionResult<DataTablePaginationResponseDto<OrderListDto>>> GetMyDelegateOrders(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.OrderService.GetMyDelegateOrdersAsync(request));

        [HttpPost("{id}/ReassignDelegate")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<OrderReadDto>> ReassignDelegate(int id, ReassignDelegateDto dto)
            => Ok(await serviceManager.OrderService.ReassignDelegateAsync(id, dto));

        [HttpPost("Cancelled")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<DataTablePaginationResponseDto<CancelledOrderListDto>>> GetCancelled(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.OrderService.GetCancelledOrdersAsync(request));

        [HttpPost("DeliveryPostponed")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<DataTablePaginationResponseDto<DeliveryPostponedOrderListDto>>> GetDeliveryPostponed(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.OrderService.GetDeliveryPostponedOrdersAsync(request));

        [HttpPost("AdminAll")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DataTablePaginationResponseDto<AdminOrderListDto>>> GetAllForAdmin(
            [FromBody] DataTablePaginationRequestDto request, [FromQuery] int? delegateId, [FromQuery] int? companyId)
             => Ok(await serviceManager.OrderService.GetAllOrdersAdminAsync(request, delegateId, companyId));

        [HttpPost("AdminSummary")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<OrderStatusSummaryDto>>> GetAdminSummary(
            [FromBody] DataTablePaginationRequestDto request, [FromQuery] int? delegateId, [FromQuery] int? companyId)
            => Ok(await serviceManager.OrderService.GetOrdersStatusSummaryAsync(request, delegateId, companyId));
    }
}
