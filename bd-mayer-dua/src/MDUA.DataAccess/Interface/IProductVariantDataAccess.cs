using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IProductVariantDataAccess : ICommonDataAccess<ProductVariant, ProductVariantList, ProductVariantBase>
    {
        /// <summary>
        /// Gets all variants for a specific product.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <returns>List of ProductVariant.</returns>
        ProductVariantList GetByProductId(int productId);
    }
}
