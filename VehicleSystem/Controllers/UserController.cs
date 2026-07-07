using Microsoft.AspNetCore.Mvc;
using VehicleSystem.Data;
using VehicleSystem.models;
//using VehicleSystem.Models;

namespace VehicleSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserHelper _userHelper = new UserHelper();
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        // POST: api/User/Register
        // Qof bannaan (public) ma isticmaali karo tan. Waxaa loo baahan yahay
        // header "X-Admin-Key" oo ku dhigan qiimaha "AdminApiKey" ee appsettings.json.
        // Isticmaal Postman: ku dar header X-Admin-Key = <qiimaha appsettings.json>.
        [HttpPost("Register")]
        public IActionResult Register([FromBody] User user, [FromHeader(Name = "X-Admin-Key")] string adminKey)
        {
            var expectedKey = _config["AdminApiKey"];

            if (string.IsNullOrEmpty(expectedKey) || adminKey != expectedKey)
            {
                return Unauthorized(new Response
                {
                    Status = false,
                    Message = "Diiwaangelinta cusub waa xaddidan tahay - fadlan la xiriir maamulaha nidaamka."
                });
            }

            var result = _userHelper.RegisterUser(user);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // POST: api/User/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] User user)
        {
            var result = _userHelper.LoginUser(user);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // GET: api/User/Get?id=0
        // Xaddidan tahay - u baahan X-Admin-Key (isla habka Register)
        [HttpGet("Get")]
        public IActionResult Get([FromQuery] int id = 0, [FromHeader(Name = "X-Admin-Key")] string adminKey = null)
        {
            if (!IsAuthorized(adminKey, out var unauthorizedResult))
                return unauthorizedResult;

            var result = _userHelper.GetUsers(id);
            return result.Status ? Ok(result) : NotFound(result);
        }

        // PUT: api/User/Update
        // Xaddidan tahay - u baahan X-Admin-Key
        [HttpPut("Update")]
        public IActionResult Update([FromBody] User user, [FromHeader(Name = "X-Admin-Key")] string adminKey = null)
        {
            if (!IsAuthorized(adminKey, out var unauthorizedResult))
                return unauthorizedResult;

            var result = _userHelper.UpdateUser(user);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // DELETE: api/User/Delete/5
        // Xaddidan tahay - u baahan X-Admin-Key
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id, [FromHeader(Name = "X-Admin-Key")] string adminKey = null)
        {
            if (!IsAuthorized(adminKey, out var unauthorizedResult))
                return unauthorizedResult;

            var result = _userHelper.DeleteUser(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // ---------- Shared admin-key check ----------
        private bool IsAuthorized(string adminKey, out IActionResult unauthorizedResult)
        {
            var expectedKey = _config["AdminApiKey"];

            if (string.IsNullOrEmpty(expectedKey) || adminKey != expectedKey)
            {
                unauthorizedResult = Unauthorized(new Response
                {
                    Status = false,
                    Message = "Falkan waa xaddidan yahay - fadlan la xiriir maamulaha nidaamka."
                });
                return false;
            }

            unauthorizedResult = null;
            return true;
        }
    }
}
