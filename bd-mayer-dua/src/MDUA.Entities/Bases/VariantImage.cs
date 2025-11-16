using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "VariantImage", Namespace = "http://www.piistech.com//entities")]
	public partial class VariantImage : VariantImageBase
	{
		#region Exernal Properties
		private Variant _VariantIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Variant"/>.
		/// </summary>
		/// <value>The source Variant for _VariantIdObject.</value>
		[DataMember]
		public Variant VariantIdObject
      	{
            get { return this._VariantIdObject; }
            set { this._VariantIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(VariantImage))
            {
                return false;
            }			
			
			 VariantImage _paramObj = obj as VariantImage;
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
