namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// Crosstab Column class
    /// </summary>
    public class CrosstabColumn
    {
        public string Caption = "";

        public object Value;

        public bool Visible = true;

        public CrosstabColumn(object val, string caption, bool visible = true)
        {
            Value = val;
            Caption = caption;
            Visible = visible;
        }
    }
} // End Partial class
