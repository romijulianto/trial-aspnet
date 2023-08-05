// Models
namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    // Configuration
    public static partial class Config {

        public static Dictionary<string, dynamic?> FileManager = new () {
            { "options", new Dictionary<string, dynamic> {
                {"theme", "flat-dark"},
                {"showTitleAttr", false},
                {"showConfirmation", true},
                {"browseOnly", false},
                {"fileSorting", "NAME_ASC"},
                {"folderPosition", "bottom"},
                {"quickSelect", false},
                {"logger", false},
                {"allowFolderDownload", true},
                {"allowChangeExtensions", false},
                {"capabilities", new List<string> {
                    "select",
                    "upload",
                    "download",
                    "rename",
                    "copy",
                    "move",
                    "delete",
                    "extract",
                    "createFolder"
                }}
            }},
            {"language", new Dictionary<string, dynamic> {
                {"default", "en"},
                {"available", new List<string> {"ar", "bs", "ca", "cs", "da", "de", "el", "en", "es", "fa", "fi", "fr", "he", "hu", "it", "ja", "nl", "pl", "pt", "ru", "sv", "th", "tr", "vi", "zh-CN", "zh-TW"}}
            }},
            {"formatter", new Dictionary<string, dynamic> {
                {"datetime", new Dictionary<string, dynamic> {
                    {"skeleton", "yMMMdHm"}
                }}
            }},
            {"filetree", new Dictionary<string, dynamic> {
                {"enabled", true},
                {"foldersOnly", false},
                {"reloadOnClick", true},
                {"expandSpeed", 200},
                {"showLine", true},
                {"width", 200},
                {"minWidth", 200}
            }},
            {"manager", new Dictionary<string, dynamic> {
                {"defaultViewMode", "grid"},
                {"dblClickOpen", false},
                {"selection", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"useCtrlKey", true}
                }},
                {"renderer", new Dictionary<string, dynamic> {
                    {"position", false},
                    {"indexFile", "readme.md"}
                }}
            }},
            {"api", new Dictionary<string, dynamic> {
                {"lang", "cs"},
                {"connectorUrl", false},
                {"requestParams", new Dictionary<string, dynamic> {
                    {"GET", new Dictionary<string, dynamic> {}},
                    {"POST", new Dictionary<string, dynamic> {}},
                    {"MIXED", new Dictionary<string, dynamic> {}}
                }}
            }},
            {"upload", new Dictionary<string, dynamic> {
                {"multiple", true},
                {"maxNumberOfFiles", 5},
                {"chunkSize", false}
            }},
            {"clipboard", new Dictionary<string, dynamic> {
                {"enabled", true},
                {"encodeCopyUrl", true}
            }},
            {"filter", new Dictionary<string, dynamic> {
                {"image", new List<string> {"jpg", "jpeg", "gif", "png", "svg"}},
                {"media", new List<string> {"ogv", "avi", "mkv", "mp4", "webm", "m4v", "ogg", "mp3", "wav"}},
                {"office", new List<string> {"txt", "pdf", "odp", "ods", "odt", "rtf", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "csv", "md"}},
                {"archive", new List<string> {"zip", "tar", "rar"}},
                {"audio", new List<string> {"ogg", "mp3", "wav"}},
                {"video", new List<string> {"ogv", "avi", "mkv", "mp4", "webm", "m4v"}}
            }},
            {"search", new Dictionary<string, dynamic> {
                {"enabled", true},
                {"recursive", false},
                {"caseSensitive", false},
                {"typingDelay", 500}
            }},
            {"viewer", new Dictionary<string, dynamic> {
                {"absolutePath", true},
                {"previewUrl", false},
                {"image", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"lazyLoad", true},
                    {"showThumbs", true},
                    {"thumbMaxWidth", 64},
                    {"extensions", new List<string> {
                        "jpg",
                        "jpe",
                        "jpeg",
                        "gif",
                        "png",
                        "svg"
                    }}
                }},
                {"video", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"extensions", new List<string> {
                        "ogv",
                        "mp4",
                        "webm",
                        "m4v"
                    }},
                    {"playerWidth", 400},
                    {"playerHeight", 222}
                }},
                {"audio", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"extensions", new List<string> {
                        "ogg",
                        "mp3",
                        "wav"
                    }}
                }},
                {"iframe", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"extensions", new List<string> {
                        "htm",
                        "html"
                    }},
                    {"readerWidth", "95%"},
                    {"readerHeight", "600"}
                }},
                {"opendoc", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"extensions", new List<string> {
                        "pdf",
                        "odt",
                        "odp",
                        "ods"
                    }},
                    {"readerWidth", "640"},
                    {"readerHeight", "480"}
                }},
                {"google", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"extensions", new List<string> {
                        "doc",
                        "docx",
                        "xls",
                        "xlsx",
                        "ppt",
                        "pptx"
                    }},
                    {"readerWidth", "640"},
                    {"readerHeight", "480"}
                }},
                {"codeMirrorRenderer", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"extensions", new List<string> {
                        "txt",
                        "csv"
                    }}
                }},
                {"markdownRenderer", new Dictionary<string, dynamic> {
                    {"enabled", true},
                    {"extensions", new List<string> {
                        "md"
                    }}
                }}
            }},
            {"editor", new Dictionary<string, dynamic> {
                {"enabled", true},
                {"theme", "default"},
                {"lineNumbers", true},
                {"lineWrapping", true},
                {"codeHighlight", true},
                {"matchBrackets", true},
                {"extensions", new List<string> {
                    "html",
                    "txt",
                    "csv",
                    "md"
                }}
            }},
            {"customScrollbar", new Dictionary<string, dynamic> {
                {"enabled", true},
                {"theme", "inset-2-dark"},
                {"button", true}
            }},
            {"extras", new Dictionary<string, dynamic> {
                {"extra_js", new List<string> {}},
                {"extra_js_async", true}
            }},
            {"url", "https://github.com/servocoder/RichFilemanager"},
            {"version", "2.7.6"}
        };
    }
}
