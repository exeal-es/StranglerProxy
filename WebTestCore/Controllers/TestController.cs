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
    }
}
