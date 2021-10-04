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
            return Ok(new { DestinationController = true });
        }

        [Route("test2/{argument}")]
        [HttpGet]
        public IActionResult Test2(string argument)
        {
            return Ok(new { DestinationController = true, Argument = argument });
        }

        [Route("test3")]
        [HttpPost]
        public IActionResult Test3(object dto)
        {
            return Ok(new { DestinationController = true, Argument = dto });
        }

        [Route("test5")]
        [HttpPut]
        public IActionResult Test5(object dto)
        {
            return Ok(new { DestinationController = true, Argument = dto });
        }

        [Route("test7/{argument}")]
        [HttpDelete]
        public IActionResult Test7(string argument)
        {
            return Ok(new { DestinationController = true, Argument = argument });
        }

        [Route("test9")]
        [HttpGet]
        public IActionResult Test9()
        {
            return Ok(new
            {
                DestinationController = true,
                Authorization = this.Request.Headers["Authorization"]
            });
        }


        [Route("test10")]
        [HttpGet]
        public IActionResult Test10()
        {
            return Ok(new
            {
                DestinationController = true,
                QueryString = this.Request.QueryString.Value
            });
        }
    }
}