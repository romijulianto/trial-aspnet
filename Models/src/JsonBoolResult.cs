namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// JsonResult with a boolean Result property
    /// </summary>
    public class JsonBoolResult : JsonResult
    {
        public bool Result;

        public static JsonBoolResult FalseResult = new (new { success = false, version = Config.ProductVersion }, false);

        // Constructor
        public JsonBoolResult(object value, bool result) : base(value) => Result = result;

        // Implicit operator
        public static implicit operator bool(JsonBoolResult me) => me.Result;
    }
} // End Partial class
