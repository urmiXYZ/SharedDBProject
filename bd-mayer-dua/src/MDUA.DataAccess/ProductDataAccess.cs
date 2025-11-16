using MDUA.Entities;
using MDUA.Entities.List;
using System;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class ProductDataAccess
    {

        public Product GetBySlug(string _Slug)
        {
            string SQLQuery = @"
        SELECT * 
        FROM Product 
        WHERE Slug = @Slug 
          AND IsActive = 1";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pNVarChar("Slug", 400, _Slug)); 

            return GetObject(cmd); 
        }


        public Product GetProductById(Int32 _Id)
        {
            string SQLQuery = @"
        SELECT 
            p.Id,
            p.CompanyId,
            p.ProductName,
            p.ReorderLevel,
            p.Barcode,
            p.CategoryId,
            p.Description,
            p.Slug,
            p.BasePrice,
            p.IsVariantBased,
            p.IsActive,
            p.CreatedBy,
            p.CreatedAt,
            p.UpdatedBy,
            p.UpdatedAt,
            c.CompanyName,
            ISNULL(SUM(vps.StockQty), 0) AS TotalStockQuantity
        FROM 
            Product p
        LEFT JOIN 
            Company c ON p.CompanyId = c.Id
        LEFT JOIN 
            ProductVariant pv ON pv.ProductId = p.Id
        LEFT JOIN 
            VariantPriceStock vps ON vps.Id = pv.Id
        WHERE 
            p.Id = @Id
        GROUP BY 
            p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode, 
            p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased, 
            p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt, c.CompanyName;
    ";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("Id", _Id));

            return GetObject(cmd);
        }

    }
}