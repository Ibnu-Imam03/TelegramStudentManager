using Microsoft.AspNetCore.Mvc;
using TelegramStudentManager.Business;

namespace TelegramStudentManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            return Ok(clsStudent.GetAllStudents());
        }

        [HttpGet("{StudentID}")]
        public IActionResult GetStudentByID(int StudentID)
        {
            clsStudent Student = clsStudent.FindStudentByID(StudentID);

            if (Student == null)
            {
                return NotFound($"Student with ID {StudentID} was not found.");
            }

            return Ok(Student);
        }

        [HttpPost]
        public IActionResult AddStudent(clsStudent Student)
        {
            if (Student.Save())
            {
                return Ok(Student);
            }

            return BadRequest("Failed to add student.");
        }
        [HttpPut("{StudentID}")]
        public IActionResult UpdateStudent(int StudentID, clsStudent Student)
        {
            clsStudent ExistingStudent = clsStudent.FindStudentByID(StudentID);

            if (ExistingStudent == null)
            {
                return NotFound($"Student with ID {StudentID} was not found.");
            }

            ExistingStudent.FullName = Student.FullName;
            ExistingStudent.StudentNumber = Student.StudentNumber;
            ExistingStudent.Department = Student.Department;
            ExistingStudent.Year = Student.Year;
            ExistingStudent.Phone = Student.Phone;

            if (ExistingStudent.Save())
            {
                return Ok(ExistingStudent);
            }

            return BadRequest("Failed to update student.");
        }
        [HttpDelete("{StudentID}")]
        public IActionResult DeleteStudent(int StudentID)
        {
            clsStudent Student = clsStudent.FindStudentByID(StudentID);

            if (Student == null)
            {
                return NotFound($"Student with ID {StudentID} was not found.");
            }

            if (clsStudent.DeleteStudent(StudentID))
            {
                return Ok($"Student with ID {StudentID} deleted successfully.");
            }

            return BadRequest("Failed to delete student.");
        }
    }
}