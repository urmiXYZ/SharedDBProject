using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "OnlineOrderBase", Namespace = "http://www.piistech.com//entities")]
	public class OnlineOrderBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			SessionId = 0,
			IPAddress = 1,
			PaymentGateway = 2,
			GatewayTxnId = 3,
			Id = 4,
			Confirmed = 5,
			ConfirmedSalesOrderId = 6
		}
		#endregion
	
		#region Constants
		public const string Property_SessionId = "SessionId";		            
		public const string Property_IPAddress = "IPAddress";		            
		public const string Property_PaymentGateway = "PaymentGateway";		            
		public const string Property_GatewayTxnId = "GatewayTxnId";		            
		public const string Property_Id = "Id";		            
		public const string Property_Confirmed = "Confirmed";		            
		public const string Property_ConfirmedSalesOrderId = "ConfirmedSalesOrderId";		            
		#endregion
		
		#region Private Data Types
		private String _SessionId;	            
		private String _IPAddress;	            
		private String _PaymentGateway;	            
		private String _GatewayTxnId;	            
		private Int32 _Id;	            
		private Boolean _Confirmed;	            
		private Nullable<Int32> _ConfirmedSalesOrderId;	            
		#endregion
		
		#region Properties		
		[DataMember]
		public String SessionId
		{	
			get{ return _SessionId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SessionId, value, _SessionId);
				if (PropertyChanging(args))
				{
					_SessionId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String IPAddress
		{	
			get{ return _IPAddress; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IPAddress, value, _IPAddress);
				if (PropertyChanging(args))
				{
					_IPAddress = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PaymentGateway
		{	
			get{ return _PaymentGateway; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentGateway, value, _PaymentGateway);
				if (PropertyChanging(args))
				{
					_PaymentGateway = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String GatewayTxnId
		{	
			get{ return _GatewayTxnId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_GatewayTxnId, value, _GatewayTxnId);
				if (PropertyChanging(args))
				{
					_GatewayTxnId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 Id
		{	
			get{ return _Id; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Id, value, _Id);
				if (PropertyChanging(args))
				{
					_Id = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean Confirmed
		{	
			get{ return _Confirmed; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Confirmed, value, _Confirmed);
				if (PropertyChanging(args))
				{
					_Confirmed = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> ConfirmedSalesOrderId
		{	
			get{ return _ConfirmedSalesOrderId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ConfirmedSalesOrderId, value, _ConfirmedSalesOrderId);
				if (PropertyChanging(args))
				{
					_ConfirmedSalesOrderId = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  OnlineOrderBase Clone()
		{
			OnlineOrderBase newObj = new  OnlineOrderBase();
			base.CloneBase(newObj);
			newObj.SessionId = this.SessionId;						
			newObj.IPAddress = this.IPAddress;						
			newObj.PaymentGateway = this.PaymentGateway;						
			newObj.GatewayTxnId = this.GatewayTxnId;						
			newObj.Id = this.Id;						
			newObj.Confirmed = this.Confirmed;						
			newObj.ConfirmedSalesOrderId = this.ConfirmedSalesOrderId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(OnlineOrderBase.Property_SessionId, SessionId);				
			info.AddValue(OnlineOrderBase.Property_IPAddress, IPAddress);				
			info.AddValue(OnlineOrderBase.Property_PaymentGateway, PaymentGateway);				
			info.AddValue(OnlineOrderBase.Property_GatewayTxnId, GatewayTxnId);				
			info.AddValue(OnlineOrderBase.Property_Id, Id);				
			info.AddValue(OnlineOrderBase.Property_Confirmed, Confirmed);				
			info.AddValue(OnlineOrderBase.Property_ConfirmedSalesOrderId, ConfirmedSalesOrderId);				
		}
		#endregion

		
	}
}
