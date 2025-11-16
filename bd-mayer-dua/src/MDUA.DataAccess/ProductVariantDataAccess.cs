using MDUA.Entities;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class ProductVariantDataAccess
    {
        /// <summary>
        /// Inline SQL fallback for GetProductVariantsByProductId
        /// Joins VariantPriceStock (vps) on v.Id = vps.Id to get stock (StockQty) and price if needed.
        /// </summary>
        public ProductVariantList GetProductVariantsByProductId(int productId)
        {
            string SQLQuery = @"
				SELECT 
					v.Id,
					v.ProductId,
					v.VariantName,
					v.SKU,
					v.Barcode,
					v.VariantPrice,
					v.IsActive,
					v.CreatedBy,
					v.CreatedAt,
					v.UpdatedBy,
					v.UpdatedAt,
					ISNULL(vps.StockQty, 0) AS StockQty,
					ISNULL(vps.Price, 0) AS VPS_Price
				FROM ProductVariant v
				LEFT JOIN VariantPriceStock vps ON v.Id = vps.Id
				WHERE v.ProductId = @ProductId
				ORDER BY v.Id";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("ProductId", productId));

            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);
            ProductVariantList list = new ProductVariantList();

            using (reader)
            {
                while (reader.Read())
                {
                    var variant = new ProductVariant
                    {
                        Id = reader.GetInt32(0),
                        ProductId = reader.GetInt32(1),
                        VariantName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        SKU = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Barcode = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        VariantPrice = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                        IsActive = reader.IsDBNull(6) ? true : reader.GetBoolean(6),
                        CreatedBy = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        CreatedAt = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8),
                        UpdatedBy = reader.IsDBNull(9) ? "" : reader.GetString(9),
                        UpdatedAt = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                        // stock and optional price from VariantPriceStock
                        StockQty = reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
                        // If you want to map VPS_Price, add a property to ProductVariant or ignore it here
                    };

                    list.Add(variant);
                }
                reader.Close();
            }

            return list;
        }
    }
}
