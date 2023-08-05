namespace ASPNETMaker2023.Controllers;

// Partial class
public partial class HomeController : Controller
{
    // list
    [Route("userlevelpermissionslist/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionslist-UserLevelPermissions-list")]
    [Route("home/userlevelpermissionslist/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionslist-UserLevelPermissions-list-2")]
    [AllowAnonymous]
    public async Task<IActionResult> UserLevelPermissionsList()
    {
        // Create page object
        userLevelPermissionsList = new GLOBALS.UserLevelPermissionsList(this);
        userLevelPermissionsList.Cache = _cache;

        // Run the page
        return await userLevelPermissionsList.Run();
    }

    // add
    [Route("userlevelpermissionsadd/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsadd-UserLevelPermissions-add")]
    [Route("home/userlevelpermissionsadd/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsadd-UserLevelPermissions-add-2")]
    public async Task<IActionResult> UserLevelPermissionsAdd()
    {
        // Create page object
        userLevelPermissionsAdd = new GLOBALS.UserLevelPermissionsAdd(this);

        // Run the page
        return await userLevelPermissionsAdd.Run();
    }

    // view
    [Route("userlevelpermissionsview/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsview-UserLevelPermissions-view")]
    [Route("home/userlevelpermissionsview/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsview-UserLevelPermissions-view-2")]
    public async Task<IActionResult> UserLevelPermissionsView()
    {
        // Create page object
        userLevelPermissionsView = new GLOBALS.UserLevelPermissionsView(this);

        // Run the page
        return await userLevelPermissionsView.Run();
    }

    // edit
    [Route("userlevelpermissionsedit/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsedit-UserLevelPermissions-edit")]
    [Route("home/userlevelpermissionsedit/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsedit-UserLevelPermissions-edit-2")]
    public async Task<IActionResult> UserLevelPermissionsEdit()
    {
        // Create page object
        userLevelPermissionsEdit = new GLOBALS.UserLevelPermissionsEdit(this);

        // Run the page
        return await userLevelPermissionsEdit.Run();
    }

    // delete
    [Route("userlevelpermissionsdelete/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsdelete-UserLevelPermissions-delete")]
    [Route("home/userlevelpermissionsdelete/{UserLevelID?}/{_TableName?}", Name = "userlevelpermissionsdelete-UserLevelPermissions-delete-2")]
    public async Task<IActionResult> UserLevelPermissionsDelete()
    {
        // Create page object
        userLevelPermissionsDelete = new GLOBALS.UserLevelPermissionsDelete(this);

        // Run the page
        return await userLevelPermissionsDelete.Run();
    }
}
