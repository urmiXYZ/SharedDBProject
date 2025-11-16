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
    public partial class ProductReviewDataAccess : BaseDataAccess, IProductReviewDataAccess
    {
        #region Constants
        private const string INSERTPRODUCTREVIEW = "InsertProductReview";
        private const string UPDATEPRODUCTREVIEW = "UpdateProductReview";
        private const string DELETEPRODUCTREVIEW = "DeleteProductReview";
        private const string GETPRODUCTREVIEWBYID = "GetProductReviewById";
        private const string GETALLPRODUCTREVIEW = "GetAllProductReview";
        private const string GETPAGEDPRODUCTREVIEW = "GetPagedProductReview";
        private const string GETPRODUCTREVIEWBYPRODUCTID = "GetProductReviewByProductId";
        private const string GETPRODUCTREVIEWMAXIMUMID = "GetProductReviewMaximumId";
        private const string GETPRODUCTREVIEWROWCOUNT = "GetProductReviewRowCount";
        private const string GETPRODUCTREVIEWBYQUERY = "GetProductReviewByQuery";
        #endregion

        #region Constructors
        public ProductReviewDataAccess(IConfiguration configuration) : base(configuration) { }
        public ProductReviewDataAccess(ClientContext context) : base(context) { }
        public ProductReviewDataAccess(SqlTransaction transaction) : base(transaction) { }
        public ProductReviewDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams
        private void AddCommonParams(SqlCommand cmd, ProductReviewBase review)
        {
            AddParameter(cmd, pInt32(ProductReviewBase.Property_ProductId, review.ProductId));
            AddParameter(cmd, pNVarChar(ProductReviewBase.Property_CustomerName, 150, review.CustomerName));
            AddParameter(cmd, pInt32(ProductReviewBase.Property_Rating, review.Rating));
            AddParameter(cmd, pNVarChar(ProductReviewBase.Property_ReviewText, 500, review.ReviewText));
            AddParameter(cmd, pBool(ProductReviewBase.Property_IsApproved, review.IsApproved));
            AddParameter(cmd, pNVarChar(ProductReviewBase.Property_CreatedBy, 100, review.CreatedBy));
            AddParameter(cmd, pDateTime(ProductReviewBase.Property_CreatedAt, review.CreatedAt));
            AddParameter(cmd, pNVarChar(ProductReviewBase.Property_UpdatedBy, 100, review.UpdatedBy));
            AddParameter(cmd, pDateTime(ProductReviewBase.Property_UpdatedAt, review.UpdatedAt));
        }
        #endregion

        #region CRUD
        public long Insert(ProductReviewBase review)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTPRODUCTREVIEW);
                AddParameter(cmd, pInt32Out(ProductReviewBase.Property_Id));
                AddCommonParams(cmd, review);

                long result = InsertRecord(cmd);
                if (result > 0)
                {
                    review.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    review.Id = (Int32)GetOutParameter(cmd, ProductReviewBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(review, x);
            }
        }

        public long Update(ProductReviewBase review)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATEPRODUCTREVIEW);
                AddParameter(cmd, pInt32(ProductReviewBase.Property_Id, review.Id));
                AddCommonParams(cmd, review);

                long result = UpdateRecord(cmd);
                if (result > 0)
                    review.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;

                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(review, x);
            }
        }

        public long Delete(int id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETEPRODUCTREVIEW);
                AddParameter(cmd, pInt32(ProductReviewBase.Property_Id, id));
                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(ProductReview), id, x);
            }
        }

        public ProductReview Get(int id)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWBYID))
            {
                AddParameter(cmd, pInt32(ProductReviewBase.Property_Id, id));
                return GetObject(cmd);
            }
        }
        #endregion

        #region GetAll / Paged / Query / Counts
        public ProductReviewList GetAll()
        {
            using (SqlCommand cmd = GetSPCommand(GETALLPRODUCTREVIEW))
            {
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public ProductReviewList GetPaged(PagedRequest request)
        {
            using (SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTREVIEW))
            {
                AddParameter(cmd, pInt32Out("TotalRows"));
                AddParameter(cmd, pInt32("PageIndex", request.PageIndex));
                AddParameter(cmd, pInt32("RowPerPage", request.RowPerPage));
                AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause));
                AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn));
                AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder));

                ProductReviewList list = GetList(cmd, ALL_AVAILABLE_RECORDS);
                request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
                return list;
            }
        }

        public ProductReviewList GetByQuery(string query)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWBYQUERY))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public int GetMaxId()
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWMAXIMUMID))
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
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWROWCOUNT))
            {
                SqlDataReader reader;
                int result = (int)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
                return result;
            }
        }

        public ProductReviewList GetByProductId(int productId)
        {
            // Inline SQL in Base file
            return GetProductReviewsByProductId(productId);
        }
        #endregion

        #region Fill Methods
        protected void FillObject(ProductReviewBase review, SqlDataReader reader, int start)
        {
            review.Id = reader.GetInt32(start + 0);
            review.ProductId = reader.GetInt32(start + 1);
            if (!reader.IsDBNull(2)) review.CustomerName = reader.GetString(start + 2);
            review.Rating = reader.GetInt32(start + 3);
            if (!reader.IsDBNull(4)) review.ReviewText = reader.GetString(start + 4);
            review.IsApproved = reader.GetBoolean(start + 5);
            if (!reader.IsDBNull(6)) review.CreatedBy = reader.GetString(start + 6);
            review.CreatedAt = reader.GetDateTime(start + 7);
            if (!reader.IsDBNull(8)) review.UpdatedBy = reader.GetString(start + 8);
            if (!reader.IsDBNull(9)) review.UpdatedAt = reader.GetDateTime(start + 9);
            FillBaseObject(review, reader, (start + 10));

            review.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        protected void FillObject(ProductReviewBase review, SqlDataReader reader)
        {
            FillObject(review, reader, 0);
        }

        private ProductReview GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);

            using (reader)
            {
                if (reader.Read())
                {
                    ProductReview obj = new ProductReview();
                    FillObject(obj, reader);
                    return obj;
                }
                else
                    return null;
            }
        }

        private ProductReviewList GetList(SqlCommand cmd, long rows)
        {
            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);
            ProductReviewList list = new ProductReviewList();

            using (reader)
            {
                while (reader.Read() && rows-- != 0)
                {
                    ProductReview obj = new ProductReview();
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
