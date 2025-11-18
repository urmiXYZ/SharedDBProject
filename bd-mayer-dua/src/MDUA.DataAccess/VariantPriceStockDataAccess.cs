using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using MDUA.Framework.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace MDUA.DataAccess
{
	public partial class VariantPriceStockDataAccess
	{
        public int Insert(VariantPriceStock vps)
        {
            using SqlCommand cmd = GetSPCommand("InsertVariantPriceStock");

            // Add all parameters from the entity
            // Note: The @Id is an INPUT, not an OUTPUT
            AddParameter(cmd, pInt32("Id", vps.Id));
            AddParameter(cmd, pDecimal("Price", vps.Price));
            AddParameter(cmd, pDecimal("CompareAtPrice", vps.CompareAtPrice));
            AddParameter(cmd, pDecimal("CostPrice", vps.CostPrice));
            AddParameter(cmd, pInt32("StockQty", vps.StockQty));
            AddParameter(cmd, pBool("TrackInventory", vps.TrackInventory));
            AddParameter(cmd, pBool("AllowBackorder", vps.AllowBackorder));
            AddParameter(cmd, pInt32("WeightGrams", vps.WeightGrams));

            // Use ExecuteNonQuery for inserts that don't return an identity
            // Or use your framework's equivalent. Assuming InsertRecord handles this.
            long result = InsertRecord(cmd);

            return (int)result; // Returns the row count (1 if successful)
        }

        public long UpdatePrice(int variantId, decimal price)
        {
            // We use the batch update + SELECT 1 trick
            string SQLQuery = @"
        UPDATE ProductVariant 
        SET VariantPrice = @Price, UpdatedAt = GETDATE() 
        WHERE Id = @Id;

        UPDATE VariantPriceStock 
        SET Price = @Price 
        WHERE Id = @Id;
        
        SELECT 1;";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", variantId));
                AddParameter(cmd, pDecimal("Price", price));

                // ✅ Step 1: Declare the variable explicitly
                SqlDataReader reader;

                // ✅ Step 2: Pass it to the method
                long result = SelectRecords(cmd, out reader);

                // ✅ Step 3: Clean up
                // We verify 'reader' exists before closing it
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                // Return 1 so the Controller knows it succeeded
                return 1;
            }
        }
    }	
}
