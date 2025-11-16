using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
    using MDUA.Entities;
using MDUA.Entities.Bases;


namespace MDUA.Entities
{
    public partial class Product : ProductBase
    {
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();

        [DataMember]
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        [DataMember]
        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();

        [DataMember]
        public ProductDiscount ActiveDiscount { get; set; }

        [DataMember]
        public decimal SellingPrice { get; set; }

        [DataMember]
        public List<string> AvailableSizes { get; set; } = new List<string>();

        [DataMember]
        public Dictionary<string, int> DeliveryCharges { get; set; }

        [DataMember]
        public int TotalStockQuantity { get; set; }

        [DataMember]
        public decimal StockBarPercentage =>
            ReorderLevel > 0 ? Math.Min(100, (TotalStockQuantity / (decimal)ReorderLevel) * 100) : 0;

        [DataMember]
        public string ScarcityMessage =>
            TotalStockQuantity <= ReorderLevel
                ? $"Only {TotalStockQuantity} left in stock!"
                : $"In stock: {TotalStockQuantity}";


    }

    public class ProductResult
    {
        public Product Product { get; set; }
        public List<int> RelatedIds { get; set; }   // For example: related category, variant, or tag IDs
        public bool IsSuccess { get; set; }
        public string Message { get; set; }         // General status or error message
        public bool IsActive { get; set; }          // Convenience property for quick status access
        public string ErrorMessage { get; set; }

    }
}
