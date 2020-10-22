using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;


namespace website.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RedirectResult Get()
        {
          return Redirect(String.Format("https://login.submissions-dev.xyz/oauth2/authorize?client_id={0}&response_type=code&redirect_uri={1}", FAuth.clientId, FAuth.redirectUri));
        }
    }
}
