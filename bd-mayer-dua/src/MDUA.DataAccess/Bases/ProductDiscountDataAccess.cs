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
    public partial class ProductDiscountDataAccess : BaseDataAccess, IProductDiscountDataAccess
    {
        #region Constants
        private const string INSERTPRODUCTDISCOUNT = "InsertProductDiscount";
        private const string UPDATEPRODUCTDISCOUNT = "UpdateProductDiscount";
        private const string DELETEPRODUCTDISCOUNT = "DeleteProductDiscount";
        private const string GETPRODUCTDISCOUNTBYID = "GetProductDiscountById";
        private const string GETALLPRODUCTDISCOUNT = "GetAllProductDiscount";
        private const string GETPAGEDPRODUCTDISCOUNT = "GetPagedProductDiscount";
        private const string GETPRODUCTDISCOUNTBYPRODUCTID = "GetProductDiscountByProductId";
        private const string GETPRODUCTDISCOUNTMAXIMUMID = "GetProductDiscountMaximumId";
        private const string GETPRODUCTDISCOUNTROWCOUNT = "GetProductDiscountRowCount";
        private const string GETPRODUCTDISCOUNTBYQUERY = "GetProductDiscountByQuery";
        #endregion

        #region Constructors
        public ProductDiscountDataAccess(IConfiguration configuration) : base(configuration) { }
        public ProductDiscountDataAccess(ClientContext context) : base(context) { }
        public ProductDiscountDataAccess(SqlTransaction transaction) : base(transaction) { }
        public ProductDiscountDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams
        private void AddCommonParams(SqlCommand cmd, ProductDiscountBase discount)
        {
            AddParameter(cmd, pInt32(ProductDiscountBase.Property_ProductId, discount.ProductId));
            AddParameter(cmd, pNVarChar(ProductDiscountBase.Property_DiscountType, 50, discount.DiscountType));
            AddParameter(cmd, pDecimal(ProductDiscountBase.Property_DiscountValue, 18, discount.DiscountValue));
            AddParameter(cmd, pInt32(ProductDiscountBase.Property_MinQuantity, discount.MinQuantity));
            AddParameter(cmd, pDateTime(ProductDiscountBase.Property_EffectiveFrom, discount.EffectiveFrom));
            AddParameter(cmd, pDateTime(ProductDiscountBase.Property_EffectiveTo, discount.EffectiveTo));
            AddParameter(cmd, pBool(ProductDiscountBase.Property_IsActive, discount.IsActive));
            AddParameter(cmd, pNVarChar(ProductDiscountBase.Property_CreatedBy, 100, discount.CreatedBy));
            AddParameter(cmd, pDateTime(ProductDiscountBase.Property_CreatedAt, discount.CreatedAt));
            AddParameter(cmd, pNVarChar(ProductDiscountBase.Property_UpdatedBy, 100, discount.UpdatedBy));
            AddParameter(cmd, pDateTime(ProductDiscountBase.Property_UpdatedAt, discount.UpdatedAt));
        }
        #endregion

        #region CRUD
        public long Insert(ProductDiscountBase discount)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTPRODUCTDISCOUNT);
                AddParameter(cmd, pInt32Out(ProductDiscountBase.Property_Id));
                AddCommonParams(cmd, discount);

                long result = InsertRecord(cmd);
                if (result > 0)
                {
                    discount.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    discount.Id = (Int32)GetOutParameter(cmd, ProductDiscountBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(discount, x);
            }
        }

        public long Update(ProductDiscountBase discount)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATEPRODUCTDISCOUNT);
                AddParameter(cmd, pInt32(ProductDiscountBase.Property_Id, discount.Id));
                AddCommonParams(cmd, discount);

                long result = UpdateRecord(cmd);
                if (result > 0)
                    discount.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;

                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(discount, x);
            }
        }

        public long Delete(int id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETEPRODUCTDISCOUNT);
                AddParameter(cmd, pInt32(ProductDiscountBase.Property_Id, id));
                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(ProductDiscount), id, x);
            }
        }

        public ProductDiscount Get(int id)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTBYID))
            {
                AddParameter(cmd, pInt32(ProductDiscountBase.Property_Id, id));
                return GetObject(cmd);
            }
        }
        #endregion

        #region GetAll / Paged / Query / Counts
        public ProductDiscountList GetAll()
        {
            using (SqlCommand cmd = GetSPCommand(GETALLPRODUCTDISCOUNT))
            {
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public ProductDiscountList GetPaged(PagedRequest request)
        {
            using (SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTDISCOUNT))
            {
                AddParameter(cmd, pInt32Out("TotalRows"));
                AddParameter(cmd, pInt32("PageIndex", request.PageIndex));
                AddParameter(cmd, pInt32("RowPerPage", request.RowPerPage));
                AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause));
                AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn));
                AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder));

                ProductDiscountList list = GetList(cmd, ALL_AVAILABLE_RECORDS);
                request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
                return list;
            }
        }

        public ProductDiscountList GetByQuery(string query)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTBYQUERY))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public int GetMaxId()
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTMAXIMUMID))
            {
                SqlDataReader reader;
                int result = (int)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
                return result;
            }
        }

        public int GetRowCount()
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTROWCOUNT))
            {
                SqlDataReader reader;
                int result = (int)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
                return result;
            }
        }

        public ProductDiscountList GetByProductId(int productId)
        {
            // inline SQL implemented in Base file
            return GetProductDiscountsByProductId(productId);
        }

        public ProductDiscount GetActiveDiscount(int productId)
        {
            // inline SQL implemented in Base file
            return GetActiveProductDiscount(productId);
        }
        #endregion

        #region Fill Methods
        protected void FillObject(ProductDiscountBase discount, SqlDataReader reader, int start)
        {
            discount.Id = reader.GetInt32(start + 0);
            discount.ProductId = reader.GetInt32(start + 1);
            discount.DiscountType = reader.GetString(start + 2);
            discount.DiscountValue = reader.GetDecimal(start + 3);
            if (!reader.IsDBNull(4)) discount.MinQuantity = reader.GetInt32(start + 4);
            discount.EffectiveFrom = reader.GetDateTime(start + 5);
            if (!reader.IsDBNull(6)) discount.EffectiveTo = reader.GetDateTime(start + 6);
            discount.IsActive = reader.GetBoolean(start + 7);
            if (!reader.IsDBNull(8)) discount.CreatedBy = reader.GetString(start + 8);
            discount.CreatedAt = reader.GetDateTime(start + 9);
            if (!reader.IsDBNull(10)) discount.UpdatedBy = reader.GetString(start + 10);
            if (!reader.IsDBNull(11)) discount.UpdatedAt = reader.GetDateTime(start + 11);
            FillBaseObject(discount, reader, (start + 12));

            discount.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        protected void FillObject(ProductDiscountBase discount, SqlDataReader reader)
        {
            FillObject(discount, reader, 0);
        }

        private ProductDiscount GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);
            using (reader)
            {
                if (reader.Read())
                {
                    ProductDiscount obj = new ProductDiscount();
                    FillObject(obj, reader);
                    return obj;
                }
                else
                    return null;
            }
        }

        private ProductDiscountList GetList(SqlCommand cmd, long rows)
        {
            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);
            ProductDiscountList list = new ProductDiscountList();

            using (reader)
            {
                while (reader.Read() && rows-- != 0)
                {
                    ProductDiscount obj = new ProductDiscount();
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
