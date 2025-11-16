using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "AttributeList", Namespace = "http://www.piistech.com//list")]	
	public class AttributeList : BaseCollection<Attribute>
	{
		#region Constructors
	    public AttributeList() : base() { }
        public AttributeList(Attribute[] list) : base(list) { }
        public AttributeList(List<Attribute> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
