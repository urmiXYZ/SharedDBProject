using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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




    }
}
