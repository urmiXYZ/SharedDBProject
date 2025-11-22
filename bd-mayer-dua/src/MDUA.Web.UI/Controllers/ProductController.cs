using MDUA.Entities; 
using MDUA.Entities.Bases;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using static MDUA.Entities.ProductVariant;

namespace MDUA.Web.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductFacade _productFacade;
        private readonly IUserLoginFacade _userLoginFacade;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductFacade productFacade, IUserLoginFacade userLoginFacade, IWebHostEnvironment webHostEnvironment)
        {
            _productFacade = productFacade;
            _userLoginFacade = userLoginFacade;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("product/{slug}")]
        public IActionResult Index(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return BadRequest();

            Product model = _productFacade.GetProductDetailsForWebBySlug(slug);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [Route("product/add")]
        [HttpGet]
        public IActionResult Add()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Index");

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Add"))
                return RedirectToAction("AccessDenied", "Home");

            var model = _productFacade.GetAddProductData(userId.Value);

            return View(model);
        }

        [Route("product/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Index");

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Add"))
                return RedirectToAction("AccessDenied", "Home");

            // 1. Generate Slug if empty
            if (string.IsNullOrWhiteSpace(product.Slug))
            {
                product.Slug = GenerateSlug(product.ProductName);
            }
            else
            {
                // Ensure manually entered slugs are also URL-safe
                product.Slug = GenerateSlug(product.Slug);
            }

            long newProductId = _productFacade.AddProduct(
                product,
                HttpContext.Session.GetString("UserName"),
                HttpContext.Session.GetInt32("CompanyId") ?? 0
            );

            TempData[newProductId > 0 ? "Success" : "Error"] =
                newProductId > 0 ? "Product added successfully!" : "Failed to add product.";

            // Redirect to the 'AllProducts' action in the 'Product' controller
            return RedirectToAction("AllProducts", "Product");
        }

        // Helper method to put inside your Controller
        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;

            string str = phrase.ToLowerInvariant();
            // Remove invalid characters
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // Convert multiple spaces into one space   
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim();
            // Replace spaces with hyphens
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-");

            return str;
        }

        [Route("product/all")]
        public IActionResult AllProducts()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Index", "Home");

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.View"))
                return RedirectToAction("AccessDenied", "Home");

            // now works
            var products = _productFacade.GetAllProductsWithCategory();
            return View(products);
        }
        [HttpGet]
        [Route("product/get-attribute-values")]
        public IActionResult GetAttributeValues(int attributeId)
        {
            var values = _productFacade.GetAttributeValues(attributeId);

            var result = values.Select(v => new
            {
                id = v.Id,
                value = v.Value
            }).ToList();

            return new JsonResult(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Important for security
        [Route("product/toggle-status")]
        public IActionResult ToggleStatus(int productId)
        {
            // Check for authorization. Use the permission that makes sense for you.
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // Assuming 'Product.Edit' is the correct permission
            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Edit"))
                return Forbid();

            // Call the facade
            bool? newStatus = _productFacade.ToggleProductStatus(productId);

            if (newStatus == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Return the new status to the JavaScript
            return Json(new { success = true, newIsActive = newStatus.Value });
        }
        [HttpGet]
        [Route("product/get-details-partial")]
        public IActionResult GetProductDetailsPartial(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // Check for authorization
            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.View"))
                return Forbid();

            Product model = _productFacade.GetProductDetails(productId);

            if (model == null)
                return NotFound();

            return PartialView("_ProductDetailsPartial", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-confirmed")]
        public IActionResult DeleteConfirmed(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Delete"))
                return Forbid();

            try
            {
                long result = _productFacade.Delete(productId);

                if (result > 0)
                {
                    // ✅ ADD THIS LINE
                    TempData["Success"] = "Product deleted successfully!";

                    // The JS will reload the page, and the Layout will read the TempData above
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Product not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("product/get-edit-partial")]
        public IActionResult GetEditPartial(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Edit"))
                return Forbid();

            var model = _productFacade.GetProductForEdit(productId);

            if (model.Product == null)
                return NotFound();

            // Return the new partial view
            return PartialView("_EditProductPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/edit-product")]
        public IActionResult EditProduct(Product product) // Model binding will get the form data
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Edit"))
                return Forbid();

            var username = HttpContext.Session.GetString("UserName");

            // Call the new Update method
            long result = _productFacade.UpdateProduct(product, username);

            if (result > 0)
                TempData["Success"] = "Product updated successfully!";
            else
                TempData["Error"] = "Failed to update product.";

            // Redirect back to the list, which will show the updated data
            return RedirectToAction("AllProducts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-variant-price")] // Renamed route
        public IActionResult UpdateVariantPrice(int variantId, decimal newPrice, string newSku)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Edit"))
                return Forbid();

            // Call the updated facade method
            long result = _productFacade.UpdateVariantPrice(variantId, newPrice, newSku);

            if (result > 0)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "Could not update variant price." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-variant")]
        public IActionResult DeleteVariant(int variantId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // Check permission (using Product.Edit as a fallback)
            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Delete"))
                return Forbid();

            // ✅ IMPORTANT: Make sure this calls '.DeleteVariant', not '.Delete'
            long result = _productFacade.DeleteVariant(variantId);

            if (result > 0)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "Could not delete variant." });
        }

        [HttpGet]
        [Route("product/get-variants")]
        public IActionResult GetVariantsPartial(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // ✅ Clean and simple: Get the Result object
            ProductVariantResult model = _productFacade.GetVariantsWithAttributes(productId);

            return PartialView("_ProductVariantsPartial", model);
        }

        // Action to Save the new variant
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-single-variant")]
        public IActionResult AddSingleVariant(ProductVariant newVariant)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();
            var username = HttpContext.Session.GetString("UserName");

            newVariant.CreatedBy = username;
            newVariant.CreatedAt = DateTime.Now;
            newVariant.IsActive = true;

            long result = _productFacade.AddVariantToExistingProduct(newVariant);

            if (result > 0) return Json(new { success = true });
            return Json(new { success = false, message = "Failed to add variant." });
        }

        [HttpGet]
        [Route("product/get-missing-attributes")]
        public IActionResult GetMissingAttributes(int productId, int variantId)
        {
            var list = _productFacade.GetMissingAttributesForVariant(productId, variantId);
            return Json(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-attribute-to-variant")]
        public IActionResult AddAttributeToVariant(int variantId, int attributeValueId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // Logic to add
            _productFacade.AddAttributeToVariant(variantId, attributeValueId);

            return Json(new { success = true });
        }

        [HttpGet]
        [Route("product/get-discounts")]
        public IActionResult GetDiscountsPartial(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var discounts = _productFacade.GetDiscountsByProductId(productId);

            // Pass ProductId to View for the "Add" form
            ViewBag.ProductId = productId;

            return PartialView("_ProductDiscountsPartial", discounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-discount")]
        public IActionResult AddDiscount(ProductDiscount discount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            discount.CreatedBy = HttpContext.Session.GetString("UserName");
            discount.IsActive = true;

            long result = _productFacade.AddDiscount(discount);

            if (result > 0) return Json(new { success = true });
            return Json(new { success = false, message = "Failed to add discount." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-discount")]
        public IActionResult UpdateDiscount(ProductDiscount discount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            discount.UpdatedBy = HttpContext.Session.GetString("UserName");

            long result = _productFacade.UpdateDiscount(discount);

            if (result > 0) return Json(new { success = true });
            return Json(new { success = false, message = "Failed to update discount." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-discount")]
        public IActionResult DeleteDiscount(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            long result = _productFacade.DeleteDiscount(id);

            if (result > 0) return Json(new { success = true });
            return Json(new { success = false, message = "Failed to delete discount." });
        }

        [HttpGet]
        [Route("product/get-updated-price")]
        public IActionResult GetUpdatedPrice(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var p = _productFacade.GetProductWithPrice(productId);

            if (p == null) return NotFound();

            return Json(new
            {
                success = true,
                hasDiscount = p.ActiveDiscount != null,
                originalPrice = "Tk. " + (p.BasePrice ?? 0).ToString("0.00"),
                sellingPrice = "Tk. " + p.SellingPrice.ToString("0.00")
            });
        }

        [HttpGet]
        [Route("product/get-images")]
        public IActionResult GetImagesPartial(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var images = _productFacade.GetProductImages(productId);
            ViewBag.ProductId = productId;
            return PartialView("_ProductImagesPartial", images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/upload-image")]
        public async Task<IActionResult> UploadImage(int productId, IFormFile file)
        {
            // ... (User checks) ...

            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file received" });

            // 1. Save to Disk
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // ✅ FIX: Include productId in the folder path
            // Was: "wwwroot/images/products"
            // Now: "wwwroot/images/products/{productId}"
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", productId.ToString());

            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 2. Save to Database (This matches the path above now)
            var username = HttpContext.Session.GetString("UserName");
            string dbPath = $"/images/products/{productId}/{fileName}";

            _productFacade.AddProductImage(productId, dbPath, false, username);

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/set-primary-image")]
        public IActionResult SetPrimaryImage(int imageId, int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            _productFacade.SetProductImageAsPrimary(imageId, productId);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-image-order")]
        public IActionResult UpdateImageOrder(int imageId, int sortOrder)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            _productFacade.UpdateProductImageSortOrder(imageId, sortOrder);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-image")]
        public IActionResult DeleteImage(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // 1. Get the image details from DB before deleting
            var img = _productFacade.GetProductImage(id);

            if (img != null && !string.IsNullOrEmpty(img.ImageUrl))
            {
                try
                {
                    // 2. Construct the full physical path
                    // img.ImageUrl looks like: "/images/products/105/abc.jpg"
                    // We remove the leading "/" to combine correctly with WebRootPath
                    string relativePath = img.ImageUrl.TrimStart('/', '\\');
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                    // 3. Delete the physical file
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
                catch (Exception ex)
                {
                    // Log error if needed, but continue to delete DB record
                    // Console.WriteLine("File delete failed: " + ex.Message);
                }
            }

            // 4. Delete from Database
            long result = _productFacade.DeleteProductImage(id);

            if (result > 0) return Json(new { success = true });
            return Json(new { success = false });
        }

        [HttpGet]
        [Route("product/get-variant-images")]
        public IActionResult GetVariantImagesPartial(int variantId)
        {
            var images = _productFacade.GetVariantImages(variantId);
            ViewBag.VariantId = variantId;
            return PartialView("_VariantImagesPartial", images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/upload-variant-image")]
        public async Task<IActionResult> UploadVariantImage(int variantId, IFormFile file)
        {
            // ... (User checks) ...

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // ✅ FIX: Include variantId in the folder path
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "variants", variantId.ToString());

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string dbPath = $"/images/variants/{variantId}/{fileName}";
            var username = HttpContext.Session.GetString("UserName");

            _productFacade.AddVariantImage(variantId, dbPath, username);

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-variant-image")]
        public IActionResult DeleteVariantImage(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // 1. Get the image details
            var img = _productFacade.GetVariantImage(id);

            if (img != null && !string.IsNullOrEmpty(img.ImageUrl))
            {
                try
                {
                    // 2. Construct physical path
                    string relativePath = img.ImageUrl.TrimStart('/', '\\');
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                    // 3. Delete file
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
                catch
                {
                    // Ignore file errors, proceed to DB delete
                }
            }

            // 4. Delete from Database
            _productFacade.DeleteVariantImage(id);

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-variant-image-order")]
        public IActionResult UpdateVariantImageOrder(int imageId, int displayOrder)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            _productFacade.UpdateVariantImageDisplayOrder(imageId, displayOrder);

            return Json(new { success = true });
        }
    }
}
