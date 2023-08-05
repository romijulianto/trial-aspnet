namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// Export to PDF class (to be replaced by extension)
    /// </summary>
    public class ExportPdf : AbstractExport
    {
        // Constructor
        public ExportPdf(object tbl) : base(tbl) {}

        #pragma warning disable 1998
        // Export
        public override async Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false)
        {
            throw new Exception("Export PDF extension disabled");
        }
        #pragma warning restore 1998
    }
} // End Partial class
