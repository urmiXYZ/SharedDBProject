using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductPriceList", Namespace = "http://www.piistech.com//list")]	
	public class ProductPriceList : BaseCollection<ProductPrice>
	{
		#region Constructors
	    public ProductPriceList() : base() { }
        public ProductPriceList(ProductPrice[] list) : base(list) { }
        public ProductPriceList(List<ProductPrice> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
