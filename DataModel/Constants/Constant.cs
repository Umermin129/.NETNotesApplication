namespace DataModel.Constants
{
    public class Constant
    {
        //Error Names
        public static string errImg = "";
        public static string errUserName = "UserName";
        public static string errName = "Name";
        public static string errEmail = "Email";

        //Error Display messages
        public  static string imagesError = "Please Upload exactly 5 images and Sequence!";
        public static string userNameError = "UserName already Exists!";
        public static string userEmailError = "User with this email already Exists!";
        public static string userNameInvalid = "UserName Invalid";
        public static string InvalidSequence = " Sequence Invalid";
        public static string SessionError = "Session Expired";
        public static string NoteNotFoundErr = "Note Not Found";
        public static string UpdateNoteErr = "Note Couldn't Update";
        //Temp Data Messages
        public static string RegisterSuccess = "Registered Successfully";
        public static string LoginSuccess = "Login Successfully";
        public static string ProfileUpdate = "Profile updated successfully!";
        public static string LogoutSuccess = "You Have LoggedOut Successfully";
        public static string NotesSuccess = "Note Successfully Created";
        public static string NoteUpdate = "Notes Updated Successfully";
        public static string NoteRetrieveSuccess = "Note Retrieved Successfully";
        //Action Names
        public static string LoginAction = "Login";
        public static string IndexAction = "Index";
        //View Names
        public static string LoginView = "Login";
        public static string LoginGridView = "LoginGrid";
        //Session Strings
        public static string ExpectedSequence = "ExpectedSequence";
        public static string sessionUserName = "UserName";
        //Routing Paths
        public static string RegisterRoute = "~/Views/Account/Register.cshtml";
        public static string CreateRoute = "~/Views/Notes/Create.cshtml";
        public static string GetNoteRoute = "~/Views/Notes/GetNote.cshtml";
        //ViewNames
        public static string GetProfileView = "GetProfile";
        public static string RegisterView = "Register";
        //Try Catch Error messages
        public static string SaveNotesDB = "Error in Save Notes to DB";
        public static string GetNoteErr = "Error in Get Notes";
  
        public static string SaveNoteVersionErr = "Error in Save NotesVersions to DB";
        public static string AddUserErr = "Error in Add USer";
        public static string GetUserErr = "Error in Get USer";
        public static string UpdateNotesDb = "Error in the Update Notes to DB";
        public static string UserCheckErr = "Error in User Check";
        public static string EmailCheckErr = "Error in Email Check";
        public static string CreateUserFolderErr = "Error in Create UserFolder";
        public static string FileCopyErr = "Error in File Copy";
        public static string PictureDBErr = "Error in Adding Pictures to DB";
        public static string UserPicErr = "Error in Get User Picture";
        //Get Random grid
        public   static int GenerateCode()
        {
            Random random = new Random();
            int number = random.Next(1, 11);
            Console.WriteLine(number);
            return number;
        }


    }
}
