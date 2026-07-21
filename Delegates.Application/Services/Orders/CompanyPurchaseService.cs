using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Entities.Orders;
using Delegates.Infrastructure.Enums.Orders;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.CompanyPurchaseProduct;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.Orders
{
    public class CompanyPurchaseService(IUnitOfWork unitOfWork) : ICompanyPurchaseService
    {
        public async Task<DataTablePaginationResponseDto<CompanyPurchaseRequestDto>> GetPurchaseRequestsAsync(DataTablePaginationRequestDto request)
        {
            var companiesRepo = unitOfWork.GetRepository<Company, int>();
            var postponedCompanyRepo = unitOfWork.GetRepository<OrderPostponedCompany, int>();

            var companyIdsQuery = postponedCompanyRepo.TableNoTracking
                .Where(x => x.Order.Status == OrderStatus.Postponed)
                .Select(x => x.CompanyId)
                .Distinct();

            var query = companiesRepo.TableNoTracking.Where(c => companyIdsQuery.Contains(c.Id));

            var search = request.SearchValue?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.Contains(search));

            var totalCount = await query.CountAsync();

            var start = Math.Max(request.Start, 0);
            var length = request.Length <= 0 ? 10 : Math.Min(request.Length, 500);

            var companies = await query
                .OrderBy(c => c.Name)
                .Skip(start)
                .Take(length)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            var companyIds = companies.Select(c => c.Id).ToList();

            var links = await postponedCompanyRepo.TableNoTracking
                .Where(x => companyIds.Contains(x.CompanyId) && x.Order.Status == OrderStatus.Postponed)
                .Include(x => x.Order)
                .Include(x => x.Products).ThenInclude(p => p.Product)
                .ToListAsync();

            var data = companies.Select(c =>
            {
                var companyLinks = links.Where(l => l.CompanyId == c.Id).ToList();

                var products = companyLinks
                    .SelectMany(l => l.Products)
                    .GroupBy(p => new { p.ProductId, p.Product.Name, p.Product.CatalogNumber })
                    .Select(g => new CompanyPurchaseProductDto
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.Name,
                        CatalogNumber = g.Key.CatalogNumber,
                        TotalQuantityNeeded = g.Sum(p => p.Quantity)
                    })
                    .ToList();

                var orders = companyLinks
                    .Select(l => new CompanyPurchaseOrderRefDto { OrderId = l.OrderId, OrderCode = l.Order.Code })
                    .DistinctBy(o => o.OrderId)
                    .ToList();

                return new CompanyPurchaseRequestDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.Name,
                    Products = products,
                    Orders = orders
                };
            }).ToList();

            return new DataTablePaginationResponseDto<CompanyPurchaseRequestDto>
            {
                Draw = request.Draw,
                Data = data,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount
            };
        }
    }
}
