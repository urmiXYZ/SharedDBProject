using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IUserLoginFacade _userLoginFacade;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IUserLoginFacade userLoginFacade, ILogger<HomeController> logger)
    {
        _userLoginFacade = userLoginFacade;
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

        // This one line gets ALL permission names (like "Product.Add")
        // and puts them in the list the view checks.
        loginResult.AuthorizedActions = _userLoginFacade.GetAllUserPermissionNames(userId.Value);

        // You can now delete these old lines:
        // loginResult.PermissionNames = _userLoginFacade.GetUserPermissionNamesByUserId(userId.Value);
        // loginResult.AuthorizedActions = new List<string>();
        // if (_userLoginFacade.IsUserAuthorized(userId.Value, "AddProduct"))
        //     loginResult.AuthorizedActions.Add("Product.Add");

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
