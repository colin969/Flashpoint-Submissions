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
            String authUri = String.Format("{0}/authorize?client_id={1}&response_type=code&redirect_uri={2}", FAuth.oauthUri, FAuth.clientId, FAuth.redirectUri);
            return Redirect(authUri);
        }
    }
}
