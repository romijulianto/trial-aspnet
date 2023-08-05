namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// Class for export to Word by Html2OpenXml
    /// </summary>
    public class ExportWord : AbstractExport
    {
        public override string FileExtension { get; set; } = "docx";

        public string Orientation = "Portrait";

        // Constructor
        public ExportWord(object tbl) : base(tbl)
        {
            Table = tbl;
            if (!Empty(Table.ExportWordPageOrientation))
                Orientation = Table.ExportWordPageOrientation;
        }

        // Caption styles
        public static string CaptionCssStyle = "font-weight: bold;";

        // Begin a row
        public override void BeginExportRow(int rowCnt = 0)
        {
            RowCount++;
            FieldCount = 0;
            if (Horizontal) {
                string className = rowCnt switch {
                    -1 => "ew-export-table-footer",
                    0 => "ew-export-table-header",
                    _ => (rowCnt % 2 == 1) ? "ew-export-table-row" : "ew-export-table-alt-row"
                };
                string style = rowCnt switch {
                    0 => CaptionCssStyle, // Header
                    _ => ""
                };
                Text.Append("<tr" + (ExportStyles ? " class=\"" + className + "\" style=\"" + style + "\"" : "") + ">");
            }
        }

        // Export a field
        public override async Task ExportField(DbField fld)
        {
            if (!fld.Exportable)
                return;
            FieldCount++;
            string value = fld.ExportValue;
            if (fld.ExportFieldImage && fld.ViewTag == "IMAGE") {
                if (fld.ImageResize) {
                    value = GetFileImgTag(await fld.GetTempImage());
                } else if (!Empty(fld.ExportHrefValue) && fld.Upload != null) {
                    if (!Empty(fld.Upload.DbValue))
                        value = GetFileATag(fld, fld.ExportHrefValue);
                }
            } else if (fld.ExportFieldImage && fld.ExportHrefValue is Func<string, Task<string>> func) { // Export Custom View Tag // DN
                value = await func("word");
                if (FileExists(value))
                    value = GetFileImgTag(new List<string> { value });
            }
            if (Horizontal) {
                ExportValue(fld, value);
            } else { // Vertical, export as a row
                RowCount++;
                Text.Append("<tr class=\"" + ((FieldCount % 2 == 1) ? "ew-export-table-row" : "ew-export-table-alt-row") + "\">" +
                    "<td" + CellStyles(fld) + " style=\"" + CaptionCssStyle + "\">" + fld.ExportCaption + "</td>");
                Text.Append("<td" + CellStyles(fld) + ">" + value + "</td></tr>");
            }
        }

        #pragma warning disable 1998
        // Add HTML tags
        public override async Task ExportHeaderAndFooter()
        {
            Text.Insert(0, "<html><body" + (Orientation != "" ? " style=\"page-orientation: " + Orientation.ToLower() + "\"" : "") + ">");
            Text.Append("</body></html>");
        }
        #pragma warning restore 1998

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="imageFile">Image file name</param>
        /// <param name="breakType">Page break type (before / after)</param>
        public override void AddImage(string imageFile, string breakType = "")
        {
            string classes = "ew-export";
            if (SameText(breakType, "before"))
                classes += " break-before-page";
            else if (SameText(breakType, "after"))
                classes += " break-after-page";
            string html = "<div class=\"" + classes + "\">" + GetFileImgTag(new List<string> { imageFile }) + "</div>";
            if (Text.ToString().Contains("</body>"))
                Text.Replace("</body>", html + "</body>"); // Insert before </body>
            else
                Text.Append(html); // Append to end
        }

        // Export
        public override async Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false)
        {
            try {
                byte[] data;
                using (MemoryStream generatedDocument = new()) {
                    using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document)) {
                        MainDocumentPart mainPart = package.MainDocumentPart ?? package.AddMainDocumentPart();
                        new DocumentFormat.OpenXml.Wordprocessing.Document(new Body()).Save(mainPart);
                        HtmlConverter converter = new HtmlConverter(mainPart);
                        converter.ParseHtml(ToString());
                        mainPart.Document.Save();
                    }
                    data = generatedDocument.ToArray();
                }

                // Save to folder
                if (save)
                    await SaveFile(ExportPath(true), GetSaveFileName(), data);

                // Output
                if (output) {
                    WriteHeaders(fileName);
                    return Controller.File(data, GetContentType().ToString());
                }
            } catch {
                if (Config.Debug)
                    throw;
            }
            return new EmptyResult();
        }
    }
} // End Partial class
