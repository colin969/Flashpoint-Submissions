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
using MimeMapping;
using SharpCompress.Archives.SevenZip;
using Microsoft.Extensions.Primitives;

namespace website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionController : ControllerBase
    {

        private readonly ILogger<SubmissionController> _logger;
        private readonly IConfiguration _config;

        public SubmissionController(ILogger<SubmissionController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            Directory.CreateDirectory("temp");
            Directory.CreateDirectory(_config["LogoDataPath"]);
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection form)
        {
            var token = HttpContext.Session.GetString("token");
            if (FAuth.ValidateToken(token)) {
                var user = FAuth.GetUserByToken(token);

                var files = form.Files;
                long size = files.Sum(f => f.Length);
                List<Submission> subs = new List<Submission>();
                foreach (var file in files)
                {
                    if (!file.FileName.EndsWith(".7z")) { return StatusCode(400, "Must be a .7z file."); }
                    if (file.Length > 0)
                    {
                        var userDataFolder = Path.Combine(_config["UserDataPath"], user.id.ToString());
                        Directory.CreateDirectory(userDataFolder);
                        var fileName = Sanitization.CoerceValidFileName(file.FileName);
                        var filePath = Path.Combine(userDataFolder, fileName);
                        while (System.IO.File.Exists(filePath)) {
                            fileName = Sanitization.CoerceValidFileName(Path.GetFileNameWithoutExtension(fileName) + "(1)" + Path.GetExtension(fileName));
                            filePath = Path.Combine(userDataFolder, fileName);
                        }
                        using (var db = new DataContext()) {
                            // Save file
                            using (var fs = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fs);
                            }
                            // Create submission
                            var newSubmission = new Submission();
                            newSubmission.fileName = fileName;
                            newSubmission.size = file.Length;
                            newSubmission.authorId = user.id.ToString();
                            newSubmission.submissionDate = DateTime.UtcNow;
                            newSubmission.statusUpdated = newSubmission.submissionDate;
                            newSubmission.status = "Awaiting Approval";
                            // Insert to DB
                            db.Submissions.Add(newSubmission);
                            db.SaveChanges();
                            // Try and extract logo
                            using (Stream stream = System.IO.File.OpenRead(filePath)) {
                                using (var archive = SevenZipArchive.Open(stream)) {
                                    // Meta
                                    try {
                                        var meta = archive.Entries.FirstOrDefault(e => e.Key.ToLower().EndsWith("meta.yaml"));
                                        if (meta.Key.ToLower().EndsWith("meta.yaml")) {
                                            StreamReader metaReader = new StreamReader(meta.OpenEntryStream());
                                            String metaStr = metaReader.ReadToEnd();
                                            Meta m = YamlHelper.LoadMeta(metaStr);
                                            var metaPath = Path.Combine("temp", Path.GetRandomFileName());
                                            m.submission = newSubmission;
                                            db.Meta.Add(m);
                                        }
                                    } catch (Exception e) {
                                        // Let the server print a 500
                                        throw e;
                                    }
                                    // Logo
                                    try {
                                        var logo = archive.Entries.FirstOrDefault(e => e.Key.ToLower().EndsWith("logo.png"));
                                        if (logo.Key.ToLower().EndsWith("logo.png")) {
                                            var logoFileName = String.Format("{0}_{1}.png", newSubmission.id, Path.GetFileNameWithoutExtension(newSubmission.fileName));
                                            var logoPath = Path.Combine(
                                                _config["LogoDataPath"], 
                                                logoFileName
                                            );
                                            var logoUrl = "/logos/" + logoFileName;
                                            using (var entryStream = logo.OpenEntryStream()) {
                                                using (var outputStream = new FileStream(logoPath, FileMode.Create)) {
                                                    await entryStream.CopyToAsync(outputStream);
                                                }
                                            }
                                            newSubmission.logoUrl = logoUrl;
                                        }
                                    } catch (Exception e) {
                                        // Let the server print a 500
                                        throw e;
                                    }
                                }
                            }
                            subs.Add(newSubmission);
                            await db.SaveChangesAsync();
                            newSubmission.meta.submission = null;
                        }
                    }
                }
                return Ok(subs);
            } else {
              return StatusCode(403);
            }
        }

        [HttpGet ("{id:int}/meta")]
        public IActionResult GetMeta(int id) {
            var token = HttpContext.Session.GetString("token");
            if (FAuth.ValidateToken(token)) {
                var user = FAuth.GetUserByToken(token);
                var userReg = user.registrations.SingleOrDefault(reg => reg.applicationId.ToString() == FAuth.clientId);
                var userRoles = (userReg is null || userReg.roles is null) ? new List<string>() : userReg.roles;

                using(var db = new DataContext()) {
                    try {
                        var submission = db.Submissions.Single(s => s.id == id);
                        if (!userRoles.Contains("staff") && user.id.ToString() != submission.authorId) { return StatusCode(403); }
                        if (submission.metaId != null) {
                            var meta = db.Meta.Single(m => m.id == submission.id);
                            meta.submission = null;
                            return Ok(meta);
                        } else {
                            return StatusCode(404);
                        }
                    } catch (InvalidOperationException) {
                        return StatusCode(404);
                    }
                }
            }
            return StatusCode(403);
        }

        [HttpGet ("{id:int}/download")]
        public IActionResult GetDownload(int id) {
            StringValues apiKey;
            Boolean validApiKey = false;
            HttpContext.Request.Headers.TryGetValue("ApiKey", out apiKey);
            if (apiKey.Count() > 0) {
                validApiKey = Environment.GetEnvironmentVariable("WEBSITE_MASTER_API_KEY") == apiKey.Single();
            }
            var token = HttpContext.Session.GetString("token");
            if (validApiKey || FAuth.ValidateToken(token)) {
                using (var db = new DataContext()) {
                    try {
                        var submission = db.Submissions.Single(e => e.id == id);
                        if (!validApiKey) { 
                            var user = FAuth.GetUserByToken(token);
                            var userReg = user.registrations.SingleOrDefault(reg => reg.applicationId.ToString() == FAuth.clientId);
                            var userRoles = (userReg is null || userReg.roles is null) ? new List<string>() : userReg.roles;
                            if (!userRoles.Contains("staff") && user.id.ToString() != submission.authorId) { return StatusCode(403); }
                        }
                        var filePath = Path.Combine(this._config["UserDataPath"], submission.authorId.ToString(), submission.fileName);
                        if (!System.IO.File.Exists(filePath)) { return StatusCode(404); }
                        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                        string contentType = MimeUtility.GetMimeMapping(filePath);
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            // for example foo.bak
                            FileName = submission.fileName, 

                            // always prompt the user for downloading, set to true if you want 
                            // the browser to try to show the file inline
                            Inline = false, 
                        };
                        Response.Headers.Append("Content-Disposition", cd.ToString());
                        return File(fileData, contentType);
                    } catch (InvalidOperationException) {
                        // Will fire if submission not found
                        return StatusCode(404);
                    }
                }
            }
            return StatusCode(403);
        }

        [HttpGet ("{id:int}")]
        public IActionResult Get(int id) {
            var token = HttpContext.Session.GetString("token");
            if (FAuth.ValidateToken(token)) {
                var user = FAuth.GetUserByToken(token);
                var userReg = user.registrations.SingleOrDefault(reg => reg.applicationId.ToString() == FAuth.clientId);
                var userRoles = (userReg is null || userReg.roles is null) ? new List<string>() : userReg.roles;

                using(var db = new DataContext()) {
                    try {
                        var submission = db.Submissions.Single(s => s.id == id);
                        if (!userRoles.Contains("staff") && user.id.ToString() != submission.authorId) { return StatusCode(403); }
                        return Ok(submission);
                    } catch (InvalidOperationException) {
                        return StatusCode(404);
                    }
                }
            }
            return StatusCode(403);
        }

        [HttpPatch ("{id:int}")]
        [HttpPut ("{id:int}")]
        public IActionResult Update(int id, Submission sub) {
            if (id != sub.id) { return StatusCode(400); }
            var token = HttpContext.Session.GetString("token");
            if (FAuth.ValidateToken(token)) {
                var user = FAuth.GetUserByToken(token);
                var userReg = user.registrations.SingleOrDefault(reg => reg.applicationId.ToString() == FAuth.clientId);
                var userRoles = (userReg is null || userReg.roles is null) ? new List<string>() : userReg.roles;

                using(var db = new DataContext()) {
                    try {
                        var submission = db.Submissions.Single(s => s.id == id);
                        if (!userRoles.Contains("staff")) { return StatusCode(403); }
                        db.Entry(submission).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    } catch (InvalidOperationException) {
                        return StatusCode(404);
                    }
                    // Update submission
                    sub.updatedById = user.id.ToString();
                    sub.statusUpdated = DateTime.UtcNow;
                    db.Submissions.Update(sub);
                    db.SaveChanges();
                    return Ok(sub);

                }
            }
            return StatusCode(403);
        }

        [HttpDelete ("{id:int}")]
        public IActionResult Delete(int id) {
            var token = HttpContext.Session.GetString("token");
            if (FAuth.ValidateToken(token)) {
                var user = FAuth.GetUserByToken(token);
                var userReg = user.registrations.SingleOrDefault(reg => reg.applicationId.ToString() == FAuth.clientId);
                var userRoles = (userReg is null || userReg.roles is null) ? new List<string>() : userReg.roles;

                using(var db = new DataContext()) {
                    try {
                        var submission = db.Submissions.Single(s => s.id == id);
                        if (!userRoles.Contains("staff") && user.id.ToString() != submission.authorId) { return StatusCode(403); }
                        if (submission.status == "Approved") { return StatusCode(423); }
                        // Update submission
                        submission.updatedById = user.id.ToString();
                        submission.status = "Deleted";
                        db.SaveChanges();
                        // Delete File
                        var filePath = Path.Combine(this._config["UserDataPath"], submission.authorId.ToString(), submission.fileName);
                        if (System.IO.File.Exists(filePath)) {
                            System.IO.File.Delete(filePath);
                        }
                        // Delete Logo
                        var logoPath = Path.Combine(
                            _config["LogoDataPath"], 
                            String.Format("{0}_{1}.png", submission.id, Path.GetFileNameWithoutExtension(submission.fileName))
                        );
                        if (System.IO.File.Exists(logoPath)) {
                            System.IO.File.Delete(logoPath);
                        }
                        return Ok(submission);
                    } catch (InvalidOperationException) {
                        return StatusCode(404);
                    }
                }
            }
            return StatusCode(403);
        }
    }
}
