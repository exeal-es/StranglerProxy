using Microsoft.AspNetCore.Mvc;

namespace WebTestCore.Controllers
{
    [ApiController]

    public class TestController : ControllerBase
    {
        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            return this.Ok(new { OverrideController = true });
        }

        [Route("test3/{hello}")]
        [HttpGet]
        public IActionResult Test3(string hello)
        {
            return this.Ok(new { OverrideController22 = true });
        }
    }
}
