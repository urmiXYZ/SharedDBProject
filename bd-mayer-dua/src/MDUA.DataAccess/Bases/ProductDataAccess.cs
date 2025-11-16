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
    public partial class ProductDataAccess : BaseDataAccess, IProductDataAccess
    {
        #region Constants
        private const string INSERTPRODUCT = "InsertProduct";
        private const string UPDATEPRODUCT = "UpdateProduct";
        private const string DELETEPRODUCT = "DeleteProduct";
        private const string GETPRODUCTBYID = "GetProductById";
        private const string GETALLPRODUCT = "GetAllProduct";
        private const string GETPAGEDPRODUCT = "GetPagedProduct";
        private const string GETPRODUCTBYCOMPANYID = "GetProductByCompanyId";
        private const string GETPRODUCTMAXIMUMID = "GetProductMaximumId";
        private const string GETPRODUCTROWCOUNT = "GetProductRowCount";
        private const string GETPRODUCTBYQUERY = "GetProductByQuery";
        #endregion

        #region Constructors
        public ProductDataAccess(IConfiguration configuration) : base(configuration) { }
        public ProductDataAccess(ClientContext context) : base(context) { }
        public ProductDataAccess(SqlTransaction transaction) : base(transaction) { }
        public ProductDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams Method
        private void AddCommonParams(SqlCommand cmd, ProductBase productObject)
        {
            AddParameter(cmd, pInt32(ProductBase.Property_CompanyId, productObject.CompanyId));
            AddParameter(cmd, pNVarChar(ProductBase.Property_ProductName, 200, productObject.ProductName));
            AddParameter(cmd, pInt32(ProductBase.Property_ReorderLevel, productObject.ReorderLevel));
            AddParameter(cmd, pNVarChar(ProductBase.Property_Barcode, 100, productObject.Barcode));
            AddParameter(cmd, pInt32(ProductBase.Property_CategoryId, productObject.CategoryId));
            AddParameter(cmd, pNVarChar(ProductBase.Property_Description, productObject.Description));
            AddParameter(cmd, pNVarChar(ProductBase.Property_Slug, 400, productObject.Slug));
            AddParameter(cmd, pDecimal(ProductBase.Property_BasePrice, 9, productObject.BasePrice));
            AddParameter(cmd, pBool(ProductBase.Property_IsVariantBased, productObject.IsVariantBased));
            AddParameter(cmd, pBool(ProductBase.Property_IsActive, productObject.IsActive));
            AddParameter(cmd, pNVarChar(ProductBase.Property_CreatedBy, 100, productObject.CreatedBy));
            AddParameter(cmd, pDateTime(ProductBase.Property_CreatedAt, productObject.CreatedAt));
            AddParameter(cmd, pNVarChar(ProductBase.Property_UpdatedBy, 100, productObject.UpdatedBy));
            AddParameter(cmd, pDateTime(ProductBase.Property_UpdatedAt, productObject.UpdatedAt));

        }
        #endregion

        #region Insert Method
        public long Insert(ProductBase productObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTPRODUCT);

                AddParameter(cmd, pInt32Out(ProductBase.Property_Id));
                AddCommonParams(cmd, productObject);

                long result = InsertRecord(cmd);
                if (result > 0)
                {
                    productObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    productObject.Id = (Int32)GetOutParameter(cmd, ProductBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(productObject, x);
            }
        }
        #endregion

        #region Update Method
        public long Update(ProductBase productObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATEPRODUCT);

                AddParameter(cmd, pInt32(ProductBase.Property_Id, productObject.Id));
                AddCommonParams(cmd, productObject);

                long result = UpdateRecord(cmd);
                if (result > 0)
                    productObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(productObject, x);
            }
        }
        #endregion

        #region Delete Method
        public long Delete(Int32 _Id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETEPRODUCT);

                AddParameter(cmd, pInt32(ProductBase.Property_Id, _Id));

                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(Product), _Id, x);
            }

        }
        #endregion

        #region Get By Id Method
        // FIX: The implementation for Get(Int32 _Id) is MOVED to the partial 
        // class MDUA.DataAccess/ProductDataAccess.Base.cs to fix the compilation error
        // caused by the missing stored procedure 'GetProductById'.
        // The declaration remains in the interface.
        public Product Get(Int32 _Id)
        {
            return GetProductById(_Id); // <-- calls the inline SQL method from your partial class
        }

        #endregion

        #region GetAll Method
        public ProductList GetAll()
        {
            using (SqlCommand cmd = GetSPCommand(GETALLPRODUCT))
            {
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public ProductList GetByCompanyId(Int32 _CompanyId)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTBYCOMPANYID))
            {
                AddParameter(cmd, pInt32(ProductBase.Property_CompanyId, _CompanyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public ProductList GetPaged(PagedRequest request)
        {
            using (SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCT))
            {
                AddParameter(cmd, pInt32Out("TotalRows"));
                AddParameter(cmd, pInt32("PageIndex", request.PageIndex));
                AddParameter(cmd, pInt32("RowPerPage", request.RowPerPage));
                AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause));
                AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn));
                AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder));

                ProductList _ProductList = GetList(cmd, ALL_AVAILABLE_RECORDS);
                request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
                return _ProductList;
            }
        }

        public ProductList GetByQuery(String query)
        {
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTBYQUERY))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        #endregion

        #region Get Product Maximum Id Method
        public Int32 GetMaxId()
        {
            Int32 _MaximumId = 0;
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTMAXIMUMID))
            {
                SqlDataReader reader;
                _MaximumId = (Int32)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return _MaximumId;
        }
        #endregion

        #region Get Product Row Count Method
        public Int32 GetRowCount()
        {
            Int32 _ProductRowCount = 0;
            using (SqlCommand cmd = GetSPCommand(GETPRODUCTROWCOUNT))
            {
                SqlDataReader reader;
                _ProductRowCount = (Int32)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return _ProductRowCount;
        }
        #endregion

        #region Fill Methods
        protected void FillObject(ProductBase productObject, SqlDataReader reader, int start)
        {
            productObject.Id = reader.GetInt32(start + 0);
            productObject.CompanyId = reader.GetInt32(start + 1);
            productObject.ProductName = reader.GetString(start + 2);
            productObject.ReorderLevel = reader.GetInt32(start + 3);
            if (!reader.IsDBNull(4)) productObject.Barcode = reader.GetString(start + 4);
            if (!reader.IsDBNull(5)) productObject.CategoryId = reader.GetInt32(start + 5);
            if (!reader.IsDBNull(6)) productObject.Description = reader.GetString(start + 6);
            if (!reader.IsDBNull(7)) productObject.Slug = reader.GetString(start + 7);
            if (!reader.IsDBNull(8)) productObject.BasePrice = reader.GetDecimal(start + 8);
            if (!reader.IsDBNull(9)) productObject.IsVariantBased = reader.GetBoolean(start + 9);
            productObject.IsActive = reader.GetBoolean(start + 10);
            if (!reader.IsDBNull(11)) productObject.CreatedBy = reader.GetString(start + 11);
            if (!reader.IsDBNull(12)) productObject.CreatedAt = reader.GetDateTime(start + 12);
            if (!reader.IsDBNull(13)) productObject.UpdatedBy = reader.GetString(start + 13);
            if (!reader.IsDBNull(14)) productObject.UpdatedAt = reader.GetDateTime(start + 14);
            FillBaseObject(productObject, reader, (start + 15));
            if (productObject is Product p)
            {
                try
                {
                    int companyNameOrdinal = reader.GetOrdinal("CompanyName");
                    if (!reader.IsDBNull(companyNameOrdinal))
                        p.CompanyName = reader.GetString(companyNameOrdinal);
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    int stockOrdinal = reader.GetOrdinal("TotalStockQuantity");
                    if (!reader.IsDBNull(stockOrdinal))
                        p.TotalStockQuantity = reader.GetInt32(stockOrdinal);
                }
                catch (IndexOutOfRangeException) { }
            }




            productObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        protected void FillObject(ProductBase productObject, SqlDataReader reader)
        {
            FillObject(productObject, reader, 0);
        }

        private Product GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);

            using (reader)
            {
                if (reader.Read())
                {
                    var productObject = new Product(); // derived from ProductBase
                    FillObject(productObject, reader); // accepts ProductBase
                    return productObject;
                }
                else
                {
                    return null;
                }
            }
        }


        private ProductList GetList(SqlCommand cmd, long rows)
        {
            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);
            ProductList list = new ProductList();

            using (reader)
            {
                while (reader.Read() && rows-- != 0)
                {
                    Product productObject = new Product();
                    FillObject(productObject, reader);
                    list.Add(productObject);
                }
                reader.Close();
            }

            return list;
        }

        #endregion

        // The Extended Implementation methods (including GetProductByBarcode, 
        // GetActiveProductsByCompany, SearchProducts, and the overridden Get) 
        // are all fulfilled by MDUA.DataAccess/ProductDataAccess.Base.cs

    }
}