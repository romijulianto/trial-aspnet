namespace ASPNETMaker2023.Controllers;

// Partial class
public partial class HomeController : Controller
{
    // list
    [Route("userlevelslist/{UserLevelID?}", Name = "userlevelslist-UserLevels-list")]
    [Route("home/userlevelslist/{UserLevelID?}", Name = "userlevelslist-UserLevels-list-2")]
    [AllowAnonymous]
    public async Task<IActionResult> UserLevelsList()
    {
        // Create page object
        userLevelsList = new GLOBALS.UserLevelsList(this);
        userLevelsList.Cache = _cache;

        // Run the page
        return await userLevelsList.Run();
    }

    // add
    [Route("userlevelsadd/{UserLevelID?}", Name = "userlevelsadd-UserLevels-add")]
    [Route("home/userlevelsadd/{UserLevelID?}", Name = "userlevelsadd-UserLevels-add-2")]
    public async Task<IActionResult> UserLevelsAdd()
    {
        // Create page object
        userLevelsAdd = new GLOBALS.UserLevelsAdd(this);

        // Run the page
        return await userLevelsAdd.Run();
    }

    // view
    [Route("userlevelsview/{UserLevelID?}", Name = "userlevelsview-UserLevels-view")]
    [Route("home/userlevelsview/{UserLevelID?}", Name = "userlevelsview-UserLevels-view-2")]
    public async Task<IActionResult> UserLevelsView()
    {
        // Create page object
        userLevelsView = new GLOBALS.UserLevelsView(this);

        // Run the page
        return await userLevelsView.Run();
    }

    // edit
    [Route("userlevelsedit/{UserLevelID?}", Name = "userlevelsedit-UserLevels-edit")]
    [Route("home/userlevelsedit/{UserLevelID?}", Name = "userlevelsedit-UserLevels-edit-2")]
    public async Task<IActionResult> UserLevelsEdit()
    {
        // Create page object
        userLevelsEdit = new GLOBALS.UserLevelsEdit(this);

        // Run the page
        return await userLevelsEdit.Run();
    }

    // delete
    [Route("userlevelsdelete/{UserLevelID?}", Name = "userlevelsdelete-UserLevels-delete")]
    [Route("home/userlevelsdelete/{UserLevelID?}", Name = "userlevelsdelete-UserLevels-delete-2")]
    public async Task<IActionResult> UserLevelsDelete()
    {
        // Create page object
        userLevelsDelete = new GLOBALS.UserLevelsDelete(this);

        // Run the page
        return await userLevelsDelete.Run();
    }
}
