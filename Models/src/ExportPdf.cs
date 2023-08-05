namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// Export to PDF class
    /// </summary>
    public class ExportPdf : AbstractExport
    {
        public override string FileExtension { get; set; } = "pdf";

        public HtmlToPdfBuilder PdfBuilder = new ();

        private string _tableHeader = "<table class=\"ew-table\">";

        // Constructor
        public ExportPdf(object tbl) : base(tbl)
        {
            if (Table != null) {
                PdfBuilder.SetPaper(Table.ExportPageSize, Table.ExportPageOrientation);
                if (Table.ExportColumnWidths.Length > 0)
                    PdfBuilder.AddColumnWidths(Table.ExportColumnWidths);
            }
        }

        // Table header
        public override void ExportTableHeader() => Text.AppendLine(_tableHeader);

        // Export a value (caption, field value, or aggregate)
        public override void ExportValue(DbField fld, object val)
        {
            Text.AppendLine("<td" + (ExportStyles ? fld.CellStyles : "") + ">" + ConvertToString(val) + "</td>");
        }

        // Begin a row
        public override void BeginExportRow(int rowCnt = 0)
        {
            FieldCount = 0;
            if (Horizontal) {
                string className = rowCnt switch {
                    -1 => "ew-table-footer",
                    0 => "ew-table-header",
                    _ => (rowCnt % 2) == 1 ? "ew-table-row" : "ew-table-alt-row"
                };
                Text.Append("<tr" + (ExportStyles ? " class=\"" + className + "\"" : "") + ">");
            }
        }

        // End a row
        public override void EndExportRow(int rowCnt = 0)
        {
            if (Horizontal)
                Text.Append("</tr>");
        }

        // Page break
        public override void ExportPageBreak()
        {
            if (Horizontal) {
                Text.AppendLine("</table>"); // End current table
                Text.AppendLine(Config.PageBreakHtml); // Page break
                Text.AppendLine(_tableHeader); // New page header
            }
        }

        // Export a field
        public override async Task ExportField(DbField fld)
        {
            if (!fld.Exportable)
                return;
            string value = fld.ExportValue;
            if (fld.ExportFieldImage && fld.ViewTag == "IMAGE") {
                value = GetFileImgTag(await fld.GetTempImage());
            } else if (fld.ExportFieldImage && fld.ExportHrefValue is Func<string, Task<string>> func) { // Export Custom View Tag // DN
                value = await func("pdf"); // DN
            } else {
                value = value.Replace("<br>", "\r\n");
                value = RemoveHtml(value);
                value = value.Replace("\r\n", "<br>");
            }
            if (Horizontal) {
                ExportValue(fld, value);
            } else { // Vertical, export as a row
                FieldCount++;
                fld.CellCssClass = (FieldCount % 2 == 1) ? "ew-table-row" : "ew-table-alt-row";
                Text.Append("<tr><td" + (ExportStyles ? fld.CellStyles : "") + ">" + fld.ExportCaption + "</td>");
                Text.Append("<td" + (ExportStyles ? fld.CellStyles : "") + ">" + value + "</td></tr>");
            }
        }

        // Get style tag
        public async Task<string> StyleTag()
        {
            if (ExportStyles && !Empty(Config.PdfStylesheetFilename)) {
                string content = await LoadText(Config.PdfStylesheetFilename);
                if (!Empty(content))
                    return "<style type=\"text/css\">" + content + "</style>\r\n";
            }
            return "";
        }

        // Add HTML tags
        public override async Task ExportHeaderAndFooter()
        {
            string header = "<html><head>\r\n";
            header += CharsetMetaTag();
            header += await StyleTag();
            header += "</head><body>";
            Text.Insert(0, header);
            Text.Append("</body></html>");
        }

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="imageFile">Image file name</param>
        /// <param name="breakType">Page break type (before / after)</param>
        public override void AddImage(string imageFile, string breakType = "")
        {
            if (FileExists(imageFile)) {
                string html = "<table class=\"ew-chart\"><tr><td>" + GetFileImgTag(new List<string> { imageFile })  + "</td></tr></table>";
                if (Text.ToString().Contains("</body>"))
                    Text.Replace("</body>", html + "</body>"); // Insert before </body>
                else
                    Text.Append(html); // Append to end
            }
        }

        // Adjust HTML before export
        protected override async Task AdjustHtml()
        {
            var doc = GetDocument(); // Load Text

            // Remove empty charts / Replace div.ew-chart with table.ew-chart
            foreach (var div in doc.QuerySelectorAll("div.ew-chart")) {
                var parent = div.ParentElement;
                var script = parent?.QuerySelector("script");
                if (script != null) // Remove script for chart
                    script.Remove();
                var img = div.QuerySelector("img");
                if (img == null) { // No image inside => Remove
                    div.Remove();
                } else { // Replace div.ew-chart with table.ew-chart
                    var tbl = new AngleSharp.Html.Parser.HtmlParser().ParseDocument("<table class=\"ew-chart\"><tr><td>" + img.OuterHtml + "</td></tr></table>");
                    div.Replace(tbl.QuerySelector("table"));
                }
            }

            // Remove empty cards
            foreach (var div in doc.QuerySelectorAll("div.card")) {
                var selector = div.QuerySelector(Selectors);
                if (selector == null) // Nothing to export => Remove
                    div.Remove();
            }

            // Adjust table class and images
            var tables = doc.QuerySelectorAll(Selectors);
            foreach (var table in tables) {
                string classNames = table.GetAttribute("class") ?? "";
                if (ContainsClass(classNames, "no-border")) // Remove border // DN
                    table.SetAttribute("border", "0");
                if (ContainsClass(classNames, "ew-table"))
                    table.SetAttribute("class", "ew-table"); // Remove other classes
                table.RemoveAttribute("style"); // Remove any style
                var imgs = table.QuerySelectorAll("img");
                foreach (var img in imgs) { // Images
                    string fn = img.GetAttribute("src") ?? "";
                    byte[] data;
                    if (fn.StartsWith("data:")) { // Handle base64 image
                        fn = await TempImageFromBase64Url(fn);
                    } else {
                        if (File.Exists(fn)) {
                            data = await FileReadAllBytes(fn);
                        } else {
                            fn = FullUrl(fn);
                            data = await DownloadDataAsync(fn);
                        }
                        fn = await TempImage(data);
                    }
                    if (!FileExists(fn))
                        continue;
                    img.SetAttribute("src", fn);
                }
            }

            // Add page break
            string html = "";
            if (Table?.ExportPageBreaks ?? ExportPageBreaks) {
                var els = doc.QuerySelectorAll(Selectors);
                int i = 0;
                foreach (var el in els) {
                    if (i > 0)
                        html += Config.PageBreakHtml;
                    html += el.OuterHtml;
                    el.Remove();
                    i++;
                }
                //html += doc.QuerySelector("body")?.InnerHtml ?? "";
            } else {
                var body = doc.QuerySelector("body");
                html = body == null ? doc.DocumentElement.OuterHtml : body.InnerHtml;
            }
            Text.Clear();
            Text.Append(html);

            // Add CSS for PDF
            await ExportHeaderAndFooter();
        }

        // Export
        public override async Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false)
        {
            await AdjustHtml();
            try {
                PdfBuilder.LoadHtml(Text.ToString());
                byte[] data;
                using (MemoryStream ms = PdfBuilder.RenderPdf().GetAwaiter().GetResult()) {
                    ms.Seek(0, SeekOrigin.Begin);
                    data = ms.ToArray(); // Use byte array
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
