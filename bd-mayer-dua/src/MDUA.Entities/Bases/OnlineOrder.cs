using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "OnlineOrder", Namespace = "http://www.piistech.com//entities")]
	public partial class OnlineOrder : OnlineOrderBase
	{
		#region Exernal Properties
		private SalesOrderHeader _ConfirmedSalesOrderIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="SalesOrderHeader"/>.
		/// </summary>
		/// <value>The source SalesOrderHeader for _ConfirmedSalesOrderIdObject.</value>
		[DataMember]
		public SalesOrderHeader ConfirmedSalesOrderIdObject
      	{
            get { return this._ConfirmedSalesOrderIdObject; }
            set { this._ConfirmedSalesOrderIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(OnlineOrder))
            {
                return false;
            }			
			
			 OnlineOrder _paramObj = obj as OnlineOrder;
            if (_paramObj != null)
            {			
                return (_paramObj.Id == this.Id && _paramObj.CustomPropertyMatch(this));
            }
            else
            {
                return base.Equals(obj);
            }
		}
		#endregion
		
		#region Orverride HashCode
		 public override int GetHashCode()
        {
            return base.Id.GetHashCode();
        }
		#endregion		
	}
}
