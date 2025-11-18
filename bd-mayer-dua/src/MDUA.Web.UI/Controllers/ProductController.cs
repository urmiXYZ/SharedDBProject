using MDUA.Entities; // Using the enhanced Product entity
using MDUA.Entities.Bases;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    // The controller is now a thin layer, strictly calling the Facade.
    public class ProductController : Controller
    {
        private readonly IProductFacade _productFacade;
        private readonly IUserLoginFacade _userLoginFacade;


        // Dependency Injection
        public ProductController(IProductFacade productFacade, IUserLoginFacade userLoginFacade)
        {
            _productFacade = productFacade;
            _userLoginFacade = userLoginFacade;

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

            long newProductId = _productFacade.AddProduct(
                product,
                HttpContext.Session.GetString("UserName"),
                HttpContext.Session.GetInt32("CompanyId") ?? 0
            );

            TempData[newProductId > 0 ? "Success" : "Error"] =
                newProductId > 0 ? "Product added successfully!" : "Failed to add product.";

            return RedirectToAction("Dashboard", "Home");
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

            // Make sure user is authorized to delete
            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Delete")) // Assuming "Product.Delete"
                return Forbid();

            try
            {
                // Your facade's Delete method will call the DA layer's Delete,
                // which will trigger the 'ON DELETE CASCADE' in the database.
                long result = _productFacade.Delete(productId);

                if (result > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Product not found or could not be deleted." });
                }
            }
            catch (Exception ex)
            {
                // This will catch errors, e.g., if a cascade delete is missing
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
        public IActionResult UpdateVariantPrice(int variantId, decimal newPrice)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (!_userLoginFacade.IsUserAuthorized(userId.Value, "Product.Edit"))
                return Forbid();

            // Call the updated facade method
            long result = _productFacade.UpdateVariantPrice(variantId, newPrice);

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

            // 1. Get the variants (Your Model)
            var variants = _productFacade.GetVariantsByProductId(productId);

            // 2. Get the attributes specifically for this product
            ViewBag.ProductAttributes = _productFacade.GetAttributesForProduct(productId);

            // 3. Store ProductId
            ViewBag.ProductId = productId;

            // ✅ 4. FIX: Fetch the Product to get the Base Price
            var product = _productFacade.Get(productId); // Uses your simple Get() method
            ViewBag.BasePrice = product?.BasePrice ?? 0;

            return PartialView("_ProductVariantsPartial", variants);
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

    }
}
