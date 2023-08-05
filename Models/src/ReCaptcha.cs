namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// ReCaptcha class
    /// </summary>
    public class ReCaptcha : CaptchaBase
    {
        public string Language = CurrentLanguage;

        public static string PublicKey = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";

        public static string PrivateKey = "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe";

        public static string Theme = "light";

        public static string Type = "image";

        public static string Size = "normal";

        public ReCaptcha() => ResponseField = "g-recaptcha-response";

        // HTML tag
        public override string GetHtml()
        {
            string classAttr = String.Empty;
            if (CurrentPage != null)
                classAttr = " class=\"" + CurrentPage.OffsetColumnClass + "\"";
            return $@"<div class=""row ew-recaptcha-{Size}"">
<div{classAttr}>
    <div id=""{ElementId}"" class=""g-recaptcha""></div>
    <input type=""hidden"" name=""{ElementId}"" class=""form-control ew-recaptcha"" disabled>
    <div class=""invalid-feedback""></div>
</div>
</div>";
        }

        // HTML tag for confirm page
        public override string GetConfirmHtml() => $@"<input type=""hidden"" id=""{ElementId}"" name=""{ElementName}"" value=""{Response}"">";

        // Validate
        public override bool Validate() => ValidateAsync().GetAwaiter().GetResult();

        // Validate async
        protected async Task<bool> ValidateAsync() {
            if (SameString(Response, Session[SessionName]))
                return true;
            string reply = await DownloadStringAsync("https://www.google.com/recaptcha/api/siteverify?secret=" + PrivateKey + "&response=" + Response);
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(reply);
            if (result is Dictionary<string, object>) {
                if (result.TryGetValue("success", out object? success) && ConvertToBool(success)) {
                    Session[SessionName] = Response;
                    return true;
                } else {
                    if (result.TryGetValue("error-codes", out object? errorCodes) && errorCodes is JArray errors)
                        CurrentPage?.SetFailureMessage(errors[0].ToString());
                }
            }
            return false;
        }

        // Client side validation script
        public override string GetScript()
        {
            string validate = !Empty(FailureMessage) ? "true" : "false";
            return $@".addField(""{ElementName}"", ew.Validators.captcha, {validate})";
        }
    }
}
