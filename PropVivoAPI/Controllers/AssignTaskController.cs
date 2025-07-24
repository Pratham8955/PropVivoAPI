using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropVivoAPI.DTO;
using PropVivoAPI.Enums;
using PropVivoAPI.Models;

namespace PropVivoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignTaskController : ControllerBase
    {
        private readonly PropvivoContext _propvivoContext;
        public AssignTaskController(PropvivoContext propvivoContext)
        {
            _propvivoContext = propvivoContext;
        }

        [HttpPost("AddTaskToEmployee")]
        public async Task<IActionResult> AddTaskToEmployee([FromBody] AssignTaskDTO assign)
        {
            try
            {
                var taskassign = new TaskAssignment
                {
                    UserId = assign.UserId.Value,
                    TaskId = assign.TaskId.Value,
                    AssignedAt = DateTime.UtcNow,
                    Status = assign.Status.ToString()
                };

                _propvivoContext.TaskAssignments.Add(taskassign);
                await _propvivoContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Task added successfully",
                    task = new
                    {
                        taskassign.UserId,
                        taskassign.TaskId,
                        taskassign.AssignedAt,
                        Status = taskassign.Status.ToString() // optional string conversion
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetAssignedTasks")]
        public async Task<IActionResult> GetAssignedTasks()
        {
            try
            {
                var assignedTasks = await _propvivoContext.TaskAssignments
                    .Include(t => t.User)
                    .Include(t => t.Task)
                    .Select(t => new
                    {
                        t.TaskAssignId,
                        UserName = t.User.Name,
                        TaskTitle = t.Task.TaskTitle,
                        t.AssignedAt,
                        t.Status
                    })
                    .ToListAsync();
                return Ok(new
                {
                    success = true,
                    message = "Assigned tasks fetched successfully",
                    tasks = assignedTasks
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
