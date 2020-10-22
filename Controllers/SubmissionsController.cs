using System.Diagnostics;
using System;
using System.IO;
using System.Linq;    
using System.Threading.Tasks;    
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using website.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController : ControllerBase
    {

        private readonly ILogger<SubmissionsController> _logger;
        private readonly IConfiguration _config;

        public SubmissionsController(ILogger<SubmissionsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        

        [HttpGet]
        public IActionResult Get(DateTime? afterDate = null, int limit = 100, int offset = 0, bool showApproved = true,
            bool showDeleted = true, bool showAwaiting = true, bool showDenied = true, bool ascending = false, bool orderByUpdated = false) {
            if (afterDate is null) { afterDate = DateTime.MinValue; }
            limit = Math.Min(limit, 100);
            StringValues apiKey;
            Boolean validApiKey = false;
            HttpContext.Request.Headers.TryGetValue("ApiKey", out apiKey);
            if (apiKey.Count() > 0) {
                validApiKey = _config.GetSection("ApiKeys").GetChildren().ToArray().Select(c => c.Value).Contains(apiKey.Single());
            }
            if (!validApiKey) {
                // Api Key not valid or missing, use token
                var token = HttpContext.Session.GetString("token");
                if (FAuth.ValidateToken(token)) {
                    var user = FAuth.GetUserByToken(token);
                    var userReg = user.registrations.SingleOrDefault(reg => reg.applicationId.ToString() == FAuth.clientId);
                    var userRoles = (userReg is null || userReg.roles is null) ? new List<string>() : userReg.roles;
                    if (!userRoles.Contains("staff")) { return StatusCode(403); }
                    
                } else {
                    return StatusCode(403);
                }
            }
            using (var db = new DataContext()) {
                var query = from subs in db.Submissions select subs;
                if (!showApproved) { query = query.Where(sub => sub.status != "Approved"); }
                if (!showDeleted)  { query = query.Where(sub => sub.status != "Deleted"); }
                if (!showAwaiting) { query = query.Where(sub => sub.status != "Awaiting Approval"); }
                if (!showDenied)   { query = query.Where(sub => sub.status != "Rejected"); }
                if (orderByUpdated) {
                    query = query.Where(sub => sub.statusUpdated > afterDate);
                } else {
                    query = query.Where(sub => sub.submissionDate > afterDate);
                }
                if (ascending) {
                    query = query.OrderBy(sub => orderByUpdated ? sub.statusUpdated : sub.submissionDate);
                } else {
                    query = query.OrderByDescending(sub => orderByUpdated ? sub.statusUpdated : sub.submissionDate);
                }
                var results = query.Skip(offset).Take(limit);
                var res = new ApiResponse(results.Count(), query.Count(), limit, offset, results.ToList());
                return Ok(res);
            }
        }

        [HttpGet ("{id:guid}")]
        public IActionResult GetUserSubmissions(Guid id, DateTime? afterDate = null, int limit = 100, int offset = 0, bool showApproved = true,
            bool showDeleted = true, bool showAwaiting = true, bool showDenied = true, bool ascending = false, bool orderByUpdated = false) {
            if (afterDate is null) { afterDate = DateTime.MinValue; }
            limit = Math.Min(limit, 100);
            StringValues apiKey;
            Boolean validApiKey = false;
            HttpContext.Request.Headers.TryGetValue("ApiKey", out apiKey);
            if (apiKey.Count() > 0) {
                validApiKey = _config.GetSection("ApiKeys").GetChildren().ToArray().Select(c => c.Value).Contains(apiKey.Single());
            }
            if (!validApiKey) {
                var token = HttpContext.Session.GetString("token");
                if (FAuth.ValidateToken(token)) {
                    var user = FAuth.GetUserByToken(token);
                    var userReg = user.registrations.SingleOrDefault(reg => reg.applicationId.ToString() == FAuth.clientId);
                    var userRoles = (userReg is null || userReg.roles is null) ? new List<string>() : userReg.roles;
                    if (!userRoles.Contains("staff") && user.id != id) { return StatusCode(403); }
                } else {
                    return StatusCode(403);
                }
            }
            using(var db = new DataContext()) {
                var strId = id.ToString();
                var query = from subs in db.Submissions where subs.authorId == id.ToString() select subs;
                if (!showApproved) { query = query.Where(sub => sub.status != "Approved"); }
                if (!showDeleted)  { query = query.Where(sub => sub.status != "Deleted"); }
                if (!showAwaiting) { query = query.Where(sub => sub.status != "Awaiting Approval"); }
                if (!showDenied)   { query = query.Where(sub => sub.status != "Rejected"); }
                if (orderByUpdated) {
                    query = query.Where(sub => sub.statusUpdated > afterDate);
                } else {
                    query = query.Where(sub => sub.submissionDate > afterDate);
                }
                if (ascending) {
                    query = query.OrderBy(sub => orderByUpdated ? sub.statusUpdated : sub.submissionDate);
                } else {
                    query = query.OrderByDescending(sub => orderByUpdated ? sub.statusUpdated : sub.submissionDate);
                }
                var results = query.Skip(offset).Take(limit);
                var res = new ApiResponse(results.Count(), query.Count(), limit, offset, results.ToList());
                return Ok(res);
            }
        }
    }
}
