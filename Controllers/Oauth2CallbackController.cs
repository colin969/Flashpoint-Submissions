using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace website.Controllers
{
    class TokenRequest {
      string client_id { get; set; }
      string client_secret { get; set; }
      string code { get; set; }
      string grant_type { get; set; }

    }

    [Controller]
    [Route("[controller]")]
    public class Oauth2CallbackController : ControllerBase
    {
        private static readonly string baseUrl = Environment.GetEnvironmentVariable("WEBSITE_BASE_URL");

        private static readonly HttpClient client = new HttpClient();

        private readonly ILogger<Oauth2CallbackController> _logger;

        public Oauth2CallbackController(ILogger<Oauth2CallbackController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RedirectResult Get([FromQuery (Name="code")] string code)
        {
          var token = FAuth.GetToken(code);
          HttpContext.Session.SetString("token", token.access_token);
          return Redirect(String.Format("{0}/dashboard", baseUrl));
        }
    }
}
