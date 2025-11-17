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


    }
}
