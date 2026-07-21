using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.IServices.PostponeOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public interface IOrderService
    {
        Task<OrderReadDto> CreateAsync(CreateOrderDto dto);
        Task<OrderReadDto> GetByIdAsync(int id);
        Task<DataTablePaginationResponseDto<OrderListDto>> GetPaginationListAsync(DataTablePaginationRequestDto request);
        Task<OrderReadDto> ApproveAsync(int id);
        Task<OrderReadDto> PostponeAsync(int id, PostponeOrderDto dto);
        Task<OrderReadDto> AddContactLogAsync(int orderId, AddOrderContactLogDto dto);
        Task<DataTablePaginationResponseDto<OrderListDto>> GetInquiriesAsync(DataTablePaginationRequestDto request);
        Task<DataTablePaginationResponseDto<OrderListDto>> GetMyApprovedOrdersAsync(DataTablePaginationRequestDto request);
        Task<DataTablePaginationResponseDto<OrderListDto>> GetMyPostponedOrdersAsync(DataTablePaginationRequestDto request);
        Task<OrderReadDto> CompletePurchaseAsync(int orderId, CompletePurchaseDto dto);
        Task<OrderReadDto> DeliverAsync(int orderId, DeliverOrderDto dto);
        Task<OrderReadDto> CancelAsync(int orderId, CancelOrderDto dto);
        Task<OrderReadDto> PostponeDeliveryAsync(int orderId, PostponeDeliveryDto dto);
        Task<OrderReadDto> ReassignDelegateAsync(int orderId, ReassignDelegateDto dto);
        Task<DataTablePaginationResponseDto<OrderListDto>> GetMyDelegateOrdersAsync(DataTablePaginationRequestDto request);
        Task<DataTablePaginationResponseDto<CancelledOrderListDto>> GetCancelledOrdersAsync(DataTablePaginationRequestDto request);
        Task<DataTablePaginationResponseDto<DeliveryPostponedOrderListDto>> GetDeliveryPostponedOrdersAsync(DataTablePaginationRequestDto request);
        Task<DataTablePaginationResponseDto<AdminOrderListDto>> GetAllOrdersAdminAsync(
            DataTablePaginationRequestDto request, int? delegateId, int? companyId);
        Task<List<OrderStatusSummaryDto>> GetOrdersStatusSummaryAsync(
            DataTablePaginationRequestDto request, int? delegateId, int? companyId);
    }
}
