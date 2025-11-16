using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductPriceBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductPriceBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			SellingPrice = 2,
			EffectiveFrom = 3,
			EffectiveTo = 4,
			IsActive = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_SellingPrice = "SellingPrice";		            
		public const string Property_EffectiveFrom = "EffectiveFrom";		            
		public const string Property_EffectiveTo = "EffectiveTo";		            
		public const string Property_IsActive = "IsActive";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private Decimal _SellingPrice;	            
		private DateTime _EffectiveFrom;	            
		private DateTime _EffectiveTo;	            
		private Boolean _IsActive;	            
		#endregion
		
		#region Properties		
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
		public Int32 ProductId
		{	
			get{ return _ProductId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductId, value, _ProductId);
				if (PropertyChanging(args))
				{
					_ProductId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal SellingPrice
		{	
			get{ return _SellingPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SellingPrice, value, _SellingPrice);
				if (PropertyChanging(args))
				{
					_SellingPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime EffectiveFrom
		{	
			get{ return _EffectiveFrom; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EffectiveFrom, value, _EffectiveFrom);
				if (PropertyChanging(args))
				{
					_EffectiveFrom = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime EffectiveTo
		{	
			get{ return _EffectiveTo; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EffectiveTo, value, _EffectiveTo);
				if (PropertyChanging(args))
				{
					_EffectiveTo = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsActive
		{	
			get{ return _IsActive; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsActive, value, _IsActive);
				if (PropertyChanging(args))
				{
					_IsActive = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  ProductPriceBase Clone()
		{
			ProductPriceBase newObj = new  ProductPriceBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.SellingPrice = this.SellingPrice;						
			newObj.EffectiveFrom = this.EffectiveFrom;						
			newObj.EffectiveTo = this.EffectiveTo;						
			newObj.IsActive = this.IsActive;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ProductPriceBase.Property_Id, Id);				
			info.AddValue(ProductPriceBase.Property_ProductId, ProductId);				
			info.AddValue(ProductPriceBase.Property_SellingPrice, SellingPrice);				
			info.AddValue(ProductPriceBase.Property_EffectiveFrom, EffectiveFrom);				
			info.AddValue(ProductPriceBase.Property_EffectiveTo, EffectiveTo);				
			info.AddValue(ProductPriceBase.Property_IsActive, IsActive);				
		}
		#endregion

		
	}
}
