using System.Web.Http;

namespace WebTest2.Controllers
{
    public class TestController : ApiController
    {
        [Route("test")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return this.Ok(new DTO() { Name = "TEST " });
        }
    }

    public class DTO
    {
        public string Name { get; set; }
    }
}
