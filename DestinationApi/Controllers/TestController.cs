using Microsoft.AspNetCore.Mvc;

namespace DestinationApi.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            return this.Ok(new {DestinationController = true});
        }

        [Route("test2/{hello}")]
        [HttpGet]
        public IActionResult Test2(string hello)
        {
            return this.Ok(new {DestinationController = true, Argument = hello});
        }

        [Route("test3")]
        [HttpPost]
        public IActionResult Test3(object dto)
        {
            return this.Ok(new {DestinationController = true, Argument = dto});
        }

        [Route("test5")]
        [HttpPut]
        public IActionResult Test5(object dto)
        {
            return this.Ok(new {DestinationController = true, Argument = dto});
        }

        [Route("test7/{argument}")]
        [HttpDelete]
        public IActionResult Test7(string argument)
        {
            return this.Ok(new {DestinationController = true, Argument = argument});
        }
    }
}