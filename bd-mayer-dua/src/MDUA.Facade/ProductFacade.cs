using MDUA.DataAccess.Interface;
using MDUA.Entities.List;
using MDUA.Entities;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MDUA.Entities.Bases;

namespace MDUA.Facade
{
    public class ProductFacade : IProductFacade
    {
        private readonly IProductDataAccess _ProductDataAccess;
        private readonly IProductImageDataAccess _ProductImageDataAccess;
        private readonly IProductReviewDataAccess _ProductReviewDataAccess;
        private readonly IProductVariantDataAccess _ProductVariantDataAccess;
        private readonly IProductDiscountDataAccess _ProductDiscountDataAccess;

        public ProductFacade(
            IProductDataAccess productDataAccess,
            IProductImageDataAccess productImageDataAccess,
            IProductReviewDataAccess productReviewDataAccess,
            IProductVariantDataAccess productVariantDataAccess,
            IProductDiscountDataAccess productDiscountDataAccess)
        {
            _ProductDataAccess = productDataAccess;
            _ProductImageDataAccess = productImageDataAccess;
            _ProductReviewDataAccess = productReviewDataAccess;
            _ProductVariantDataAccess = productVariantDataAccess;
            _ProductDiscountDataAccess = productDiscountDataAccess;
        }

        #region Common Implementation
        public long Delete(int id) => _ProductDataAccess.Delete(id);

        public Product Get(int id) => _ProductDataAccess.Get(id);

        public ProductList GetAll() => _ProductDataAccess.GetAll();

        public ProductList GetByQuery(string query) => _ProductDataAccess.GetByQuery(query);

        public long Insert(ProductBase obj) => _ProductDataAccess.Insert(obj);

        public long Update(ProductBase obj) => _ProductDataAccess.Update(obj);
        #endregion

        #region Extended Implementation

        public Product GetProductDetailsForWebBySlug(string slug)
        {
            Product product = _ProductDataAccess.GetBySlug(slug);

            if (product == null || !product.IsActive)
                return null;

            // Load related entities
            product.Images = _ProductImageDataAccess.GetByProductId(product.Id);
            product.Reviews = _ProductReviewDataAccess.GetByProductId(product.Id);
            product.Variants = _ProductVariantDataAccess.GetByProductId(product.Id);
            product.ActiveDiscount = _ProductDiscountDataAccess.GetActiveDiscount(product.Id);
            // 3️⃣ Compute presentation data
            decimal basePrice = product.BasePrice ?? 0;
            decimal sellingPrice = basePrice;

            if (product.ActiveDiscount != null && product.ActiveDiscount.DiscountValue > 0)
            {
                if (product.ActiveDiscount.DiscountType.Equals("flat", StringComparison.OrdinalIgnoreCase))
                    sellingPrice -= product.ActiveDiscount.DiscountValue;
                else if (product.ActiveDiscount.DiscountType.Equals("percent", StringComparison.OrdinalIgnoreCase))
                    sellingPrice -= (basePrice * (product.ActiveDiscount.DiscountValue / 100));
            }

            product.SellingPrice = Math.Max(sellingPrice, 0);

            // 4️⃣ Stock logic
            int totalStock = product.Variants.Sum(v => v.StockQty);
            product.TotalStockQuantity = totalStock;

            // 5️⃣ UI shaping
            product.AvailableSizes = product.Variants
                .Select(v => v.VariantName.Split('-').Last().Trim())
                .Distinct()
                .ToList();

            product.DeliveryCharges = new Dictionary<string, int>
            {
                { "dhaka", 50 },
                { "outside", 100 }
            };
            return product;
        }

        public long AddProduct(ProductBase product, string username, int companyId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            // Set required fields
            product.CompanyId = companyId;
            product.CreatedBy = username;
            product.UpdatedBy = username;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.ReorderLevel = product.ReorderLevel < 0 ? 0 : product.ReorderLevel;
            product.IsActive = product.IsActive;

            return Insert(product); // call existing Insert
        }


        #endregion
    }
}
