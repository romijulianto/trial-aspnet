// Controllers
namespace ASPNETMaker2023.Controllers
{
    /// <summary>
    /// Filemanager controller class
    /// </summary>
    public class FilemanagerConfigController : Controller
    {
        // Get config
        [Route("node_modules/@hkvstore/rich-filemanager/config/filemanager.config.json")]
        public IActionResult Get()
        {
            // Config
            var config = new DotAccessData(Config.FileManager);

            // readOnly - (IMPLICIT) Allow write operations when set to false. Otherwise disable all modifications to the filesystem, including thumbnail generation.
            config.Set("security.readOnly", false);

            // extensions.policy - (IMPLICIT) Takes value "ALLOW_LIST" / "DISALLOW_LIST".
            // If is set to "ALLOW_LIST", only files with extensions that match extensions.restrictions list will be allowed, all other files are forbidden.
            // If is set to "DISALLOW_LIST", all files are allowed except of files with extensions that match extensions.restrictions list.
            config.Set("security.extensions.policy", "ALLOW_LIST");

            // extensions.ignoreCase - (IMPLICIT) Whether extension comparison should be case sensitive.
            config.Set("security.extensions.ignoreCase", true);

            // extensions.restrictions - (IMPLICIT) List of allowed / disallowed extensions, depending on the policy.
            // The presence of an empty string ("") in list affects availability of files without extension.
            config.Set("security.extensions.restrictions", Config.UploadAllowedFileExtensions.Split(','));

            // User config
            object? UserConfig = null;
            if (UserConfig != null) {
                if (IsAnonymousType(UserConfig.GetType()))
                    UserConfig = ConvertToDictionary<dynamic?>(UserConfig);
                if (UserConfig is Dictionary<string, dynamic?> d)
                    config.Import(d);
            } 

            // Remove comment
            config.Remove("_comment");

            // Language
            string lang = CurrentLanguageID;
            if (!(config.Get("language.available")?.Contains(lang) ?? false))
                lang = "en"; // Fallback to English
            config.Set("language.default", lang);

            // Connector URL
            config.Set("api.connectorUrl", FullUrl("filemanager/connect"));

            // Preview URL
            config.Set("viewer.previewUrl", AppPath());

            // Return dictionary
            return Json(config.ToDictionary());
        }
    }

    /// <summary>
    /// Filemanager controller class
    /// </summary>
    [Authorize(Policy = "UserLevel")]
    public class FilemanagerController : Controller
    {
        private readonly string _webRootPath;

        private readonly string _webPath;

        private readonly List<string> _allowedExtensions;

        public FilemanagerController(IWebHostEnvironment env)
        {
            // FileManager Content Folder
            // _webPath = "ContentLibrary";
            _webPath = UploadPath(false) + ""; // All files will be uploaded to a subfolder named userFilesPath under upload temp folder // DN
            if (string.IsNullOrWhiteSpace(env.WebRootPath))
            {
                env.WebRootPath = Directory.GetCurrentDirectory();
            }
            _webRootPath = Path.Combine(env.WebRootPath, _webPath);
            CreateFolder(_webRootPath); // DN
            // _allowedExtensions = new List<string> { "jpg", "jpe", "jpeg", "gif", "png", "svg", "txt", "pdf", "odp", "ods", "odt", "rtf", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "csv", "ogv", "avi", "mkv", "mp4", "webm", "m4v", "ogg", "mp3", "wav", "zip", "rar", "md" };
            _allowedExtensions = Config.UploadAllowedFileExtensions.Split(',').ToList(); // DN
        }

        [Route("filemanager/connect", Name = "filemanager-connect")]
        [Route("home/filemanager/connect", Name = "filemanager-connect-2")]
        public async Task<IActionResult> Index(string mode, string path, string name, List<IFormFile> files, string old, string @new, string source, string target, string content, bool thumbnail, string @string)
        {
            try
            {
                mode ??= Post("mode"); // For mode="upload" // DN
                if (mode == null)
                    return new EmptyResult();
                path ??= Post("path"); // For mode="upload" // DN
                if (!string.IsNullOrWhiteSpace(path) && path.StartsWith("/"))
                    path = path.Substring(1);
                if (!string.IsNullOrWhiteSpace(@new) && @new.StartsWith("/"))
                    @new = @new == "/" ? String.Empty : @new.Substring(1);
                if (!string.IsNullOrWhiteSpace(source) && source.StartsWith("/"))
                    source = source.Substring(1);
                if (!string.IsNullOrWhiteSpace(target) && target.StartsWith("/"))
                    target = target.Substring(1);
                var settings = new JsonSerializerSettings {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }; // DN
                if (SameText(mode, "upload")) { // DN
                    var result = Upload(path, files).GetAwaiter().GetResult();
                    if (GetProperty(result, "Data") != null)
                        return Json(result, settings);
                    else if (GetProperty(result, "Errors") != null)
                        return new JsonResult(result, settings)
                        {
                            StatusCode = StatusCodes.Status500InternalServerError
                        };
                    else
                        throw new Exception("Failed to Upload!");
                }
                return mode.ToLower() switch
                {
                    "initiate" => Json(Initiate(), settings),
                    "getinfo" => Json(GetInfo(path), settings),
                    "readfolder" => Json(ReadFolder(path), settings),
                    "addfolder" => Json(AddFolder(path, name), settings),
                    //"upload" => Json(Upload(path, files).GetAwaiter().GetResult(), settings), // DN
                    "rename" => Json(Rename(old, @new), settings),
                    "move" => Json(Move(old, @new), settings),
                    "copy" => Json(Copy(source, target), settings),
                    "savefile" => Json(SaveFile(path, content), settings),
                    "delete" => Json(Delete(path), settings),
                    "download" => await Download(path),
                    "getimage" => await GetImage(path, thumbnail),
                    "readfile" => await ReadFile(path),
                    "summarize" => Json(Summarize(), settings),
                    "seekfolder" => Json(SeekFolder(path, @string), settings),
                    _ => throw new Exception("Unknown Request!")
                };
            }
            catch (Exception e)
            {
                // returns all unhandeled exceptions and returns them in JSON format with 500.
                // Issue #314
                return new JsonResult(e.Message)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ContentType = "application/json"
                };
            }
        }

        private dynamic Initiate()
        {
            var result = new
            {
                Data = new
                {
                    Type = "initiate",
                    Attributes = new
                    {
                        Config = new
                        {
                            Security = new
                            {
                                ReadOnly = false,
                                Extensions = new
                                {
                                    IgnoreCase = true,
                                    Policy = "ALLOW_LIST",
                                    Restrictions = _allowedExtensions
                                }
                            }
                        }
                    }
                }
            };
            return result;
        }

        private Int32 GetUnixTimestamp(DateTime dt)
        {
            return (Int32)(dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        private dynamic SeekFolder(string path, string search)
        {
            path ??= String.Empty;
            var searchPath = Path.Combine(_webRootPath, path);
            var data = new List<dynamic>();
            foreach (FileInfo file in GetDirectoryInfo(searchPath).GetFiles("*" + search + "*", SearchOption.AllDirectories))
            {
                var item = new
                {
                    Id = MakeWebPath(Path.Combine(Path.GetRelativePath(_webRootPath, file.DirectoryName ?? ""), file.Name), true),
                    Type = "file",
                    Attributes = new
                    {
                        Name = file.Name,
                        Path = MakeWebPath(Path.Combine(Path.GetRelativePath(_webRootPath, file.DirectoryName ?? ""), file.Name), true),
                        Readable = 1,
                        Writable = 1,
                        Created = GetUnixTimestamp(file.CreationTimeUtc),
                        Modified = GetUnixTimestamp(file.LastWriteTimeUtc),
                        Size = file.Length,
                        Extension = file.Extension.TrimStart('.'),
                        // Insert Height and Width logic for images
                        Timestamp = DateTime.Now.Subtract(file.LastWriteTime).TotalSeconds
                    }
                };
                data.Add(item);
            }
            foreach (DirectoryInfo dir in GetDirectoryInfo(searchPath).GetDirectories("*" + search + "*", SearchOption.AllDirectories))
            {
                var item = new
                {
                    Id = MakeWebPath(Path.GetRelativePath(_webRootPath, dir.FullName), false, true),
                    Type = "folder",
                    Attributes = new
                    {
                        Name = dir.Name,
                        Path = MakeWebPath(dir.FullName, true, true),
                        Readable = 1,
                        Writable = 1,
                        Created = GetUnixTimestamp(dir.CreationTimeUtc),
                        Modified = GetUnixTimestamp(dir.LastWriteTimeUtc),
                        Timestamp = DateTime.Now.Subtract(dir.LastWriteTime).TotalSeconds
                    }
                };
                data.Add(item);
            }
            return new
            {
                Data = data
            };
        }

        private dynamic GetInfo(string path)
        {
            path ??= String.Empty;
            var filePath = Path.Combine(_webRootPath, path);
            FileInfo file = new FileInfo(path);
            return new
            {
                Data = new
                {
                    Id = MakeWebPath(Path.Combine(Path.GetRelativePath(_webRootPath, file.DirectoryName ?? ""), file.Name), true),
                    Type = "file",
                    Attributes = new
                    {
                        Name = file.Name,
                        Path = MakeWebPath(Path.Combine(Path.GetRelativePath(_webRootPath, file.DirectoryName ?? ""), file.Name), false),
                        Readable = 1,
                        Writable = 1,
                        Created = GetUnixTimestamp(file.CreationTimeUtc),
                        Modified = GetUnixTimestamp(file.LastWriteTimeUtc),
                        Size = file.Length,
                        Extension = file.Extension.TrimStart('.'),
                        Timestamp = DateTime.Now.Subtract(file.LastWriteTime).TotalSeconds
                    }
                }
            };
        }

        private dynamic ReadFolder(string path)
        {
            path ??= String.Empty;
            var rootpath = Path.Combine(_webRootPath, path);
            var rootDirectory = GetDirectoryInfo(rootpath);
            var data = new List<dynamic>();
            foreach (var directory in rootDirectory.GetDirectories())
            {
                var item = new
                {
                    Id = MakeWebPath(Path.Combine(path, directory.Name), false, true),
                    Type = "folder",
                    Attributes = new
                    {
                        Name = directory.Name,
                        Path = MakeWebPath(Path.Combine(_webPath, path, directory.Name), true, true),
                        Readable = 1,
                        Writable = 1,
                        Created = GetUnixTimestamp(directory.CreationTime),
                        Modified = GetUnixTimestamp(directory.LastWriteTime),
                        Timestamp = DateTime.Now.Subtract(directory.LastWriteTime).TotalSeconds
                    }
                };
                data.Add(item);
            }
            foreach (var file in rootDirectory.GetFiles())
            {
                var item = new
                {
                    Id = MakeWebPath(Path.Combine(path, file.Name)),
                    Type = "file",
                    Attributes = new
                    {
                        Name = file.Name,
                        Path = MakeWebPath(Path.Combine(_webPath, path, file.Name), true),
                        Readable = 1,
                        Writable = 1,
                        Created = GetUnixTimestamp(file.CreationTime),
                        Modified = GetUnixTimestamp(file.LastWriteTime),
                        Extension = file.Extension.Replace(".", ""),
                        Size = file.Length,
                        Timestamp = DateTime.Now.Subtract(file.LastWriteTime).TotalSeconds,
                    }
                };
                data.Add(item);
            }
            var result = new
            {
                Data = data
            };
            return result;
        }

        private dynamic AddFolder(string path, string name)
        {
            var newDirectoryPath = Path.Combine(_webRootPath, path, name);
            var directoryExist = DirectoryExists(newDirectoryPath);
            if (directoryExist)
            {
                var errorResult = new { Errors = new List<dynamic>() };
                errorResult.Errors.Add(new
                {
                    Code = "500",
                    Title = "DIRECTORY_ALREADY_EXISTS",
                    Meta = new
                    {
                        Arguments = new List<string>
                        {
                            name
                        }
                    }
                });
                return errorResult;
            }
            DirectoryCreate(newDirectoryPath);
            var directory = GetDirectoryInfo(newDirectoryPath);
            var result = new
            {
                Data =
                    new
                    {
                        Id = MakeWebPath(Path.Combine(path, directory.Name), false, true),
                        Type = "folder",
                        Attributes = new
                        {
                            Name = directory.Name,
                            Path = MakeWebPath(Path.Combine(_webPath, path, directory.Name), true, true),
                            Readable = 1,
                            Writable = 1,
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                        }
                    }
            };
            return result;
        }

        private async Task<dynamic> Upload(string path, IEnumerable<IFormFile> files)
        {
            var result = new { Data = new List<dynamic>() };
            foreach (var file in files)
            {
                if (file.Length <= 0) continue;
                var fileExist = FileExists(Path.Combine(_webRootPath, path, file.FileName));
                if (fileExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Title = "FILE_ALREADY_EXISTS",
                        Meta = new
                        {
                            Arguments = new List<string>
                            {
                                file.FileName
                            }
                        }
                    });
                    return errorResult;
                }
                using (var fileStream = new FileStream(Path.Combine(_webRootPath, path, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                result.Data.Add(new
                {
                    Id = MakeWebPath(Path.Combine(path, file.FileName)),
                    Type = "file",
                    Attributes = new
                    {
                        Name = file.FileName,
                        Extension = Path.GetExtension(file.FileName).Replace(".", ""),
                        Path = MakeWebPath(Path.Combine(_webPath, path, file.FileName), true),
                        Readable = 1,
                        Writable = 1,
                        Created = GetUnixTimestamp(DateTime.Now),
                        Modified = GetUnixTimestamp(DateTime.Now),
                        Size = file.Length
                    }
                });
            }
            return result;
        }

        private dynamic Rename(string old, string @new)
        {
            var oldPath = Path.Combine(_webRootPath, old);
            if (IsDirectory(oldPath)) //Fixed if the directory is compressed
            {
                var oldDirectoryName = Path.GetDirectoryName(old)?.Split('\\').Last() ?? "";
                var newDirectoryPath = old.Replace(oldDirectoryName ?? "", @new);
                var newPath = Path.Combine(_webRootPath, newDirectoryPath);
                var directoryExist = DirectoryExists(newPath);
                if (directoryExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Title = "DIRECTORY_ALREADY_EXISTS",
                        Meta = new
                        {
                            Arguments = new List<string>
                            {
                                @new
                            }
                        }
                    });
                    return errorResult;
                }
                DirectoryMove(oldPath, newPath);
                var result = new
                {
                    Data = new
                    {
                        Id = newDirectoryPath,
                        Type = "folder",
                        Attributes = new
                        {
                            Name = @new,
                            Readable = 1,
                            Writable = 1,
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                        }
                    }
                };
                return result;
            }
            else
            {
                var oldFileName = Path.GetFileName(old);
                var newFilePath = old.Replace(oldFileName, @new);
                var newPath = Path.Combine(_webRootPath, newFilePath);
                var fileExist = FileExists(newPath);
                if (fileExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Title = "FILE_ALREADY_EXISTS",
                        Meta = new
                        {
                            Arguments = new List<string>
                            {
                                @new
                            }
                        }
                    });
                    return errorResult;
                }
                FileMove(oldPath, newPath);
                var result = new
                {
                    Data = new
                    {
                        Id = newFilePath,
                        Type = "file",
                        Attributes = new
                        {
                            Name = @new,
                            Extension = Path.GetExtension(newPath).Replace(".", ""),
                            Readable = 1,
                            Writable = 1,
                            // created date, size vb.
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                        }
                    }
                };
                return result;
            }
        }

        private dynamic Move(string old, string @new)
        {
            if (IsDirectory(Path.Combine(_webRootPath, old))) //Fixed if the directory is compressed
            {
                string directoryName = Path.GetDirectoryName(old)?.Split('\\').Last() ?? "";
                var newDirectoryPath = Path.Combine(@new, directoryName);
                var oldPath = Path.Combine(_webRootPath, old);
                var newPath = Path.Combine(_webRootPath, @new, directoryName);
                var directoryExist = DirectoryExists(newPath);
                if (directoryExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Title = "DIRECTORY_ALREADY_EXISTS",
                        Meta = new
                        {
                            Arguments = new List<string>
                            {
                                directoryName
                            }
                        }
                    });
                    return errorResult;
                }
                DirectoryMove(oldPath, newPath);
                var result = new
                {
                    Data = new
                    {
                        Id = newDirectoryPath,
                        Type = "folder",
                        Attributes = new
                        {
                            Name = directoryName,
                            Readable = 1,
                            Writable = 1,
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                        }
                    }
                };
                return result;
            }
            else
            {
                var fileName = Path.GetFileName(old);
                var newFilePath = Path.Combine(@new, fileName);
                var oldPath = Path.Combine(_webRootPath, old);
                var newPath = @new == "/"
                    ? Path.Combine(_webRootPath, fileName.Replace("/", ""))
                    : Path.Combine(_webRootPath, @new, fileName);
                var fileExist = FileExists(newPath);
                if (fileExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Title = "FILE_ALREADY_EXISTS",
                        Meta = new
                        {
                            Arguments = new List<string>
                            {
                                fileName
                            }
                        }
                    });
                    return errorResult;
                }
                FileMove(oldPath, newPath);
                var result = new
                {
                    Data = new
                    {
                        Id = newFilePath,
                        Type = "file",
                        Attributes = new
                        {
                            Name = fileName,
                            Extension = Path.GetExtension(@new).Replace(".", ""),
                            Readable = 1,
                            Writable = 1,
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                        }
                    }
                };
                return result;
            }
        }

        private dynamic Copy(string source, string target)
        {
            if (IsDirectory(Path.Combine(_webRootPath, source))) //Fixed if the directory is compressed
            {
                string directoryName = Path.GetDirectoryName(source)?.Split('\\').Last() ?? "";
                var newDirectoryPath = Path.Combine(target, directoryName);
                var oldPath = Path.Combine(_webRootPath, source);
                var newPath = Path.Combine(_webRootPath, target, directoryName);
                var directoryExist = DirectoryExists(newPath);
                if (directoryExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Title = "DIRECTORY_ALREADY_EXISTS",
                        Meta = new
                        {
                            Arguments = new List<string>
                            {
                                directoryName
                            }
                        }
                    });
                    return errorResult;
                }
                DirectoryCopy(oldPath, newPath);
                var result = new
                {
                    Data = new
                    {
                        Id = newDirectoryPath,
                        Type = "folder",
                        Attributes = new
                        {
                            Name = directoryName,
                            Readable = 1,
                            Writable = 1,
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                        }
                    }
                };
                return result;
            }
            else
            {
                var fileName = Path.GetFileName(source);
                var newFilePath = Path.Combine(@target, fileName);
                var oldPath = Path.Combine(_webRootPath, source);
                var newPath = Path.Combine(_webRootPath, target, fileName);
                var fileExist = FileExists(newPath);
                if (fileExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Title = "FILE_ALREADY_EXISTS",
                        Meta = new
                        {
                            Arguments = new List<string>
                            {
                                fileName
                            }
                        }
                    });
                    return errorResult;
                }
                FileCopy(oldPath, newPath, false);
                var result = new
                {
                    Data = new
                    {
                        Id = newFilePath,
                        Type = "file",
                        Attributes = new
                        {
                            Name = fileName,
                            Extension = Path.GetExtension(fileName).Replace(".", ""),
                            Readable = 1,
                            Writable = 1,
                            // created date, size vb.
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                        }
                    }
                };
                return result;
            }
        }

        private dynamic SaveFile(string path, string content)
        {
            var filePath = Path.Combine(_webRootPath, path);
            FileWriteAllText(filePath, content);
            var fileName = Path.GetFileName(path);
            var fileExtension = Path.GetExtension(fileName);
            var result = new
            {
                Data = new
                {
                    Id = path,
                    Type = "file",
                    Attributes = new
                    {
                        Name = fileName,
                        Extension = fileExtension,
                        Readable = 1,
                        Writable = 1
                    }
                }
            };
            return result;
        }

        private dynamic Delete(string path)
        {
            if (IsDirectory(Path.Combine(_webRootPath, path))) //Fixed if the directory is compressed
            {
                string directoryName = Path.GetDirectoryName(path)?.Split('\\').Last() ?? "";
                DirectoryDelete(Path.Combine(_webRootPath, path));
                var result = new
                {
                    Data = new
                    {
                        Id = path,
                        Type = "folder",
                        Attributes = new
                        {
                            Name = directoryName,
                            Readable = 1,
                            Writable = 1,
                            // created date, size vb.
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now),
                            Path = path
                        }
                    }
                };
                return result;
            }
            else
            {
                var fileName = Path.GetFileName(Path.Combine(_webRootPath, path));
                var fileExtension = Path.GetExtension(fileName).Replace(".", "");
                FileDelete(Path.Combine(_webRootPath, path));
                var result = new
                {
                    Data = new
                    {
                        Id = path,
                        Type = "file",
                        Attributes = new
                        {
                            Name = fileName,
                            Extension = fileExtension,
                            Readable = 1,
                            Writable = 1,
                            Created = GetUnixTimestamp(DateTime.Now),
                            Modified = GetUnixTimestamp(DateTime.Now)
                            // Path = $"/{fileName}"
                        }
                    }
                };
                return result;
            }
        }

        private async Task<IActionResult> ReadFile(string path)
        {
            var filePath = Path.Combine(_webRootPath, path);
            var fileName = Path.GetFileName(filePath);
            byte[] fileBytes = await FileReadAllBytes(filePath);
            var cd = new System.Net.Mime.ContentDisposition
            {
                Inline = true,
                FileName = fileName
            };
            Response?.Headers.Add("Content-Disposition", cd.ToString());
            return File(fileBytes, "application/octet-stream");
        }

        private async Task<IActionResult> GetImage(string path, bool thumbnail)
        {
            var filePath = Path.Combine(_webRootPath, path);
            var fileName = Path.GetFileName(filePath);
            byte[] fileBytes = await FileReadAllBytes(filePath);
            var cd = new System.Net.Mime.ContentDisposition
            {
                Inline = true,
                FileName = fileName
            };
            Response?.Headers.Add("Content-Disposition", cd.ToString());
            return File(fileBytes, "image/*");
        }

        private async Task<IActionResult> Download(string path)
        {
            var filePath = Path.Combine(_webRootPath, path);
            var fileName = Path.GetFileName(filePath);
            byte[] fileBytes = await FileReadAllBytes(filePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }

        private dynamic Summarize()
        {
            // Get Dir count
            var directories = GetDirectories(_webRootPath).Length;

            // Get file count
            var directoryInfo = GetDirectoryInfo(_webRootPath);
            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

            // Get combined file sizes
            var allSize = files.Select(f => f.Length).Sum();
            var result = new
            {
                Data = new
                {
                    Id = "/",
                    Type = "summary",
                    Attributes = new
                    {
                        Size = allSize,
                        Files = files.Length,
                        Folders = directories,
                        SizeLimit = 0
                    }
                }
            };
            return result;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            var dir = GetDirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!DirectoryExists(destDirName))
            {
                DirectoryCreate(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        private static string MakeWebPath(string path, bool addSeperatorToBegin = false, bool addSeperatorToLast = false)
        {
            path = path.Replace("\\", "/");
            if (addSeperatorToBegin)
                path = "/" + path;
            if (addSeperatorToLast)
                path = path + "/";
            return path;
        }
    }
}
