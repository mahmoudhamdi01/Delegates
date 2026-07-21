using AutoMapper;
using Delegates.Application.Services.Inventory;
using Delegates.Application.Services.MasterData;
using Delegates.Application.Services.Orders;
using Delegates.Application.Services.UserManagement;
using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Shared;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.CompanyPurchaseProduct;
using Delegates.Interface.IServices.MasterData.Client;
using Delegates.Interface.IServices.MasterData.Company;
using Delegates.Interface.IServices.MasterData.ProductCategory;
using Delegates.Interface.IServices.MasterData.UnitOfMeasure;
using Delegates.Interface.IServices.MasterData.VisitDestination;
using Delegates.Interface.IServices.MasterData.Warehouse;
using Delegates.Interface.IServices.Order;
using Delegates.Interface.IServices.Product;
using Delegates.Interface.IServices.RegisterDevice;
using Delegates.Interface.IServices.UserManagement.Login;
using Delegates.Interface.IServices.UserManagement.User;
using Delegates.Interface.IServices.Visit;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Repositories
{
    public class ServiceManager(
        IUnitOfWork unitOfWork,
        IEntityAuditHelper entityAuditHelper,
        ICurrentUserContext currentUserContext,
        IMapper mapper,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IJwtTokenService jwtTokenService,
        IPushNotificationService pushNotificationService) : IServiceManager
    {
        private readonly Lazy<IAuthService> _lazyAuthService =
            new(() => new AuthService(unitOfWork, passwordHasher, jwtTokenService));
        public IAuthService AuthService => _lazyAuthService.Value;

        private readonly Lazy<IUserService> _lazyUserService =
            new(() => new UserService(unitOfWork, passwordHasher, entityAuditHelper, currentUserContext));
        public IUserService UserService => _lazyUserService.Value;

        private readonly Lazy<IWarehouseService> _lazyWarehouseService =
            new(() => new WarehouseService(unitOfWork, entityAuditHelper, mapper));
        public IWarehouseService WarehouseService => _lazyWarehouseService.Value;

        private readonly Lazy<ICompanyService> _lazyCompanyService =
            new(() => new CompanyService(unitOfWork, entityAuditHelper, mapper));
        public ICompanyService CompanyService => _lazyCompanyService.Value;

        private readonly Lazy<IUnitOfMeasureService> _lazyUnitOfMeasureService =
            new(() => new UnitOfMeasureService(unitOfWork, entityAuditHelper, mapper));
        public IUnitOfMeasureService UnitOfMeasureService => _lazyUnitOfMeasureService.Value;

        private readonly Lazy<IProductCategoryService> _lazyProductCategoryService =
            new(() => new ProductCategoryService(unitOfWork, entityAuditHelper, mapper));
        public IProductCategoryService ProductCategoryService => _lazyProductCategoryService.Value;

        private readonly Lazy<IVisitDestinationService> _lazyVisitDestinationService =
            new(() => new VisitDestinationService(unitOfWork, entityAuditHelper, mapper));
        public IVisitDestinationService VisitDestinationService => _lazyVisitDestinationService.Value;

        private readonly Lazy<IClientService> _lazyClientService =
            new(() => new ClientService(unitOfWork, entityAuditHelper, mapper));
        public IClientService ClientService => _lazyClientService.Value;

        private readonly Lazy<IProductService> _lazyProductService =
            new(() => new ProductService(unitOfWork, entityAuditHelper, mapper));
        public IProductService ProductService => _lazyProductService.Value;

        private readonly Lazy<IOrderService> _lazyOrderService =
            new(() => new OrderService(unitOfWork, entityAuditHelper, pushNotificationService));
        public IOrderService OrderService => _lazyOrderService.Value;

        private readonly Lazy<ICompanyPurchaseService> _lazyCompanyPurchaseService =
            new(() => new CompanyPurchaseService(unitOfWork));
        public ICompanyPurchaseService CompanyPurchaseService => _lazyCompanyPurchaseService.Value;

        private readonly Lazy<IVisitService> _lazyVisitService =
            new(() => new VisitService(unitOfWork, entityAuditHelper, pushNotificationService));
        public IVisitService VisitService => _lazyVisitService.Value;

        private readonly Lazy<IDeviceTokenService> _lazyDeviceTokenService =
            new(() => new DeviceTokenService(unitOfWork, entityAuditHelper));
        public IDeviceTokenService DeviceTokenService => _lazyDeviceTokenService.Value;
    }
}
