namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// User level table permission class
    /// </summary>
    public class UserLevelTablePermission : ICloneable
    {
        // Table name
        public string TableName { set; get; } = "";

        // Table var name
        public string TableVar { set; get; } = "";

        // Caption
        public string Caption { set; get; } = "";

        // Project ID
        public string ProjectId { set; get; } = "";

        // Permission
        public int Permission { set; get; } = 0;

        // Allowed
        public bool Allowed { set; get; }

        // URL
        public string Url { set; get; } = "";

        // Clone
        public object Clone() => this.MemberwiseClone();
    }
} // End Partial class
