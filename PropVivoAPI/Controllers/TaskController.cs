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

        [HttpPost("AddTask")]
        public async Task<IActionResult> AddTask([FromBody] TaskMasterDTO taskDto)
        {
            try
            {
                // Optional: Validate inputs
                if (string.IsNullOrEmpty(taskDto.TaskTitle) || string.IsNullOrEmpty(taskDto.EstimatedHrs))
                {
                    return BadRequest(new { success = false, message = "Task title and estimated hours are required." });
                }

                // Create a new TaskMaster entity
                var newTask = new TaskMaster
                {
                    CreatedBy = taskDto.CreatedBy,
                    TaskTitle = taskDto.TaskTitle,
                    Description = taskDto.Description,
                    EstimatedHrs = taskDto.EstimatedHrs,
                    CreatedAt = DateTime.UtcNow  // Auto-set creation time
                };

                // Save to DB
                _propvivoContext.TaskMasters.Add(newTask);
                await _propvivoContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Task added successfully",
                    task = new
                    {
                        newTask.TaskId,
                        newTask.TaskTitle,
                        newTask.Description,
                        newTask.EstimatedHrs,
                        newTask.CreatedBy,
                        newTask.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _propvivoContext.TaskMasters
                .Select(t => new TaskMasterDTO
                {
                    TaskId = t.TaskId,
                    TaskTitle = t.TaskTitle,
                    Description = t.Description,
                    EstimatedHrs = t.EstimatedHrs,
                    CreatedBy = t.CreatedBy,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(new { success = true, data = tasks });
        }

        [HttpGet("GetTaskById/{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _propvivoContext.TaskMasters
                .Where(t => t.TaskId == id)
                .Select(t => new TaskMasterDTO
                {
                    TaskId = t.TaskId,
                    TaskTitle = t.TaskTitle,
                    Description = t.Description,
                    EstimatedHrs = t.EstimatedHrs,
                    CreatedBy = t.CreatedBy,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound(new { success = false, message = "Task not found" });

            return Ok(new { success = true, data = task });
        }

        [HttpGet("UnassignedTask")]
        public async Task<IActionResult> GetUnassignedTask()
        {
            var task = await _propvivoContext.TaskMasters
                .Where(t => !_propvivoContext.TaskAssignments.Any(ft=>ft.TaskId==t.TaskId))
                .Select(t => new TaskMasterDTO
                {
                    TaskId = t.TaskId,
                    TaskTitle = t.TaskTitle,
                    Description = t.Description,
                    EstimatedHrs = t.EstimatedHrs,
                    CreatedBy = t.CreatedBy,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound(new { success = false, message = "Task not found" });

            return Ok(new { success = true, data = task });
        }

        [HttpPut("UpdateTask/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskMasterDTO updatedTask)
        {
            var task = await _propvivoContext.TaskMasters.FindAsync(id);

            if (task == null)
                return NotFound(new { success = false, message = "Task not found" });

            task.TaskTitle = updatedTask.TaskTitle;
            task.Description = updatedTask.Description;
            task.EstimatedHrs = updatedTask.EstimatedHrs;
            task.CreatedBy = updatedTask.CreatedBy;

            await _propvivoContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Task updated successfully" });
        }

        
    }
}
