﻿using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropVivoAPI.Models;
using Humanizer;
using PropVivoAPI.DTO;

namespace PropVivoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly PropvivoContext _propvivoContext;
        public UserController(PropvivoContext propvivoContext)
        {
            _propvivoContext = propvivoContext;
        }

        private async Task<bool> SendEmail(string toEmail, string Name, string plainPassword)
        {
            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplate.html");
                string emailBody = await System.IO.File.ReadAllTextAsync(templatePath);

                emailBody = emailBody
                    .Replace("{{Name}}", Name)
                    .Replace("{{Email}}", toEmail)
                    .Replace("{{Password}}", plainPassword);

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("salipratham033@gmail.com", "zlbd txsz xmso uswe"),
                    EnableSsl = true,
                };

                var mailMsg = new MailMessage
                {
                    From = new MailAddress("salipratham033@gmail.com"),
                    Subject = "Your Login Credentials",
                    Body = emailBody,
                    IsBodyHtml = true,
                };

                mailMsg.To.Add(toEmail);
                await smtp.SendMailAsync(mailMsg);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateSecurePassword(int length = 10)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";

            var random = new Random();
            var chars = new List<char>
    {
        upper[random.Next(upper.Length)],
        special[random.Next(special.Length)],
        digits[random.Next(digits.Length)],
        lower[random.Next(lower.Length)],
    };

            string allChars = lower + upper + digits + special;
            for (int i = chars.Count; i < length; i++)
            {
                chars.Add(allChars[random.Next(allChars.Length)]);
            }

            return new string(chars.OrderBy(x => random.Next()).ToArray());
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO user)
        {
            try
            {
                if (_propvivoContext.UserMasters.Any(fs => fs.Name == user.Name))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "This User already exists."
                    });
                }
                string generatedPassword = GenerateSecurePassword();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);
                var Username = new UserMaster
                {

                    RoleId = user.RoleId,
                    Email = user.Email,
                    Name = user.Name,
                    Password = hashedPassword,

                };

                _propvivoContext.UserMasters.Add(Username);
                await _propvivoContext.SaveChangesAsync();
                await SendEmail(user.Email, user.Name, generatedPassword);
                return Ok(new
                {
                    success = true,
                    message = "User Added Successfully",
                    Department = new
                    {
                        Username.Name,
                        Username.Email,
                        Username.RoleId,
                        Username.UserId,
                    },
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [HttpGet("GetUnassignedEmployees")]
        public async Task<IActionResult> GetUnassignedEmployees()
        {
            var employeesWithoutManager = await _propvivoContext.UserMasters
                .Where(u => u.RoleId == 2 && u.SuperiorId == null) 
                .Select(u => new {
                    u.UserId,
                    u.Name,
                    u.Email
                })
                .ToListAsync();

            return Ok(employeesWithoutManager);
        }

        [HttpGet("GetByManagerId/{id}")]
        public async Task<IActionResult> GetByManagerId(int id)
        {
            var employeesWithoutManager = await _propvivoContext.UserMasters
                .Where(u => u.SuperiorId==id)
                .Select(u => new {
                    u.UserId,
                    u.Name,
                    u.Email
                })
                .ToListAsync();

            return Ok(employeesWithoutManager);
        }


        [HttpPost("assignmanagertoemployee")]
        public async Task<IActionResult> AssignManagerToEmployee([FromBody] AssignManagerDTO assignManager)
        {
            try
            {
                var employee = await _propvivoContext.UserMasters
                .FirstOrDefaultAsync(u => u.UserId == assignManager.EmployeeId && u.RoleId == 2);

                if (employee == null)
                    return NotFound("Employee not found.");

                employee.SuperiorId = assignManager.ManagerId;

                _propvivoContext.UserMasters.Update(employee);
                await _propvivoContext.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Superior assigned successfully.",
                    user = new
                    {
                        employee.UserId,
                        employee.Name,
                        employee.Email,
                        employee.SuperiorId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> getallemployees()
        {
            try {
                var employee = await _propvivoContext.UserMasters.Select(e => new
                {
                    e.UserId,
                    e.Name,
                    e.Email,
                    e.Role.RoleName,
                }).ToListAsync();
                return Ok(new
                {
                    success = true,
                    message = "All Users  fetched successfully",
                    tasks = employee
                });
            } catch (Exception ex) {
                return StatusCode(500, new { success = false, message = ex.Message });

            }

        }

        

    }
}
