using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

public class HomeController : Controller
{
    private readonly IUserLoginFacade _userLoginFacade;
            private readonly IProductFacade _productFacade;

    private readonly ILogger<HomeController> _logger;

    public HomeController(IUserLoginFacade userLoginFacade, IProductFacade productFacade, ILogger<HomeController> logger)
    {
        _userLoginFacade = userLoginFacade;
        _productFacade = productFacade;

        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(null);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return View(new UserLoginResult { IsSuccess = false, ErrorMessage = "Please enter both username and password." });
        }

        var loginResult = _userLoginFacade.GetUserLoginBy(username, password);

        if (loginResult.IsSuccess)
        {
            HttpContext.Session.SetInt32("UserId", loginResult.UserLogin.Id);
            HttpContext.Session.SetString("UserName", loginResult.UserLogin.UserName);
            HttpContext.Session.SetString("Role", loginResult.IsAdmin ? "Admin" : "User");
            HttpContext.Session.SetInt32("CompanyId", loginResult.UserLogin.CompanyId); 
            return RedirectToAction("Dashboard");
        }

        return View(loginResult);
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Index");

        var loginResult = _userLoginFacade.GetUserLoginById(userId.Value);

        loginResult.AuthorizedActions = _userLoginFacade.GetAllUserPermissionNames(userId.Value);

        // Permission
        loginResult.CanViewProducts = loginResult.AuthorizedActions.Contains("Product.View");
        bool canAddProduct = loginResult.AuthorizedActions.Contains("Product.Add");


        // Load product list only if allowed
        if (loginResult.CanViewProducts)
            loginResult.LastFiveProducts = _productFacade.GetLastFiveProducts();
        if (canAddProduct)
        {
            var addProductData = _productFacade.GetAddProductData(userId.Value);
            loginResult.Categories = addProductData.Categories;
            loginResult.Attributes = addProductData.Attributes;
        }
        return View(loginResult);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

}
