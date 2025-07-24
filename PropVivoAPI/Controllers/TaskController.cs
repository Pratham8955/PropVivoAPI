using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropVivoAPI.DTO;
using PropVivoAPI.Models;

namespace PropVivoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly PropvivoContext _propvivoContext;
        public TaskController(PropvivoContext propvivoContext)
        {
            _propvivoContext = propvivoContext; 
        }

        //[HttpPost("AddEmployee")]

        //public IActionResult AddEmployee(TaskMasterDTO task)
        //{
        //    try
        //    {
        //        if (_propvivoContext.tas.Any(fs => fs.DeptName == addDepartment.DeptName))
        //        {
        //            return BadRequest(new
        //            {
        //                success = false,
        //                message = "This department already exists."
        //            });
        //        }

        //        var department = new Department
        //        {
        //            DeptName = addDepartment.DeptName
        //        };
        //        _context.Departments.Add(department);
        //        await _context.SaveChangesAsync();

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Department Added Successfully",
        //            Department = new
        //            {
        //                department.DeptName,
        //                department.HeadOfDept,
        //            },
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { success = false, message = ex.Message });
        //    }
        //}


    }
}
