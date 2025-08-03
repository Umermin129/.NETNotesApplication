using Microsoft.AspNetCore.Http;
namespace DataModel.Constants
{
    public class FileConfig
    {
        public static string UserFolderPath;
        public static string _webHostEnvironment = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        public IFormFile FormFile { get; set; }
        public int Sequence { get; set; }
        public string FileName;
        public string Name { get; set; }
        public const string ImageFileNameFormat = "{0}_{1}{2}";
        public string FilePath;
        public string referncePath;
        public FileConfig(IFormFile formFile, int sequence)
        {
            FormFile = formFile;
            Sequence = sequence;

        }

        public static void SetUserFolderPath(string userName)
        {
            UserFolderPath = Path.Combine(_webHostEnvironment, "UserImages", userName).Replace("\\", "/");
        }

        public void GenerateFilePath()
        {
            FileName = string.Format(ImageFileNameFormat, Guid.NewGuid(), Sequence, Path.GetExtension(FormFile.FileName));
            FilePath = Path.Combine(UserFolderPath, FileName).Replace("\\", "/");
        }
        public void GenerateReferencePath(string UserName)
        {
            referncePath = Path.Combine("UserImages", UserName, FileName).Replace("\\", "/");
        }
        public void CopyFileContent()
        {
            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                FormFile.CopyTo(stream);
            }
        }
    }
}

