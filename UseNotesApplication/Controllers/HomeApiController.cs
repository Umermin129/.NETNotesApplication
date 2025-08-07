using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using ViewModel.Notes;

namespace UseNotesApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        //Services Reference
        UserServices _userService;
        NotesServices _noteService;
        public HomeApiController( UserServices userServices, NotesServices notesServices)
        {
            _userService = userServices;
            _noteService = notesServices;
        }
        //Notes Creation
        [HttpPost("CreateNote/{userName}")]
        public IActionResult CreateNote(string userName, [FromBody] TaskEditViewModel model)
        {
            try
            {
                var userData = _userService.GetUser(userName);
                if (userData == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                _noteService.CreateNotes(userData, model);

                return Ok(new { message = "Note created successfully." ,data = model});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the note." });
            }
        }
        //Get Notes
        [HttpGet("GetNote/{noteId}")]
        public IActionResult GetNote( int noteId)
        {
            try
            {
                var noteData = _noteService.GetNote(noteId);
                if (noteData == null)
                {
                    return NotFound(new { message = "Note not found." });
                }
                var noteViewModel = _noteService.CreateViewModel(noteData);

                return Ok(new
                {
                    message = "Note retrieved successfully.",
                    data = noteViewModel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the note." });
            }
        }
        //Update Notes
        [HttpPut("EditNote/{noteId}")]
        public IActionResult EditNote(int noteId, [FromBody] TaskEditViewModel model)
        {
            try
            {
               
                var noteData = _noteService.GetNote( noteId);
                if (noteData == null)
                    return NotFound(new { message = "Note not found." });

                _noteService.CreateNoteVersion(noteData); 
                _noteService.UpdateNote(noteData, model); 

                return Ok(new { message = "Note updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the note." });
            }
        }
        //DeleteNote 
        [HttpDelete("DeleteNote/{Id}")]
        public IActionResult Delete(int Id)
        {
            var note = _noteService.GetNote(Id);

            if (note == null)
                return NotFound(new { message = "Note not found." });

            _noteService.DeleteNote(note);
            return Ok(new { message = "Note Deleted successfully." });
        }
    }
}
