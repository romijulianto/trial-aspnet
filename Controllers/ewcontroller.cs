namespace ASPNETMaker2023.Controllers;

// Partial class
[AutoValidateAntiforgeryToken]
[Authorize(Policy = "UserLevel")]
[ApiExplorerSettings(IgnoreApi=true)]
public partial class HomeController : Controller
{
    private IMemoryCache _cache;

    // Constructor
    public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
    {
        _cache = memoryCache;
        GLOBALS.Logger = logger;
    }

    // Destructor
    protected override void Dispose(bool disposing)
    {
        if (disposing) {
            // Clean up temp folder if not add/edit/export
            dynamic page = CurrentPage;
            if (page != null) {
                string pageId = page.PageID;
                if (GetProperty(page, "TableName") != null &&
                    !(new [] { "add", "register", "edit", "update" }).Contains(pageId) &&
                    !(pageId == "list" && page.IsAddOrEdit) &&
                    !(!Empty(GetPropertyValue(page, "Export")) && page.Export != "print" && page.UseCustomTemplate))
                CleanUploadTempPaths(Session.SessionId);
            }
        }
        base.Dispose(disposing);
    }

    // Login
    [Route("login/{provider?}", Name = "login")]
    [Route("home/login/{provider?}", Name = "login-2")]
    [AllowAnonymous]
    public async Task<IActionResult> Login()
    {
        // Create page object
        login = new GLOBALS.Login(this);

        // Run the page
        return await login.Run();
    }

    // Logout
    [Route("logout", Name = "logout")]
    [Route("home/logout", Name = "logout-2")]
    public async Task<IActionResult> Logout()
    {
        // Create page object
        logout = new GLOBALS.Logout(this);

        // Run the page
        return await logout.Run();
    }

    // Index
    [Route("")]
    [Route("index", Name = "index")]
    [Route("home/index", Name = "index-2")]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        // Create page object
        index = new GLOBALS.Index(this);

        // Run the page
        return await index.Run();
    }

    // Error
    [Route("error", Name = "error")]
    [Route("home/error", Name = "error-2")]
    [AllowAnonymous]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Private, NoStore = true, MaxAge = 0)]
    public async Task<IActionResult> Error()
    {
        // Create page object
        error = new GLOBALS.Error(this);

        // Run the page
        return await error.Run();
    }

    // Swagger
    [Route("swagger/swagger", Name = "swagger")]
    [Route("home/swagger/swagger", Name = "swagger-2")]
    [AllowAnonymous]
    public IActionResult Swagger()
    {
        Language = ResolveLanguage();
        ViewData["Title"] = Language.Phrase("ApiTitle");
        ViewData["Version"] = Config.ApiVersion;
        ViewData["BasePath"] = Request.PathBase.ToString();
        return View();
    }

    // Dispose
    // protected override void Dispose(bool disposing) {
    //     try {
    //         base.Dispose(disposing);
    //     } finally {
    //         CurrentPage?.Terminate();
    //     }
    // }
}
