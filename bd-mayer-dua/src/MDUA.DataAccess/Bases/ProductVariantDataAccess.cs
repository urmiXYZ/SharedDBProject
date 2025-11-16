using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using MDUA.Framework;
using MDUA.Framework.DataAccess;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
    public partial class ProductVariantDataAccess : BaseDataAccess, IProductVariantDataAccess
    {
        #region Constants
        private const string INSERTPRODUCTVARIANT = "InsertProductVariant";
        private const string UPDATEPRODUCTVARIANT = "UpdateProductVariant";
        private const string DELETEPRODUCTVARIANT = "DeleteProductVariant";
        private const string GETPRODUCTVARIANTBYID = "GetProductVariantById";
        private const string GETALLPRODUCTVARIANT = "GetAllProductVariant";
        private const string GETPAGEDPRODUCTVARIANT = "GetPagedProductVariant";
        private const string GETPRODUCTVARIANTBYPRODUCTID = "GetProductVariantByProductId";
        private const string GETPRODUCTVARIANTMAXIMUMID = "GetProductVariantMaximumId";
        private const string GETPRODUCTVARIANTROWCOUNT = "GetProductVariantRowCount";
        private const string GETPRODUCTVARIANTBYQUERY = "GetProductVariantByQuery";
        #endregion

        #region Constructors
        public ProductVariantDataAccess(IConfiguration configuration) : base(configuration) { }
        public ProductVariantDataAccess(ClientContext context) : base(context) { }
        public ProductVariantDataAccess(SqlTransaction transaction) : base(transaction) { }
        public ProductVariantDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams
        private void AddCommonParams(SqlCommand cmd, ProductVariantBase obj)
        {
            AddParameter(cmd, pInt32(ProductVariantBase.Property_ProductId, obj.ProductId));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_VariantName, 150, obj.VariantName));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_SKU, 50, obj.SKU));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_Barcode, 100, obj.Barcode));
            AddParameter(cmd, pDecimal(ProductVariantBase.Property_VariantPrice, 18, obj.VariantPrice));
            AddParameter(cmd, pBool(ProductVariantBase.Property_IsActive, obj.IsActive));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_CreatedBy, 100, obj.CreatedBy));
            AddParameter(cmd, pDateTime(ProductVariantBase.Property_CreatedAt, obj.CreatedAt));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_UpdatedBy, 100, obj.UpdatedBy));
            AddParameter(cmd, pDateTime(ProductVariantBase.Property_UpdatedAt, obj.UpdatedAt));
        }
        #endregion

        #region Insert / Update / Delete
        public long Insert(ProductVariantBase obj)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTPRODUCTVARIANT);
                AddParameter(cmd, pInt32Out(ProductVariantBase.Property_Id));
                AddCommonParams(cmd, obj);

                long result = InsertRecord(cmd);
                if (result > 0)
                    obj.Id = (int)GetOutParameter(cmd, ProductVariantBase.Property_Id);
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(obj, x);
            }
        }

        public long Update(ProductVariantBase obj)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATEPRODUCTVARIANT);
                AddParameter(cmd, pInt32(ProductVariantBase.Property_Id, obj.Id));
                AddCommonParams(cmd, obj);
                return UpdateRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(obj, x);
            }
        }

        public long Delete(int id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETEPRODUCTVARIANT);
                AddParameter(cmd, pInt32(ProductVariantBase.Property_Id, id));
                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(ProductVariant), id, x);
            }
        }
        #endregion

        #region Get / GetAll / Paged / Query
        public ProductVariant Get(int id)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTBYID))
            {
                AddParameter(cmd, pInt32(ProductVariantBase.Property_Id, id));
                return GetObject(cmd);
            }
        }

        public ProductVariantList GetAll()
        {
            using (SqlCommand cmd = GetSPCommand(GETALLPRODUCTVARIANT))
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
        }

        public ProductVariantList GetByProductId(int productId)
        {
            return GetProductVariantsByProductId(productId); // handled in Base.cs
        }

        public ProductVariantList GetPaged(PagedRequest request)
        {
            using (SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTVARIANT))
            {
                AddParameter(cmd, pInt32Out("TotalRows"));
                AddParameter(cmd, pInt32("PageIndex", request.PageIndex));
                AddParameter(cmd, pInt32("RowPerPage", request.RowPerPage));
                AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause));
                AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn));
                AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder));

                ProductVariantList list = GetList(cmd, ALL_AVAILABLE_RECORDS);
                request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
                return list;
            }
        }

        public ProductVariantList GetByQuery(string query)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTBYQUERY))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        #endregion

        #region GetMaxId / GetRowCount
        public int GetMaxId()
        {
            int maxId = 0;
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTMAXIMUMID))
            {
                SqlDataReader reader;
                maxId = (int)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return maxId;
        }

        public int GetRowCount()
        {
            int rowCount = 0;
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTROWCOUNT))
            {
                SqlDataReader reader;
                rowCount = (int)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return rowCount;
        }
        #endregion

        #region Fill Methods
        protected void FillObject(ProductVariantBase obj, SqlDataReader reader)
        {
            obj.Id = reader.GetInt32(0);
            obj.ProductId = reader.GetInt32(1);
            obj.VariantName = reader.IsDBNull(2) ? "" : reader.GetString(2);
            obj.SKU = reader.IsDBNull(3) ? "" : reader.GetString(3);
            obj.Barcode = reader.IsDBNull(4) ? "" : reader.GetString(4);
            obj.VariantPrice = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5);
            obj.IsActive = reader.IsDBNull(6) ? true : reader.GetBoolean(6);
            obj.CreatedBy = reader.IsDBNull(7) ? "" : reader.GetString(7);
            obj.CreatedAt = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8);
            obj.UpdatedBy = reader.IsDBNull(9) ? "" : reader.GetString(9);
            obj.UpdatedAt = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10);
        }

        private ProductVariant GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);
            using (reader)
            {
                if (reader.Read())
                {
                    ProductVariant obj = new ProductVariant();
                    FillObject(obj, reader);
                    return obj;
                }
            }
            return null;
        }

        private ProductVariantList GetList(SqlCommand cmd, long rows)
        {
            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);
            ProductVariantList list = new ProductVariantList();

            using (reader)
            {
                while (reader.Read() && rows-- != 0)
                {
                    ProductVariant obj = new ProductVariant();
                    FillObject(obj, reader);
                    list.Add(obj);
                }
                reader.Close();
            }
            return list;
        }
        #endregion
    }
}
