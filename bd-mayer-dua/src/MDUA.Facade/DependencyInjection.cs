using Microsoft.Extensions.DependencyInjection;
using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Facade;
using MDUA.Facade.Interface;

namespace MDUA.Facade
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            // Data Access Layer - User & Permissions
            services.AddScoped<IUserLoginDataAccess, UserLoginDataAccess>();
            services.AddScoped<IPermissionGroupMapDataAccess, PermissionGroupMapDataAccess>();
            services.AddScoped<IPermissionDataAccess, PermissionDataAccess>();
            services.AddScoped<IPermissionGroupDataAccess, PermissionGroupDataAccess>();
            services.AddScoped<IUserPermissionDataAccess, UserPermissionDataAccess>();

            // Product-related Data Access
            services.AddScoped<IAttributeNameDataAccess, AttributeNameDataAccess>();
            services.AddScoped<IProductDataAccess, ProductDataAccess>();
            services.AddScoped<IProductImageDataAccess, ProductImageDataAccess>();
            services.AddScoped<IProductReviewDataAccess, ProductReviewDataAccess>();
            services.AddScoped<IProductVariantDataAccess, ProductVariantDataAccess>();
            services.AddScoped<IProductDiscountDataAccess, ProductDiscountDataAccess>();
            services.AddScoped<IProductCategoryDataAccess, ProductCategoryDataAccess>();
            services.AddScoped<IProductAttributeDataAccess, ProductAttributeDataAccess>();

            // ✅ ADDED THIS (Required for your new Variant Image feature)
            services.AddScoped<IVariantImageDataAccess, VariantImageDataAccess>();

            // Changed to Scoped for consistency (unless you specifically need Transient)
            services.AddScoped<IVariantPriceStockDataAccess, VariantPriceStockDataAccess>();

            // Facade Layer
            services.AddServiceFacade();

            return services;
        }

        private static void AddServiceFacade(this IServiceCollection services)
        {
            services.AddScoped<IUserLoginFacade, UserLoginFacade>();
            services.AddScoped<IProductFacade, ProductFacade>();
        }
    }
}