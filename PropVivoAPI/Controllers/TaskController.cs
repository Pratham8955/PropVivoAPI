using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropVivoAPI.DTO;
using PropVivoAPI.DTO.AddDataDTO;
using PropVivoAPI.Enums;
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
                .Where(t => !_propvivoContext.TaskAssignments.Any(ft => ft.TaskId == t.TaskId))
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

        //[HttpGet("getAlltaskByEmployee")]
        //public async Task<IActionResult> getAllTaskByEmployee(int userid)
        //{

        //    var task = await _propvivoContext.TaskAssignments.Where(ta => ta.UserId == userid).Include(t => t.Task).ThenInclude(t => t.CreatedByNavigation).
        //        Select(ta => new
        //        {

        //            ta.TaskAssignId,
        //            ta.TaskId,
        //            ta.Task.TaskTitle,
        //            ta.Task.Description,
        //            ta.Task.EstimatedHrs,
        //            CreatedAt = ta.Task.CreatedAt,
        //            CreatedByName = ta.Task.CreatedByNavigation.Name,
        //            Status = ta.Status,
        //            AssignedAt = ta.AssignedAt
        //        }).ToListAsync();

        //    return Ok(new
        //    {
        //        success = true,
        //        message = "Tasks fetched successfully.",
        //        data = task
        //    });
        //}

       

        [HttpPost("StartTask/{taskAssignId}")]
        public async Task<IActionResult> StartTask(int taskAssignId)
        {
            var newTaskTime = new TaskTimeTracking
            {
                TaskAssignId = taskAssignId,
                StartedAt = DateTime.UtcNow
            };

            _propvivoContext.TaskTimeTrackings.Add(newTaskTime);
            await _propvivoContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Task started." });
        }


        //[HttpPost("PauseTask/{taskAssignId}")]
        //public async Task<IActionResult> PauseTask(int taskAssignId)
        //{
        //    // 1. Find current active task time tracking
        //    var taskTime = await _propvivoContext.TaskTimeTrackings
        //        .Where(x => x.TaskAssignId == taskAssignId && x.EndedAt == null)
        //        .OrderByDescending(x => x.StartedAt)
        //        .FirstOrDefaultAsync();

        //    if (taskTime == null)
        //        return BadRequest("No active task session found.");

        //    // 2. Close task time tracking
        //    taskTime.EndedAt = DateTime.UtcNow;
        //    TimeSpan duration = taskTime.EndedAt.Value - taskTime.StartedAt;

        //    // Save as TimeSpan (if your DB column is `time(7)`)
        //    taskTime.TotalTime = duration;

        //    _propvivoContext.TaskTimeTrackings.Update(taskTime);

        //    // 3. Start new break log tracking entry
        //    var newBreakLog = new BreakLogTracking
        //    {
        //        TaskAssignId = taskAssignId,
        //        StartedAt = DateTime.UtcNow, // same as pause time
        //        EndedAt = null,
        //        TotalTime = null // will be set when break ends
        //    };

        //    await _propvivoContext.BreakLogTrackings.AddAsync(newBreakLog);

        //    // 4. Save both operations
        //    await _propvivoContext.SaveChangesAsync();

        //    return Ok("Task paused. Break started.");
        //}





        //[HttpPost("ResumeTask/{taskAssignId}")]
        //public async Task<IActionResult> ResumeTask(int taskAssignId)
        //{
        //    // 1. Find active break
        //    var activeBreak = await _propvivoContext.BreakLogTrackings
        //        .Where(b => b.TaskAssignId == taskAssignId && b.EndedAt == null)
        //        .OrderByDescending(b => b.StartedAt)
        //        .FirstOrDefaultAsync();

        //    if (activeBreak != null)
        //    {
        //        // 2. End the break
        //        activeBreak.EndedAt = DateTime.UtcNow;
        //        var breakDuration = activeBreak.EndedAt.Value - activeBreak.StartedAt;

        //        activeBreak.TotalTime = breakDuration;

        //        // ✅ Add this line to update the break
        //        _propvivoContext.BreakLogTrackings.Update(activeBreak);
        //    }

        //    // 3. Start a new task session
        //    var newTaskTime = new TaskTimeTracking
        //    {
        //        TaskAssignId = taskAssignId,
        //        StartedAt = DateTime.UtcNow
        //    };

        //    await _propvivoContext.TaskTimeTrackings.AddAsync(newTaskTime);
        //    await _propvivoContext.SaveChangesAsync();

        //    return Ok(new { success = true, message = "Break ended, task resumed." });
        //}

        //[HttpPost("EndTask/{taskAssignId}")]
        //public async Task<IActionResult> EndTask(int taskAssignId)
        //{
        //    var activeTaskTime = await _propvivoContext.TaskTimeTrackings
        //        .Where(t => t.TaskAssignId == taskAssignId && t.EndedAt == null)
        //        .OrderByDescending(t => t.StartedAt)
        //        .FirstOrDefaultAsync();

        //    if (activeTaskTime == null)
        //    {
        //        return BadRequest("No active task session found to end.");
        //    }

        //    activeTaskTime.EndedAt = DateTime.UtcNow;

        //    var timeSpent = activeTaskTime.EndedAt.Value - activeTaskTime.StartedAt;

        //    activeTaskTime.TotalTime = timeSpent;

        //    // Optional, but good practice
        //    _propvivoContext.TaskTimeTrackings.Update(activeTaskTime);

        //    await _propvivoContext.SaveChangesAsync();

        //    return Ok(new { success = true, message = "Task ended successfully." });
        //}


        //[HttpGet("GetTotalTimeWorkedByEmployee/{userId}")]
        //public async Task<IActionResult> GetTotalTimeWorkedByEmployee(int userId)
        //{
        //    var totalTime = await (from tt in _propvivoContext.TaskTimeTrackings
        //                           join ta in _propvivoContext.TaskAssignments
        //                               on tt.TaskAssignId equals ta.TaskAssignId
        //                           where ta.UserId == userId && tt.TotalTime != null
        //                           select tt.TotalTime.Value).ToListAsync();

        //    var totalWorkedTime = totalTime.Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

        //    return Ok(new
        //    {
        //        success = true,
        //        userId = userId,
        //        totalWorked = totalWorkedTime.ToString(@"hh\:mm\:ss")
        //    });
        //}

        //[HttpGet("CheckDailyWorkStatus/{userId}/{date}")]
        //public async Task<IActionResult> CheckDailyWorkStatus(int userId, DateTime date)
        //{
        //    // Get all sessions on the given date
        //    var totalTime = await (
        //        from tt in _propvivoContext.TaskTimeTrackings
        //        join ta in _propvivoContext.TaskAssignments
        //            on tt.TaskAssignId equals ta.TaskAssignId
        //        where ta.UserId == userId
        //              && tt.StartedAt.Date == date.Date
        //              && tt.TotalTime != null
        //        select tt.TotalTime.Value
        //    ).ToListAsync();

        //    // Sum up the total worked time
        //    var totalWorkedTime = totalTime.Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));
        //    var isCompleted = totalWorkedTime.TotalHours >= 8;

        //    return Ok(new
        //    {
        //        success = true,
        //        date = date.ToString("yyyy-MM-dd"),
        //        totalWorked = totalWorkedTime.ToString(@"hh\:mm\:ss"),
        //        isEightHoursCompleted = isCompleted
        //    });
        //}

        //[HttpGet("WorkHoursSummary/{userId}")]
        //public async Task<IActionResult> GetWorkHoursSummary(int userId)
        //{
        //    var logs = await _propvivoContext.TaskTimeTrackings
        //        .Where(t => t.TaskAssign.UserId == userId && t.TotalTime != null)
        //        .Select(t => new
        //        {
        //            t.TaskAssignId,
        //            TaskTitle = t.TaskAssign.Task.TaskTitle,
        //            Date = t.StartedAt.Date,
        //            Duration = t.TotalTime.Value
        //        })
        //        .ToListAsync();

        //    var grouped = logs
        //        .GroupBy(x => new { x.TaskAssignId, x.TaskTitle, x.Date })
        //        .Select(g => new
        //        {
        //            g.Key.TaskAssignId,
        //            g.Key.TaskTitle,
        //            Date = g.Key.Date.ToString("yyyy-MM-dd"),
        //            TotalHours = Math.Round(g.Sum(x => x.Duration.TotalHours), 2)
        //        });

        //    return Ok(grouped);
        //}


        //[HttpGet("DailyWorkStatus/{userId}")]
        //public async Task<IActionResult> GetDailyWorkStatus(int userId)
        //{
        //    var logs = await _propvivoContext.TaskTimeTrackings
        //        .Where(t => t.TaskAssign.UserId == userId && t.TotalTime != null)
        //        .Select(t => new
        //        {
        //            Date = t.StartedAt.Date,
        //            Duration = t.TotalTime.Value
        //        })
        //        .ToListAsync();

        //    var result = logs
        //        .GroupBy(x => x.Date)
        //        .Select(g => new
        //        {
        //            Date = g.Key.ToString("yyyy-MM-dd"),
        //            TotalHours = Math.Round(g.Sum(x => x.Duration.TotalHours), 2),
        //            IsComplete = g.Sum(x => x.Duration.TotalHours) >= 8
        //        });

        //    return Ok(result);
        //}

        //Employee API
        [HttpGet("getAlltaskByEmployee")]
        public async Task<IActionResult> getAllTaskByEmployee()
        {
            var usertoken = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(usertoken))
            {
                return Unauthorized(new { message = "Invalid token or missing UserId." });
            }
            int userid = int.Parse(usertoken);
            var task = await _propvivoContext.TaskAssignments.Where(ta => ta.UserId == userid).Include(t => t.Task).ThenInclude(t => t.CreatedByNavigation).
                Select(ta => new
                {

                    ta.TaskAssignId,
                    ta.TaskId,
                    ta.Task.TaskTitle,
                    ta.Task.Description,
                    ta.Task.EstimatedHrs,
                    CreatedAt = ta.Task.CreatedAt,
                    CreatedByName = ta.Task.CreatedByNavigation.Name,
                    Status = ta.Status,
                    AssignedAt = ta.AssignedAt
                }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Tasks fetched successfully.",
                data = task
            });
        }

        [HttpPost("starttimerforfirsttime")]
        public async Task<IActionResult> startTaskTimer([FromBody] AddTaskAssignTimer timerData)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Invalid token" });

            int userId = int.Parse(userIdClaim);

            var activeUserTask = await _propvivoContext.TaskTimeTrackings.Include(t => t.TaskAssign).Where(u => u.TaskAssign.UserId == userId & u.EndedAt == null).FirstOrDefaultAsync();
            int taskAssignId = timerData.TaskAssignId;
            if (activeUserTask != null)
            {
                return BadRequest(new { message = "Another task timer is already running. Please stop it before starting a new one." });
            }
            var starttimer = new TaskTimeTracking
            {
                TaskAssignId = taskAssignId,
                StartedAt = DateTime.Now,
                EndedAt = null,
                TotalTime = null
            };
            _propvivoContext.TaskTimeTrackings.Add(starttimer);
            await _propvivoContext.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Timer started successfully.",
                taskTimeId = starttimer.TaskTimeId
            });
        }

        [HttpPost("pauseTimer")]
        public async Task<IActionResult> pauseTaskTimer([FromBody] AddTaskAssignTimer timerData)
        {

            var userClaimId = User.FindFirst("UserId").Value;
            if (string.IsNullOrEmpty(userClaimId))
                return Unauthorized(new { message = "Invalid token" });
            int userid = int.Parse(userClaimId);
            using var transaction = await _propvivoContext.Database.BeginTransactionAsync();
            int taskAssignId = timerData.TaskAssignId;
            try
            {
                var activeTimer = await _propvivoContext.TaskTimeTrackings.Include(t => t.TaskAssign).Where(t => t.TaskAssign.UserId == userid && t.EndedAt == null &&
                t.TaskAssignId == taskAssignId).FirstOrDefaultAsync();
                if (activeTimer == null)
                    return NotFound(new { message = "No running task timer found." });

                activeTimer.EndedAt = DateTime.Now;
                var enddate = DateTime.Now;
                TimeSpan duration = (TimeSpan)(enddate - activeTimer.StartedAt);
                activeTimer.TotalTime = duration.ToString(@"hh\:mm\:ss");
                activeTimer.TaskAssign.Status = TaskStatusEnum.Pause.ToString();


                var breakLog = new BreakLogTracking
                {
                    TaskAssignId = taskAssignId,
                    StartedAt = DateTime.Now,
                    EndedAt = null,
                    TotalTime = null
                };
                _propvivoContext.BreakLogTrackings.Add(breakLog);
                await _propvivoContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { success = true, message = "Task paused and break started." });

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Something went wrong.", error = ex.Message });
            }
        }

        [HttpPost("resumetimer")]
        public async Task<IActionResult> resumeBreakTimer([FromBody] AddTaskAssignTimer timerData)
        {
            var userClaimId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userClaimId))
                return Unauthorized(new { message = "Invalid token" });

            int userId = int.Parse(userClaimId);

            var now = DateTime.Now;

            using var transaction = await _propvivoContext.Database.BeginTransactionAsync();
            int taskAssignId = timerData.TaskAssignId;
            try
            {
                var ongoingBreak = await _propvivoContext.BreakLogTrackings
               .Include(b => b.TaskAssign)
               .Where(b => b.TaskAssignId == taskAssignId &&
                           b.TaskAssign.UserId == userId &&
                           b.EndedAt == null)
               .OrderByDescending(b => b.StartedAt)
               .FirstOrDefaultAsync();

                if (ongoingBreak == null)
                {
                    return NotFound(new { message = "No active break found to resume." });

                }
                TimeSpan duration = (TimeSpan)(now - ongoingBreak.StartedAt);

                ongoingBreak.EndedAt = DateTime.Now;
                ongoingBreak.TotalTime = duration.ToString(@"hh\:mm\:ss");

                ongoingBreak.TaskAssign.Status = TaskStatusEnum.pending.ToString();
                var newTimer = new TaskTimeTracking
                {
                    TaskAssignId = taskAssignId,
                    StartedAt = DateTime.Now,
                    EndedAt = null,
                    TotalTime = null
                };
                _propvivoContext.TaskTimeTrackings.Add(newTimer);

                await _propvivoContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new
                {
                    success = true,
                    message = "Break ended and task resumed.",
                    resumedAt = now
                });

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }

        }

        [HttpPut("stopTimer")]
        public async Task<IActionResult> stopTaskTimer([FromBody] AddTaskAssignTimer timerData)
        {
            var userClaimId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userClaimId))
                return Unauthorized(new { message = "Invalid token" });

            int userId = int.Parse(userClaimId);

            using var transaction = await _propvivoContext.Database.BeginTransactionAsync();
            int taskAssignId = timerData.TaskAssignId;
            try
            {
                var activeTimer = await _propvivoContext.TaskTimeTrackings
                    .Include(t => t.TaskAssign)
                    .Where(t => t.TaskAssign.UserId == userId &&
                                t.TaskAssignId == taskAssignId &&
                                t.EndedAt == null)
                    .FirstOrDefaultAsync();

                if (activeTimer == null)
                    return NotFound(new { message = "No running task timer found to stop." });

                activeTimer.EndedAt = DateTime.Now;
                TimeSpan duration = (TimeSpan)(DateTime.Now - activeTimer.StartedAt);
                activeTimer.TotalTime = duration.ToString(@"hh\:mm\:ss");

                activeTimer.TaskAssign.Status = TaskStatusEnum.Completed.ToString();

                await _propvivoContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    message = "Task stopped and marked as completed.",
                    endedAt = DateTime.Now,
                    totalTime = activeTimer.TotalTime
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while stopping the timer.",
                    error = ex.Message
                });
            }
        }

    }
}
