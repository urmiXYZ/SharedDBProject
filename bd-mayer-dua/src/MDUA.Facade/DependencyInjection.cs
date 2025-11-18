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
            // Data Access Layer
            services.AddScoped<IUserLoginDataAccess, UserLoginDataAccess>();
            services.AddScoped<IPermissionGroupMapDataAccess, PermissionGroupMapDataAccess>();
            services.AddScoped<IPermissionDataAccess, PermissionDataAccess>();              // ← ADD THIS
            services.AddScoped<IPermissionGroupDataAccess, PermissionGroupDataAccess>();    // ← If your facade needs it
            services.AddScoped<IUserPermissionDataAccess, UserPermissionDataAccess>();
            services.AddScoped<IPermissionDataAccess, PermissionDataAccess>();
            services.AddScoped<IAttributeNameDataAccess, AttributeNameDataAccess>();

            // Product-related
            services.AddScoped<IProductDataAccess, ProductDataAccess>();
            services.AddScoped<IProductImageDataAccess, ProductImageDataAccess>();
            services.AddScoped<IProductReviewDataAccess, ProductReviewDataAccess>();
            services.AddScoped<IProductVariantDataAccess, ProductVariantDataAccess>();
            services.AddScoped<IProductDiscountDataAccess, ProductDiscountDataAccess>();
            services.AddScoped<IProductCategoryDataAccess, ProductCategoryDataAccess>();
            services.AddScoped<IProductAttributeDataAccess, ProductAttributeDataAccess>();
            services.AddTransient<IVariantPriceStockDataAccess, VariantPriceStockDataAccess>();

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
