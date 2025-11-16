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
	public partial class OnlineOrderDataAccess : BaseDataAccess, IOnlineOrderDataAccess
	{
		#region Constants
		private const string INSERTONLINEORDER = "InsertOnlineOrder";
		private const string UPDATEONLINEORDER = "UpdateOnlineOrder";
		private const string DELETEONLINEORDER = "DeleteOnlineOrder";
		private const string GETONLINEORDERBYID = "GetOnlineOrderById";
		private const string GETALLONLINEORDER = "GetAllOnlineOrder";
		private const string GETPAGEDONLINEORDER = "GetPagedOnlineOrder";
		private const string GETONLINEORDERBYCONFIRMEDSALESORDERID = "GetOnlineOrderByConfirmedSalesOrderId";
		private const string GETONLINEORDERMAXIMUMID = "GetOnlineOrderMaximumId";
		private const string GETONLINEORDERROWCOUNT = "GetOnlineOrderRowCount";	
		private const string GETONLINEORDERBYQUERY = "GetOnlineOrderByQuery";
		#endregion
		
		#region Constructors
		public OnlineOrderDataAccess(IConfiguration configuration) : base(configuration) { }
		public OnlineOrderDataAccess(ClientContext context) : base(context) { }
		public OnlineOrderDataAccess(SqlTransaction transaction) : base(transaction) { }
		public OnlineOrderDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="onlineOrderObject"></param>
		private void AddCommonParams(SqlCommand cmd, OnlineOrderBase onlineOrderObject)
		{	
			AddParameter(cmd, pNVarChar(OnlineOrderBase.Property_SessionId, 100, onlineOrderObject.SessionId));
			AddParameter(cmd, pVarChar(OnlineOrderBase.Property_IPAddress, 45, onlineOrderObject.IPAddress));
			AddParameter(cmd, pNVarChar(OnlineOrderBase.Property_PaymentGateway, 50, onlineOrderObject.PaymentGateway));
			AddParameter(cmd, pNVarChar(OnlineOrderBase.Property_GatewayTxnId, 100, onlineOrderObject.GatewayTxnId));
			AddParameter(cmd, pBool(OnlineOrderBase.Property_Confirmed, onlineOrderObject.Confirmed));
			AddParameter(cmd, pInt32(OnlineOrderBase.Property_ConfirmedSalesOrderId, onlineOrderObject.ConfirmedSalesOrderId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts OnlineOrder
        /// </summary>
        /// <param name="onlineOrderObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(OnlineOrderBase onlineOrderObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTONLINEORDER);
	
				AddParameter(cmd, pInt32Out(OnlineOrderBase.Property_Id));
				AddCommonParams(cmd, onlineOrderObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					onlineOrderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					onlineOrderObject.Id = (Int32)GetOutParameter(cmd, OnlineOrderBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(onlineOrderObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates OnlineOrder
        /// </summary>
        /// <param name="onlineOrderObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(OnlineOrderBase onlineOrderObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEONLINEORDER);
				
				AddParameter(cmd, pInt32(OnlineOrderBase.Property_Id, onlineOrderObject.Id));
				AddCommonParams(cmd, onlineOrderObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					onlineOrderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(onlineOrderObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes OnlineOrder
        /// </summary>
        /// <param name="Id">Id of the OnlineOrder object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEONLINEORDER);	
				
				AddParameter(cmd, pInt32(OnlineOrderBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(OnlineOrder), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves OnlineOrder object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the OnlineOrder object to retrieve</param>
        /// <returns>OnlineOrder object, null if not found</returns>
		public OnlineOrder Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETONLINEORDERBYID))
			{
				AddParameter( cmd, pInt32(OnlineOrderBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all OnlineOrder objects 
        /// </summary>
        /// <returns>A list of OnlineOrder objects</returns>
		public OnlineOrderList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLONLINEORDER))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all OnlineOrder objects by ConfirmedSalesOrderId
        /// </summary>
        /// <returns>A list of OnlineOrder objects</returns>
		public OnlineOrderList GetByConfirmedSalesOrderId(Nullable<Int32> _ConfirmedSalesOrderId)
		{
			using( SqlCommand cmd = GetSPCommand(GETONLINEORDERBYCONFIRMEDSALESORDERID))
			{
				
				AddParameter( cmd, pInt32(OnlineOrderBase.Property_ConfirmedSalesOrderId, _ConfirmedSalesOrderId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all OnlineOrder objects by PageRequest
        /// </summary>
        /// <returns>A list of OnlineOrder objects</returns>
		public OnlineOrderList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDONLINEORDER))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				OnlineOrderList _OnlineOrderList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _OnlineOrderList;
			}
		}
		
		/// <summary>
        /// Retrieves all OnlineOrder objects by query String
        /// </summary>
        /// <returns>A list of OnlineOrder objects</returns>
		public OnlineOrderList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETONLINEORDERBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get OnlineOrder Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of OnlineOrder
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETONLINEORDERMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get OnlineOrder Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of OnlineOrder
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _OnlineOrderRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETONLINEORDERROWCOUNT))
			{
				SqlDataReader reader;
				_OnlineOrderRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _OnlineOrderRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills OnlineOrder object
        /// </summary>
        /// <param name="onlineOrderObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(OnlineOrderBase onlineOrderObject, SqlDataReader reader, int start)
		{
			
				if(!reader.IsDBNull(0)) onlineOrderObject.SessionId = reader.GetString( start + 0 );			
				if(!reader.IsDBNull(1)) onlineOrderObject.IPAddress = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) onlineOrderObject.PaymentGateway = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) onlineOrderObject.GatewayTxnId = reader.GetString( start + 3 );			
				onlineOrderObject.Id = reader.GetInt32( start + 4 );			
				onlineOrderObject.Confirmed = reader.GetBoolean( start + 5 );			
				if(!reader.IsDBNull(6)) onlineOrderObject.ConfirmedSalesOrderId = reader.GetInt32( start + 6 );			
			FillBaseObject(onlineOrderObject, reader, (start + 7));

			
			onlineOrderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills OnlineOrder object
        /// </summary>
        /// <param name="onlineOrderObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(OnlineOrderBase onlineOrderObject, SqlDataReader reader)
		{
			FillObject(onlineOrderObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves OnlineOrder object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>OnlineOrder object</returns>
		private OnlineOrder GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					OnlineOrder onlineOrderObject= new OnlineOrder();
					FillObject(onlineOrderObject, reader);
					return onlineOrderObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of OnlineOrder objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of OnlineOrder objects</returns>
		private OnlineOrderList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//OnlineOrder list
			OnlineOrderList list = new OnlineOrderList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					OnlineOrder onlineOrderObject = new OnlineOrder();
					FillObject(onlineOrderObject, reader);

					list.Add(onlineOrderObject);
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
