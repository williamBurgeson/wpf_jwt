using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models;

namespace web_app.Controllers
{
    [ApiController]
    public class LoginController : Controller
    {
        [AllowAnonymous]
        [HttpPost, Route("")]
        public IActionResult Authenticate([FromBody] LoginModel loginModel)
        {
            return View();
        }
    }
}