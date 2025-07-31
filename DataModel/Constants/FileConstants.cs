using DataModel.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
namespace DataModel.Constants
{
    public  class FileConstants
    {
        //FileHandling Constants
        public  string _webHostEnvironment;
        public static string userFolderPath="";
        public const string UserImagesFolder = "UserImages";
        public const string DefaultFileExtension = ".jpg";
        public const string ImageFileNameFormat = "{0}_{1}{2}"; // e.g., Guid_Sequence.Extension
        public static IFormFile _formFile;
        public static string _fileName;
        public static string _filePath;
        public static int _sequence;

        //ImageFileHandling Methods
        public FileConstants(string webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public FileConstants()
        {

        }
        public  void SetFormFile(IFormFile formFile)
        {
            _formFile = formFile;
        }
        public  void SetSequence(int sequence)
        {
            _sequence = sequence;
        }
        public  void SetUserFolderPath(string userName)
        {
            userFolderPath= Path.Combine(_webHostEnvironment, UserImagesFolder, userName);
        }

        public  string GetFullImagePath(string folderPath, string fileName)
        {
            return Path.Combine(folderPath, fileName);
        }

        public  void GenerateFileName()
        {
            _fileName =  string.Format(ImageFileNameFormat, Guid.NewGuid(), _sequence, Path.GetExtension(_formFile.FileName));
        }
        public  void GenerateFilePath()
        {
            _filePath = Path.Combine(userFolderPath, _fileName);
        }
        public  void CopyFileContent()
        {
            using (var stream = new FileStream(_filePath, FileMode.Create))
            {
                _formFile.CopyTo(stream);
            }
        }
        public  string SetImageURI(string userName)
        {
            return Path.Combine("UserImages", userName, FileConstants._fileName).Replace("\\", "/");
        }
    }
}

