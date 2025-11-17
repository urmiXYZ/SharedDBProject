using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDUA.Facade
{
    public class ProductFacade : IProductFacade
    {
        private readonly IAttributeNameDataAccess _attributeNameDataAccess;
        private readonly IProductDataAccess _ProductDataAccess;
        private readonly IProductImageDataAccess _ProductImageDataAccess;
        private readonly IProductReviewDataAccess _ProductReviewDataAccess;
        private readonly IProductVariantDataAccess _ProductVariantDataAccess;
        private readonly IProductDiscountDataAccess _ProductDiscountDataAccess;
                private readonly IProductCategoryDataAccess _categoryDataAccess;


        public ProductFacade(
            IProductDataAccess productDataAccess,
            IProductImageDataAccess productImageDataAccess,
            IProductReviewDataAccess productReviewDataAccess,
            IProductVariantDataAccess productVariantDataAccess,
            IProductDiscountDataAccess productDiscountDataAccess,
                    IProductCategoryDataAccess categoryDataAccess,
                    IAttributeNameDataAccess attributeNameDataAccess)
        {
            _ProductDataAccess = productDataAccess;
            _ProductImageDataAccess = productImageDataAccess;
            _ProductReviewDataAccess = productReviewDataAccess;
            _ProductVariantDataAccess = productVariantDataAccess;
            _ProductDiscountDataAccess = productDiscountDataAccess;
            _categoryDataAccess = categoryDataAccess;
            _attributeNameDataAccess = attributeNameDataAccess;
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

        public long AddProduct(Product product, string username, int companyId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            // 1️⃣ Prepare product fields
            product.CompanyId = companyId;
            product.CreatedBy = username;
            product.UpdatedBy = username;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.ReorderLevel = product.ReorderLevel < 0 ? 0 : product.ReorderLevel;

            // 2️⃣ INSERT PRODUCT
            long productId = _ProductDataAccess.Insert(product);

            if (productId <= 0)
                return productId;

            // 3️⃣ INSERT VARIANTS
            foreach (var variant in product.Variants)
            {
                variant.ProductId = (int)productId;
                variant.CreatedBy = username;
                variant.CreatedAt = DateTime.Now;

                // Insert ProductVariant row
                int variantId = _ProductVariantDataAccess.Insert(variant);

                // 4️⃣ INSERT VARIANT ATTRIBUTE VALUES
                if (variant.AttributeValueIds != null)
                {
                    int displayOrder = 0;

                    foreach (int valueId in variant.AttributeValueIds)
                    {
                        _ProductVariantDataAccess.InsertVariantAttributeValue(
                            variantId,
                            valueId,
                            displayOrder++
                        );
                    }
                }
            }

            return productId;
        }

        public ProductList GetLastFiveProducts()
        {
            return _ProductDataAccess.GetLastFiveProducts();
        }

        public List<Product> GetAllProductsWithCategory()
        {
            var products = _ProductDataAccess.GetAll(); // returns List<Product> or ProductList

            // Get all categories in one query
            var categories = _categoryDataAccess.GetAll().ToDictionary(c => c.Id, c => c.Name);

            // Fill CategoryName for each product
            foreach (var p in products)
            {
                if (p.CategoryId.HasValue && categories.ContainsKey(p.CategoryId.Value))
                    p.CategoryName = categories[p.CategoryId.Value];
                else
                    p.CategoryName = "N/A";
            }

            return products.ToList();
        }

        public UserLoginResult GetAddProductData(int userId)
        {
            var result = new UserLoginResult
            {
                Categories = _categoryDataAccess.GetAll()?.ToList() ?? new List<ProductCategory>(),
                Attributes = _attributeNameDataAccess.GetAll()?.ToList() ?? new List<AttributeName>()
            };

            return result;
        }

        public List<AttributeValue> GetAttributeValues(int attributeId)
        {
            return _attributeNameDataAccess.GetValuesByAttributeId(attributeId);
        }

        #endregion
    }
}
