namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// Class OptionValues
    /// </summary>
    public class OptionValues : IHtmlValue
    {
        public List<string> Values = new ();

        // Constructor
        public OptionValues(List<string>? list = null)
        {
            if (list != null)
                Values = list;
        }

        // Add value
        public void Add(string value) => Values.Add(value);

        // Convert to HTML
        public string ToHtml() => OptionsHtml(Values) ?? ToString();

        // Convert to string (MUST return a string value)
        public override string ToString() => String.Join(Config.OptionSeparator, Values);
    }
} // End Partial class
