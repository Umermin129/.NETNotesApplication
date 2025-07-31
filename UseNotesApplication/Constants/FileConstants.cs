using UseNotesApplication.Models;

namespace UseNotesApplication.Constants
{
    public static class FileConstants
    {
        //FileHandling Constants
        public static string userFolderPath="";
        public const string UserImagesFolder = "UserImages";
        public const string DefaultFileExtension = ".jpg";
        public const string ImageFileNameFormat = "{0}_{1}{2}"; // e.g., Guid_Sequence.Extension
        public static IFormFile _formFile;
        public static string _fileName;
        public static string _filePath;
        public static int _sequence;

        //ImageFileHandling Methods
        public static void SetFormFile(IFormFile formFile)
        {
            _formFile = formFile;
        }
        public static void SetSequence(int sequence)
        {
            _sequence = sequence;
        }
        public static void SetUserFolderPath(string webRootPath,string userName)
        {
            userFolderPath= Path.Combine(webRootPath, UserImagesFolder, userName);
        }

        public static string GetFullImagePath(string folderPath, string fileName)
        {
            return Path.Combine(folderPath, fileName);
        }

        public static void GenerateFileName()
        {
            _fileName =  string.Format(ImageFileNameFormat, Guid.NewGuid(), _sequence, Path.GetExtension(_formFile.FileName));
        }
        public static void GenerateFilePath()
        {
            _filePath = Path.Combine(userFolderPath, _fileName);
        }
        public static void CopyFileContent()
        {
            using (var stream = new FileStream(_filePath, FileMode.Create))
            {
                _formFile.CopyTo(stream);
            }
        }
        public static string SetImageURI(string userName)
        {
            return Path.Combine("UserImages", userName, FileConstants._fileName).Replace("\\", "/");
        }
    }
}

