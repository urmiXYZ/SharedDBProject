using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDUA.Entities.ProductVariant;

namespace MDUA.Facade.Interface
{
    public interface IProductFacade : ICommonFacade<Product, ProductList, ProductBase>
    {
        // Extended methods for Product
        long AddProduct(Product product, string username, int companyId);

        Product GetProductDetailsForWebBySlug(string slug);
        ProductList GetLastFiveProducts();
        List<Product> GetAllProductsWithCategory();
        UserLoginResult GetAddProductData(int userId);
        List<AttributeValue> GetAttributeValues(int attributeId);
        ProductVariantList GetVariantsByProductId(int productId);

        bool? ToggleProductStatus(int productId);
        Product GetProductDetails(int productId);
        ProductResult GetProductForEdit(int productId);
        long UpdateProduct(Product product, string username);
        long UpdateVariantPrice(int variantId, decimal newPrice, string newSku);
        long DeleteVariant(int variantId);
        List<AttributeName> GetAttributesForProduct(int productId);
        long AddVariantToExistingProduct(ProductVariant variant);
        ProductVariantResult GetVariantsWithAttributes(int productId);
        List<AttributeName> GetMissingAttributesForVariant(int productId, int variantId);

        void AddAttributeToVariant(int variantId, int attributeValueId);
        List<ProductDiscount> GetDiscountsByProductId(int productId);
        long AddDiscount(ProductDiscount discount);
        long UpdateDiscount(ProductDiscount discount);
        long DeleteDiscount(int id);
        Product GetProductWithPrice(int productId);
    }
}
