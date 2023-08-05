namespace ASPNETMaker2023.Controllers;

// Partial class
public partial class HomeController : Controller
{
    // list
    [Route("mttasklist/{id?}", Name = "mttasklist-mt_task-list")]
    [Route("home/mttasklist/{id?}", Name = "mttasklist-mt_task-list-2")]
    [AllowAnonymous]
    public async Task<IActionResult> MtTaskList()
    {
        // Create page object
        mtTaskList = new GLOBALS.MtTaskList(this);
        mtTaskList.Cache = _cache;

        // Run the page
        return await mtTaskList.Run();
    }

    // add
    [Route("mttaskadd/{id?}", Name = "mttaskadd-mt_task-add")]
    [Route("home/mttaskadd/{id?}", Name = "mttaskadd-mt_task-add-2")]
    [AllowAnonymous]
    public async Task<IActionResult> MtTaskAdd()
    {
        // Create page object
        mtTaskAdd = new GLOBALS.MtTaskAdd(this);

        // Run the page
        return await mtTaskAdd.Run();
    }

    // view
    [Route("mttaskview/{id?}", Name = "mttaskview-mt_task-view")]
    [Route("home/mttaskview/{id?}", Name = "mttaskview-mt_task-view-2")]
    [AllowAnonymous]
    public async Task<IActionResult> MtTaskView()
    {
        // Create page object
        mtTaskView = new GLOBALS.MtTaskView(this);

        // Run the page
        return await mtTaskView.Run();
    }

    // edit
    [Route("mttaskedit/{id?}", Name = "mttaskedit-mt_task-edit")]
    [Route("home/mttaskedit/{id?}", Name = "mttaskedit-mt_task-edit-2")]
    public async Task<IActionResult> MtTaskEdit()
    {
        // Create page object
        mtTaskEdit = new GLOBALS.MtTaskEdit(this);

        // Run the page
        return await mtTaskEdit.Run();
    }

    // delete
    [Route("mttaskdelete/{id?}", Name = "mttaskdelete-mt_task-delete")]
    [Route("home/mttaskdelete/{id?}", Name = "mttaskdelete-mt_task-delete-2")]
    [AllowAnonymous]
    public async Task<IActionResult> MtTaskDelete()
    {
        // Create page object
        mtTaskDelete = new GLOBALS.MtTaskDelete(this);

        // Run the page
        return await mtTaskDelete.Run();
    }
}
