using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Entities;
using MDUA.Framework.DataAccess;   // contains BaseDataAccess + Query<T>
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
    // Do NOT inherit again (already inherited in the generated file)
    public partial class AttributeNameDataAccess
    {
        public List<AttributeValue> GetValuesByAttributeId(int attributeId)
        {
            string query = $"AttributeId = {attributeId}";

            using (SqlCommand cmd = GetSPCommand("GetAttributeValueByQuery"))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                List<AttributeValue> list = new List<AttributeValue>();
                using (reader)
                {
                    while (reader.Read())
                    {
                        var av = new AttributeValue
                        {
                            Id = reader.GetInt32(0),
                            AttributeId = reader.GetInt32(1),
                            Value = reader.GetString(2)
                        };
                        list.Add(av);
                    }
                }
                return list;
            }
        }


        public List<AttributeName> GetByProductId(int productId)
        {
            // This joins the AttributeName table with the ProductAttribute table
            // to get ONLY the attributes used by this specific product.
            string SQLQuery = @"
        SELECT DISTINCT a.Id, a.Name
        FROM AttributeName a
        JOIN ProductAttribute pa ON a.Id = pa.AttributeId
        WHERE pa.ProductId = @ProductId
        ORDER BY a.Name";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("ProductId", productId));
                // Pass 0 as the second argument to get all rows
                return GetList(cmd, 0);
            }
        }

        public List<AttributeName> GetMissingAttributesForVariant(int productId, int variantId)
        {
            var list = new List<AttributeName>();

            // ✅ Direct SQL: Selects attributes NOT linked to this variant
            string SQLQuery = @"
        SELECT Id, Name 
        FROM AttributeName 
        WHERE Id NOT IN (
            SELECT AttributeId 
            FROM VariantAttributeValue 
            WHERE VariantId = @VariantId
        )
        ORDER BY Name";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                // Your helper adds '@' automatically, so we pass "VariantId"
                AddParameter(cmd, pInt32("VariantId", variantId));

                // ✅ Use SelectRecords (Raw Reader) instead of GetList
                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    while (reader.Read())
                    {
                        list.Add(new AttributeName
                        {
                            Id = reader.GetInt32(0), // Index 0 = Id
                            Name = reader.GetString(1) // Index 1 = Name
                        });
                    }
                    reader.Close();
                }
            }

            return list;
        }

        public string GetValueName(int valueId)
        {
            string SQLQuery = "SELECT Value FROM AttributeValue WHERE Id = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", valueId));

                // ✅ Use SelectRecords instead of ExecuteScalar
                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                string result = "";

                using (reader)
                {
                    if (reader.Read())
                    {
                        // Get the first column (Index 0) as a string
                        if (!reader.IsDBNull(0))
                        {
                            result = reader.GetString(0);
                        }
                    }
                    reader.Close();
                }

                return result;
            }
        }
    }
}
