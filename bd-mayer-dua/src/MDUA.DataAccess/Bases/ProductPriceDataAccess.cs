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
	public partial class ProductPriceDataAccess : BaseDataAccess, IProductPriceDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTPRICE = "InsertProductPrice";
		private const string UPDATEPRODUCTPRICE = "UpdateProductPrice";
		private const string DELETEPRODUCTPRICE = "DeleteProductPrice";
		private const string GETPRODUCTPRICEBYID = "GetProductPriceById";
		private const string GETALLPRODUCTPRICE = "GetAllProductPrice";
		private const string GETPAGEDPRODUCTPRICE = "GetPagedProductPrice";
		private const string GETPRODUCTPRICEBYPRODUCTID = "GetProductPriceByProductId";
		private const string GETPRODUCTPRICEMAXIMUMID = "GetProductPriceMaximumId";
		private const string GETPRODUCTPRICEROWCOUNT = "GetProductPriceRowCount";	
		private const string GETPRODUCTPRICEBYQUERY = "GetProductPriceByQuery";
		#endregion
		
		#region Constructors
		public ProductPriceDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductPriceDataAccess(ClientContext context) : base(context) { }
		public ProductPriceDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductPriceDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productPriceObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductPriceBase productPriceObject)
		{	
			AddParameter(cmd, pInt32(ProductPriceBase.Property_ProductId, productPriceObject.ProductId));
			AddParameter(cmd, pDecimal(ProductPriceBase.Property_SellingPrice, 9, productPriceObject.SellingPrice));
			AddParameter(cmd, pDateTime(ProductPriceBase.Property_EffectiveFrom, productPriceObject.EffectiveFrom));
			AddParameter(cmd, pDateTime(ProductPriceBase.Property_EffectiveTo, productPriceObject.EffectiveTo));
			AddParameter(cmd, pBool(ProductPriceBase.Property_IsActive, productPriceObject.IsActive));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductPrice
        /// </summary>
        /// <param name="productPriceObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductPriceBase productPriceObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTPRICE);
	
				AddParameter(cmd, pInt32Out(ProductPriceBase.Property_Id));
				AddCommonParams(cmd, productPriceObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productPriceObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productPriceObject.Id = (Int32)GetOutParameter(cmd, ProductPriceBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productPriceObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductPrice
        /// </summary>
        /// <param name="productPriceObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductPriceBase productPriceObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTPRICE);
				
				AddParameter(cmd, pInt32(ProductPriceBase.Property_Id, productPriceObject.Id));
				AddCommonParams(cmd, productPriceObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productPriceObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productPriceObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductPrice
        /// </summary>
        /// <param name="Id">Id of the ProductPrice object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTPRICE);	
				
				AddParameter(cmd, pInt32(ProductPriceBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductPrice), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductPrice object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductPrice object to retrieve</param>
        /// <returns>ProductPrice object, null if not found</returns>
		public ProductPrice Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTPRICEBYID))
			{
				AddParameter( cmd, pInt32(ProductPriceBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductPrice objects 
        /// </summary>
        /// <returns>A list of ProductPrice objects</returns>
		public ProductPriceList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTPRICE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductPrice objects by ProductId
        /// </summary>
        /// <returns>A list of ProductPrice objects</returns>
		public ProductPriceList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTPRICEBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductPriceBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductPrice objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductPrice objects</returns>
		public ProductPriceList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTPRICE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductPriceList _ProductPriceList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductPriceList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductPrice objects by query String
        /// </summary>
        /// <returns>A list of ProductPrice objects</returns>
		public ProductPriceList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTPRICEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductPrice Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductPrice
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTPRICEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductPrice Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductPrice
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductPriceRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTPRICEROWCOUNT))
			{
				SqlDataReader reader;
				_ProductPriceRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductPriceRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductPrice object
        /// </summary>
        /// <param name="productPriceObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductPriceBase productPriceObject, SqlDataReader reader, int start)
		{
			
				productPriceObject.Id = reader.GetInt32( start + 0 );			
				productPriceObject.ProductId = reader.GetInt32( start + 1 );			
				productPriceObject.SellingPrice = reader.GetDecimal( start + 2 );			
				productPriceObject.EffectiveFrom = reader.GetDateTime( start + 3 );			
				productPriceObject.EffectiveTo = reader.GetDateTime( start + 4 );			
				productPriceObject.IsActive = reader.GetBoolean( start + 5 );			
			FillBaseObject(productPriceObject, reader, (start + 6));

			
			productPriceObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductPrice object
        /// </summary>
        /// <param name="productPriceObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductPriceBase productPriceObject, SqlDataReader reader)
		{
			FillObject(productPriceObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductPrice object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductPrice object</returns>
		private ProductPrice GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductPrice productPriceObject= new ProductPrice();
					FillObject(productPriceObject, reader);
					return productPriceObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductPrice objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductPrice objects</returns>
		private ProductPriceList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductPrice list
			ProductPriceList list = new ProductPriceList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductPrice productPriceObject = new ProductPrice();
					FillObject(productPriceObject, reader);

					list.Add(productPriceObject);
				}
				
				// Close the reader in order to receive output parameters
				// Output parameters are not available until reader is closed.
				reader.Close();
			}

			return list;
		}
		
		#endregion
	}	
}
