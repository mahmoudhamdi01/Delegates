using Delegates.Interface.IServices.CompanyPurchaseProduct;
using Delegates.Interface.IServices.MasterData.Client;
using Delegates.Interface.IServices.MasterData.Company;
using Delegates.Interface.IServices.MasterData.ProductCategory;
using Delegates.Interface.IServices.MasterData.UnitOfMeasure;
using Delegates.Interface.IServices.MasterData.VisitDestination;
using Delegates.Interface.IServices.MasterData.Warehouse;
using Delegates.Interface.IServices.Order;
using Delegates.Interface.IServices.Product;
using Delegates.Interface.IServices.UserManagement.Login;
using Delegates.Interface.IServices.UserManagement.User;
using Delegates.Interface.IServices.Visit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.Interfaces
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
        IUserService UserService { get; }
        IWarehouseService WarehouseService { get; }
        ICompanyService CompanyService { get; }
        IUnitOfMeasureService UnitOfMeasureService { get; }
        IProductCategoryService ProductCategoryService { get; }
        IVisitDestinationService VisitDestinationService { get; }
        IClientService ClientService { get; }
        IProductService ProductService { get; }
        IOrderService OrderService { get; }
        ICompanyPurchaseService CompanyPurchaseService { get; }
        IVisitService VisitService { get; }
    }
}
