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
        long AddProduct(ProductBase product, string username, int companyId); // new method

        Product GetProductDetailsForWebBySlug(string slug);
    }
}
