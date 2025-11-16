using System;
using System.Runtime.Serialization;
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    // Assuming this is the definition of the partial ProductVariant class.
    public partial class ProductVariant : ProductVariantBase
    {
        // Add the StockQty property which is fetched from the VariantPriceStock table
        // but exposed here for the Facade to easily calculate total stock.
        [DataMember]
        public int StockQty { get; set; }

        // Add other properties that are fetched from VariantPriceStock if needed, e.g.:
        // [DataMember]
        // public decimal Price { get; set; } 
    }
}