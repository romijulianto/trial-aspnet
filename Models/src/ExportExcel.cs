namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// Class for export to Excel by EPPlus
    /// </summary>
    public class ExportExcel : AbstractExport
    {
        public override string FileExtension { get; set; } = "xlsx";

        private OfficeOpenXml.ExcelPackage _package;

        private OfficeOpenXml.ExcelWorksheet _worksheet;

        private List<int> _imageColumns = new ();

        public int RowCnt; // Note: RowCnt is NOT RowCount.

        public OfficeOpenXml.eOrientation Orientation = OfficeOpenXml.eOrientation.Portrait;

        public OfficeOpenXml.ePaperSize PageSize = OfficeOpenXml.ePaperSize.A4;

        // Package
        public OfficeOpenXml.ExcelPackage Package => _package;

        // Worksheet
        public OfficeOpenXml.ExcelWorksheet Worksheet => _worksheet;

        // Constructor
        public ExportExcel(object tbl) : base(tbl)
        {
            _package = new ();
            Table = tbl;
            _worksheet = _package.Workbook.Worksheets.Add(Table.Name);
            if (!Empty(Table.ExportExcelPageOrientation))
                Orientation = Enum.Parse<OfficeOpenXml.eOrientation>(Table.ExportExcelPageOrientation);
            if (!Empty(Table.ExportExcelPageSize))
                PageSize = Enum.Parse<OfficeOpenXml.ePaperSize>(Table.ExportExcelPageSize);
            _worksheet.PrinterSettings.Orientation = Orientation;
            _worksheet.PrinterSettings.PaperSize = PageSize;
        }

        // Table header
        public override void ExportTableHeader() {}

        // Field aggregate
        public override void ExportAggregate(DbField fld, string type)
        {
            if (!fld.Exportable)
                return;
            FieldCount++;
            if (Horizontal) {
                var val = "";
                if ((new[] {"TOTAL", "COUNT", "AVERAGE"}).Contains(type))
                    val = Language.Phrase(type) + ": " + fld.ExportValue;
                _worksheet.Cells[RowCount, FieldCount].Value = val;
            }
        }

        // Field caption
        public override void ExportCaption(DbField fld)
        {
            if (!fld.Exportable)
                return;
            FieldCount++;
            ExportCaptionBy(fld, FieldCount, RowCount);
        }

        // Field caption by column and row
        public void ExportCaptionBy(DbField fld, int col, int row)
        {
            var val = fld.ExportCaption;
            var cell = _worksheet.Cells[row, col];
            cell.Value = val.Trim();
            cell.Style.Font.Bold = true;
        }

        // Field value by column and row
        public async Task ExportValueBy(DbField fld, int col, int row)
        {
            object? val = null;
            if (fld.ViewTag == "IMAGE") { // Image
                if (!_imageColumns.Contains(col))
                    _imageColumns.Add(col);
                double totalW = 0;
                double maxH = 0;
                int offset = 0;
                var images = await fld.GetTempImage();
                foreach (var (imagefn, i) in images.Select((imagefn, i) => (imagefn, i))) {
                    string fn = imagefn;
                    if (fn.StartsWith("data:")) // Handle base64 image
                        fn = await TempImageFromBase64Url(fn);
                    if (FileExists(fn)) {
                        var imagefile = GetFileInfo(fn);
                        using var imagestream = new FileStream(imagefile.FullName, FileMode.Open, FileAccess.Read);
                        using var image = new MagickImage(imagestream);
                        if ((new[] { MagickFormat.Gif, MagickFormat.Jpg, MagickFormat.Jpeg, MagickFormat.Png, MagickFormat.Bmp }).Contains(image.Format)) {
                            var pic = _worksheet.Drawings.AddPicture(fld.Name + row.ToString() + i.ToString(), imagefile);
                            double width = image.Width * Config.ExcelColumnWidthFactor;
                            double height = image.Height * Config.ExcelRowHeightFactor;
                            pic.SetPosition(row - 1, 0, col - 1, offset);
                            if (width > 0) { // Width
                                offset += image.Width;
                                totalW += width;
                            }
                            maxH = Math.Max(maxH, height); // Height
                        }
                    }
                }
                _worksheet.Column(col).Width = Math.Max(_worksheet.Column(col).Width, totalW);
                _worksheet.Row(row).Height = Math.Max(_worksheet.Row(row).Height, maxH);
            } else if (fld.ExportFieldImage && fld.ExportHrefValue is Func<string, Task<string>> func) { // Export Custom View Tag // DN
                var imagefn = await func("excel");
                string fn = imagefn;
                if (fn.StartsWith("data:")) // Handle base64 image
                    fn = await TempImageFromBase64Url(fn);
                if (FileExists(fn)) {
                    var pic = _worksheet.Drawings.AddPicture(fld.Name + row.ToString(), GetFileInfo(fn));
                    pic.SetPosition(row - 1, 0, col - 1, 0);
                    _worksheet.Column(col).Width = Math.Max(_worksheet.Column(col).Width, pic.Image.Width * Config.ExcelColumnWidthFactor);
                    _worksheet.Row(row).Height = Math.Max(_worksheet.Row(row).Height, pic.Image.Height * Config.ExcelRowHeightFactor);
                }
            } else { // Formatted Text
                val = fld.ExportValue;
                if (Config.RemoveXss)
                    val = HtmlDecode(val);
                if (RowCnt > 0) { // Not table header/footer
                    if ((new[] {4, 5, 6, 14, 131}).Contains(fld.Type) && fld.Lookup == null) // If float or currency
                        val = fld.CurrentValue; // Use original value instead of formatted value
                }
                _worksheet.Cells[row, col].Value = val;
            }
        }

        // Begin a row
        public override void BeginExportRow(int rowCnt = 0)
        {
            RowCount++;
            FieldCount = 0;
            RowCnt = rowCnt;
        }

        // End a row
        public override void EndExportRow(int rowCnt = 0) {}

        // Empty row
        public override void ExportEmptyRow() => RowCount++;

        // Page break
        public override void ExportPageBreak() {}

        // Export a field
        public override async Task ExportField(DbField fld)
        {
            if (!fld.Exportable)
                return;
            FieldCount++;
            if (Horizontal) {
                await ExportValueBy(fld, FieldCount, RowCount);
            } else { // Vertical, export as a row
                RowCount++;
                ExportCaptionBy(fld, 1, RowCount);
                await ExportValueBy(fld, 2, RowCount);
            }
        }

        // Table footer
        public override void ExportTableFooter() {}

        #pragma warning disable 1998
        // Page header and footer
        public override async Task ExportHeaderAndFooter()
        {
            // Auto fit
            if (Horizontal) {
                for (int i = 1; i <= FieldCount; i++) {
                    if (!_imageColumns.Contains(i)) // Not image column
                        _worksheet.Column(i).AutoFit();
                }
            } else {
                _worksheet.Column(1).AutoFit();
            }
        }
        #pragma warning restore 1998

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="imageFile">Image file name</param>
        /// <param name="breakType">Page break type (before / after)</param>
        public override void AddImage(string imageFile, string breakType = "")
        {
            if (FileExists(imageFile)) {
                RowCount++;
                var pic = _worksheet.Drawings.AddPicture(Path.GetFileName(imageFile) + RowCount.ToString(), GetFileInfo(imageFile));
                int row = RowCount;
                int col = 1;
                pic.SetPosition(row - 1, 0, col - 1, 0);
                _worksheet.Column(col).Width = Math.Max(_worksheet.Column(col).Width, pic.Image.Width * Config.ExcelColumnWidthFactor);
                _worksheet.Row(row).Height = Math.Max(_worksheet.Row(row).Height, pic.Image.Height * Config.ExcelRowHeightFactor);
            }
        }

        // Load html
        public override void LoadHtml(string html) => LoadHtmlAsync(html).GetAwaiter().GetResult();

        // Load html async
        public async Task LoadHtmlAsync(string html)
        {
            var parser = new AngleSharp.Html.Parser.HtmlParser();
            AngleSharp.Html.Dom.IHtmlDocument doc = await parser.ParseDocumentAsync(html, default(CancellationToken));
            var tables = doc.QuerySelectorAll(Selectors);
            if (!Empty(Table?.ExportExcelPageOrientation))
                _worksheet.PrinterSettings.Orientation = Enum.Parse<OfficeOpenXml.eOrientation>(Table?.ExportExcelPageOrientation!);
            if (!Empty(Table?.ExportExcelPageSize))
                _worksheet.PrinterSettings.PaperSize = Enum.Parse<OfficeOpenXml.ePaperSize>(Table?.ExportExcelPageSize!);
            int r = 1;
            foreach (var table in tables) {
                var classes = (table.GetAttribute("class") ?? "").Split(' ');
                bool isChart = classes.Contains("ew-chart"), isTable = classes.Contains("ew-table");
                if (isTable) {
                    var rows = table.GetElementsByTagName("tr");
                    foreach (var row in rows) {
                        var cells = row.ChildNodes;
                        int c = 1;
                        foreach (var cell in cells) {
                            if (cell.NodeType != AngleSharp.Dom.NodeType.Element || !SameText(cell.NodeName, "TD") && !SameText(cell.NodeName, "TH"))
                                continue;
                            var td = (AngleSharp.Dom.IElement)cell;
                            var imgs = td.GetElementsByTagName("img");
                            if (imgs.Length > 0) { // Images
                                double totalW = 0, maxH = 0;
                                int offset = 0;
                                for (var i = 0; i < imgs.Length; i++) {
                                    string fn = imgs[i].GetAttribute("src") ?? "";
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
                                    var imagefile = GetFileInfo(fn);
                                    using var imagestream = new FileStream(imagefile.FullName, FileMode.Open, FileAccess.Read);
                                    using var image = new MagickImage(imagestream);
                                    if ((new[] { MagickFormat.Gif, MagickFormat.Jpg, MagickFormat.Jpeg, MagickFormat.Png, MagickFormat.Bmp }).Contains(image.Format)) {
                                        var pic = _worksheet.Drawings.AddPicture("image_" + r.ToString() + i.ToString() + "_" + c.ToString(), imagefile);
                                        double width = image.Width * Config.ExcelColumnWidthFactor;
                                        double height = image.Height * Config.ExcelRowHeightFactor;
                                        pic.SetPosition(r - 1, 0, c - 1, offset);
                                        if (width > 0) { // Width
                                            offset += image.Width;
                                            totalW += width;
                                        }
                                        maxH = Math.Max(maxH, height); // Height
                                    }
                                }
                                _worksheet.Column(c).Width = Math.Max(_worksheet.Column(c).Width, totalW);
                                _worksheet.Row(r).Height = Math.Max(_worksheet.Row(r).Height, maxH);
                            } else { // Text
                                _worksheet.Cells[r, c].Value = td.TextContent.Trim();
                            }
                            if (td.HasAttribute("colspan")) {
                                c += ConvertToInt(td.GetAttribute("colspan"));
                            } else {
                                c++;
                            }
                        }
                        r++;
                    }
                    r++;
                } else { // div
                    var imgs = table.QuerySelectorAll("img");
                    if (imgs.Length > 0) { // Images
                        int c = 1;
                        double totalW = 0, maxH = 0;
                        int offset = 0;
                        for (var i = 0; i < imgs.Length; i++)
                        {
                            string fn = imgs[i].GetAttribute("src") ?? "";
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
                            var imagefile = GetFileInfo(fn);
                            using var imagestream = new FileStream(imagefile.FullName, FileMode.Open, FileAccess.Read);
                            using var image = new MagickImage(imagestream);
                            if ((new[] { MagickFormat.Gif, MagickFormat.Jpg, MagickFormat.Jpeg, MagickFormat.Png, MagickFormat.Bmp }).Contains(image.Format)) {
                                var pic = _worksheet.Drawings.AddPicture("image_" + r.ToString() + i.ToString() + "_" + c.ToString(), imagefile);
                                double width = image.Width * Config.ExcelColumnWidthFactor;
                                double height = image.Height * Config.ExcelRowHeightFactor;
                                pic.SetPosition(r - 1, 0, c - 1, offset);
                                if (width > 0) { // Width
                                    offset += image.Width;
                                    totalW += width;
                                }
                                maxH = Math.Max(maxH, height); // Height
                            }
                        }
                        _worksheet.Column(c).Width = Math.Max(_worksheet.Column(c).Width, totalW);
                        _worksheet.Row(r).Height = Math.Max(_worksheet.Row(r).Height, maxH);
                        r++;
                    }
                }
            }
        }

        // Export
        public override async Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false)
        {
            try {
                var ms = new MemoryStream();
                _package.SaveAs(ms);
                ms.Seek(0, SeekOrigin.Begin);
                byte[] data = ms.ToArray(); // Use byte array
                //string text = new StreamReader(ms).ReadToEnd();
                //Text.Clear();
                //Text.Append(text);

                // Save to folder
                if (save)
                    await SaveFile(ExportPath(true), GetSaveFileName(), data);

                // Output
                if (output) {
                    WriteHeaders(fileName);
                    return Controller.File(data, GetContentType().ToString());
                }
            } finally {
                _package.Dispose();
            }
            return new EmptyResult();
        }
    }
} // End Partial class
