using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "OnlineOrderList", Namespace = "http://www.piistech.com//list")]	
	public class OnlineOrderList : BaseCollection<OnlineOrder>
	{
		#region Constructors
	    public OnlineOrderList() : base() { }
        public OnlineOrderList(OnlineOrder[] list) : base(list) { }
        public OnlineOrderList(List<OnlineOrder> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
