using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using io.fusionauth.domain;


namespace website.Controllers
{

    public class FlashpointUser {
      public string id { get; set; }
      public string username { get; set; }
      public string email { get; set; }
      public string imageUrl { get; set; }
      public List<string> roles { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post(FlashpointUser fpUser) {
          var token = HttpContext.Session.GetString("token");
          if (token != "") {
            var valid = FAuth.ValidateToken(token);
            if (!valid) {
              return Redirect("https://submissions-dev.xyz/login");
            }
            var user = FAuth.GetUserByToken(token);
            if (user.email != fpUser.email) { return StatusCode(403); }
            var userReg = user.registrations.Single(reg => reg.applicationId.ToString() == FAuth.clientId);
            userReg.username = fpUser.username;
          }
          return StatusCode(403);
        }

        [HttpGet ("{id:guid}")]
        public IActionResult Get(Guid id) {
          var token = HttpContext.Session.GetString("token");
          if (token != "") {
            var valid = FAuth.ValidateToken(token);
            if (!valid) {
              return Redirect("https://submissions-dev.xyz/login");
            }
            var user = FAuth.GetUserById(id);
            var userReg = user.registrations.Single(reg => reg.applicationId.ToString() == FAuth.clientId);
            var fpUser = new FlashpointUser();
            var reg = userReg.username;
            fpUser.id = user.id.ToString();
            fpUser.email = user.email ?? user.username;
            fpUser.username = (reg ?? fpUser.email).ToString();
            fpUser.imageUrl = user.imageUrl;
            fpUser.roles = userReg.roles is null ? new List<string>() : userReg.roles;
            return Ok(fpUser);
          }
          return StatusCode(403);
        }

        [HttpGet]
        public IActionResult GetSelf()
        {
          var token = HttpContext.Session.GetString("token");
          if (token != "") {
            var valid = FAuth.ValidateToken(token);
            if (!valid) {
              return Redirect("https://submissions-dev.xyz/login");
            }
            var user = FAuth.GetUserByToken(token);
            var userReg = user.registrations.Single(reg => reg.applicationId.ToString() == FAuth.clientId);
            var fpUser = new FlashpointUser();
            var reg = userReg.username;
            fpUser.id = user.id.ToString();
            fpUser.email = user.email ?? user.username;
            fpUser.username = (reg ?? fpUser.email).ToString();
            fpUser.imageUrl = user.imageUrl;
            fpUser.roles = userReg.roles is null ? new List<string>() : userReg.roles;
            return Ok(fpUser);
          } else {
            return StatusCode(403);
          }
        }
    }
}
