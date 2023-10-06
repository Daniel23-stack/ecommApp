using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommApp.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommApp.Controllers
{
    [Route("api/authTest")]
    [ApiController]
    public class AuthOaController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> GetSomething()
        {
            return "You are authenticated";
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles =SD.Role_Admin)]
        public async Task<ActionResult<string>> GetSomething(int someIntValue)
        {
            //authorization -> Authentication + Some access/roles
            return "You are Authorized with Role of Admin";
        }
    }
}
