namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    // Configuration
    public static partial class Config
    {
        //
        // ASP.NET Maker 2023 user level settings
        //

        // User level info
        public static List<UserLevel> UserLevels = new ()
        {
            new () { Id = -2, Name = "Anonymous" }
        };

        // User level priv info
        public static List<UserLevelPermission> UserLevelPermissions = new ()
        {
            new () { Table = "{86DBF2C8-AAE1-4B5D-9407-46680224C91D}mt_task", Id = -2, Permission = 2027 },
            new () { Table = "{86DBF2C8-AAE1-4B5D-9407-46680224C91D}UserLevels", Id = -2, Permission = 72 },
            new () { Table = "{86DBF2C8-AAE1-4B5D-9407-46680224C91D}UserLevelPermissions", Id = -2, Permission = 72 }
        };

        // User level table info // DN
        public static List<UserLevelTablePermission> UserLevelTablePermissions = new ()
        {
            new () { TableName = "mt_task", TableVar = "mt_task", Caption = "mt task", Allowed = true, ProjectId = "{86DBF2C8-AAE1-4B5D-9407-46680224C91D}", Url = "mttasklist" },
            new () { TableName = "UserLevels", TableVar = "UserLevels", Caption = "User Levels", Allowed = true, ProjectId = "{86DBF2C8-AAE1-4B5D-9407-46680224C91D}", Url = "userlevelslist" },
            new () { TableName = "UserLevelPermissions", TableVar = "UserLevelPermissions", Caption = "User Level Permissions", Allowed = true, ProjectId = "{86DBF2C8-AAE1-4B5D-9407-46680224C91D}", Url = "userlevelpermissionslist" }
        };
    }
} // End Partial class
