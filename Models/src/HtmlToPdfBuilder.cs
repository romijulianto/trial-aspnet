namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {

        #pragma warning disable 612
        /// <summary>
        /// HTML to PDF builder class
        /// </summary>
        public class HtmlToPdfBuilder
        {
            // Properties
            private iTextSharp.text.Rectangle _size = iTextSharp.text.PageSize.A4;

            private StyleSheet _styles = new ();

            private List<float[]> _colWidths = new ();

            public CssParser CssParser = new ();

            private List<string> Pages = new ();

            // Constructor
            public HtmlToPdfBuilder()
            {
            }

            // Set paper size and orientation
            public void SetPaper(string size, string orientation)
            {
                try {
                    _size = iTextSharp.text.PageSize.GetRectangle(size);
                    if (SameText(orientation, "landscape")) // Landscape
                        _size = _size.Rotate();
                } catch {
                    _size = iTextSharp.text.PageSize.GetRectangle(size);
                }
            }

            // Set column widths
            public void AddColumnWidths(float[] widths) => _colWidths.Add(widths);

            // Load HTML
            public void LoadHtml(string html)
            {
                // Get the style block(s)
                string pattern = @"<style type=""text/css"">([\S\s]*)</style>"; // Note: MUST match ExportHeaderAndFooter()
                foreach (Match match in Regex.Matches(html, pattern, RegexOptions.IgnoreCase))
                    AddStyles(match.Groups[1].Value);
                // Remove the style block(s)
                html = Regex.Replace(html, pattern, "");
                string[] StringSeparators = new string[] { Config.PageBreakHtml }; // Note: MUST match ExportPageBreak()
                Pages.AddRange(html.Split(StringSeparators, StringSplitOptions.None));
            }

            // Add styles by CSS string
            public void AddStyles(string content)
            {
                CssParser.Clear();
                CssParser.ParseString(content);
                AddStyles();
            }

            // Add styles to _styles
            private void AddStyles()
            {
                Dictionary<string, Dictionary<string, string>> css = CssParser.Css;
                foreach (var (key, value) in css) {
                    if (key.StartsWith(".") && !key.Contains(" ")) { // Class name
                        _styles.LoadStyle(key.Substring(1), new Hashtable(value));
                    } else if (!key.Contains(".") && !key.Contains(" ")) { // Tag
                        _styles.LoadTagStyle(key, new Hashtable(value));
                    } else {
                        // Not supported by iTextSharp
                    }
                }
            }

            // Renders the PDF to an array of bytes
            public async Task<MemoryStream> RenderPdf()
            {
                var stream = new MemoryStream();
                var document = new iTextSharp.text.Document(_size);
                var writer = PdfWriter.GetInstance(document, stream);

                // Open document
                document.Open();

                // Render each page that has been added
                foreach (string page in Pages) {
                    document.NewPage();

                    // Generate this page of text
                    using var ms = new MemoryStream();
                    using (var sw = new StreamWriter(ms, Encoding.UTF8)) {
                        await sw.WriteAsync(page); // Get the page output
                    }
                    // Read the created stream
                    using var generate = new MemoryStream(ms.ToArray());
                    using var reader = new StreamReader(generate);
                    var providers = new Hashtable();
                    int i = -1;
                    foreach (IElement el in HtmlWorker.ParseToList(reader, _styles, providers)) {
                        if (el is iTextSharp.text.pdf.PdfPTable tbl) {
                            i++;
                            if (i < _colWidths.Count && _colWidths[i].Length == tbl.NumberOfColumns)
                                tbl.SetWidths(_colWidths[i]);
                        }
                        document.Add(el);
                    }
                }

                // Close document
                document.Close();

                // Return the rendered PDF
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }
        #pragma warning restore 612
} // End Partial class
