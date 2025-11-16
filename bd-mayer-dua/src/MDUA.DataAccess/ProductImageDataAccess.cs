using MDUA.Entities;
using MDUA.Entities.List;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class ProductImageDataAccess
    {
        public ProductImageList GetProductImagesByProductId(int productId)
        {
            string SQL = @"
                SELECT 
                    Id,
                    ProductId,
                    ImageUrl,
                    IsPrimary,
                    SortOrder,
                    AltText,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM ProductImage
                WHERE ProductId = @ProductId
                ORDER BY SortOrder ASC";

            using SqlCommand cmd = GetSQLCommand(SQL);
            AddParameter(cmd, pInt32("ProductId", productId));

            return GetList(cmd, ALL_AVAILABLE_RECORDS);
        }
    }
}
