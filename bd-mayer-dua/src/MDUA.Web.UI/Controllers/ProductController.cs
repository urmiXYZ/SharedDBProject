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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(ProductBase product)
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


            if (newProductId > 0)
                TempData["Success"] = "Product added successfully!";
            else
                TempData["Error"] = "Failed to add product.";

            return RedirectToAction("Dashboard", "Home");
        }



    }
}
