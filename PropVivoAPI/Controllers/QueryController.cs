using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropVivoAPI.DTO.AddDataDTO;
using PropVivoAPI.DTO.UpdateDataDTO;
using PropVivoAPI.Models;
using PropVivoAPI.Services;

namespace PropVivoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly PropvivoContext _propvivoContext;
        private readonly IFileService _fileService;
        public QueryController(PropvivoContext propvivoContext,IFileService fileService)
        {
            _propvivoContext = propvivoContext;
            _fileService = fileService;
        }
        //Employee Query api
        [HttpPost("addQuery")]
        public async Task<IActionResult> addQuery([FromForm] AddQueryDTO queryDTO)
        {
            try
            {
                string uploadfile = null;
                if (queryDTO.IssueAttachment != null)
                {
                    uploadfile = await _fileService.uploadFile(queryDTO.IssueAttachment, "uploadattachments");
                }
                else
                {
                    uploadfile = null;
                }
                var query = new QueryMaster
                {
                    TaskAssignId = queryDTO.TaskAssignId,
                    Subject = queryDTO.Subject,
                    Description = queryDTO.Description,
                    IssueAttachment = uploadfile,
                    Status = "Pending", // Set default
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null
                };

                _propvivoContext.QueryMasters.Add(query);
                await _propvivoContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Query added successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to add query.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("getQueryByEmployeeTask/{taskAssignId:int}")]
        public async Task<IActionResult> getQueryByEmployee(int taskAssignId)
        {
            try
            {
                var userClaimId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userClaimId))
                    return Unauthorized(new { message = "Invalid token" });

                int userId = int.Parse(userClaimId);
                var fetchQuery = await (from q in _propvivoContext.QueryMasters
                                        join t in _propvivoContext.TaskAssignments on q.TaskAssignId equals t.TaskAssignId
                                        where t.UserId == userId && q.TaskAssignId == taskAssignId
                                        select new
                                        {
                                            q.QueryId,
                                            q.Subject,
                                            q.TaskAssignId,
                                            q.Description,
                                            q.Status,
                                            attachment = !string.IsNullOrEmpty(q.IssueAttachment) ? $"{Request.Scheme}://{Request.Host}/uploadattachments/{q.IssueAttachment}" : null,
                                            q.CreatedAt,
                                            response=(from re in _propvivoContext.QueryResponses
                                                      where re.QueryResponseId==q.QueryId
                                                      select new{
                                                          re.Message,
                                                          re.CreatedAt
                                                      }).ToList()
                                        }).ToListAsync();
                if (fetchQuery == null || fetchQuery.Count == 0)
                    return NotFound(new { message = "No queries found for this task" });

                return Ok(fetchQuery);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });

            }
        }
        ////superior query api
        [HttpGet("getTaskByQuery/{taskid:int}")]
        public async Task<IActionResult> getTaskByQuery(int taskid)
        {
            try
            {
                var queryList = await (from q in _propvivoContext.QueryMasters
                                       join t in _propvivoContext.TaskAssignments
                                            on q.TaskAssignId equals t.TaskAssignId
                                       join u in _propvivoContext.UserMasters
                                            on t.UserId equals u.UserId
                                       where t.TaskId == taskid
                                       select new
                                       {
                                           q.QueryId,
                                           q.Subject,
                                           q.Description,
                                           q.Status,
                                           q.CreatedAt,
                                           attachment = !string.IsNullOrEmpty(q.IssueAttachment) ? $"{Request.Scheme}://{Request.Host}/uploadattachments/{q.IssueAttachment}" : null,

                                           t.TaskId,
                                           t.Task.TaskTitle,

                                           // User Details
                                           u.UserId,
                                           u.Name,
                                           u.Email
                                       }).ToListAsync();

                if (queryList == null || queryList.Count == 0)
                    return NotFound(new { message = "No queries found for the given task" });

                return Ok(queryList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
        [HttpPost("queryresponse/{queryid:int}")]
        public async Task<IActionResult> addQueryResponse(int queryid, [FromBody] Addresponse dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                {
                    return BadRequest(new { message = "Invalid input" });
                }

                // Optional: validate that the query exists
                var existingQuery = await _propvivoContext.QueryMasters.FindAsync(queryid);
                if (existingQuery == null)
                {
                    return NotFound(new { message = "Query not found" });
                }

                var response = new QueryResponse
                {
                    QueryId = queryid,
                    Message = dto.Message,
                    CreatedAt= DateTime.Now
                };

                _propvivoContext.QueryResponses.Add(response);
                await _propvivoContext.SaveChangesAsync();

                return Ok(new { message = "Response added successfully", responseId = response.QueryResponseId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error while saving response", error = ex.Message });
            }
        }
        [HttpPut("updatestatus/{queryid:int}")]
        public async Task<IActionResult> UpdateQueryStatus(int queryid, [FromBody]UpdateQueryStatusDTO dto)
        {
            try
            {
                var query = await _propvivoContext.QueryMasters.FindAsync(queryid);

                if (query == null)
                    return NotFound(new { message = "Query not found" });

                if (string.IsNullOrWhiteSpace(dto.Status))
                    return BadRequest(new { message = "Status cannot be empty" });

                query.Status = dto.Status;
                query.UpdatedAt = DateTime.Now;

                _propvivoContext.QueryMasters.Update(query);
                await _propvivoContext.SaveChangesAsync();

                return Ok(new { message = "Query status updated successfully", queryId = queryid, newStatus = dto.Status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating status", error = ex.Message });
            }
        }

    }
}
