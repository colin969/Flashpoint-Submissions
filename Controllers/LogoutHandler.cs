using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace website.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class LogoutController : ControllerBase
    {

        private readonly ILogger<LogoutController> _logger;

        public LogoutController(ILogger<LogoutController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RedirectResult Get()
        {
            HttpContext.Session.SetString("token", "");
            return Redirect(String.Format("https://login.submissions-dev.xyz/oauth2/logout?client_id={0}&post_logout_redirect_uri={1}", FAuth.clientId, FAuth.logoutRedirectUri));
        }
    }
}
