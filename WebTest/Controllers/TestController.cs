using System.Threading.Tasks;
using System.Web.Http;
using NetFwkProxy;

namespace WebTest.Controllers
{
    public class TestController : ApiController
    {
        [Route("test")]
        [HttpGet]
        public async Task<IHttpActionResult> Test()
        {
            var l = new NetProxy(new System.Uri("https://localhost:44391/"));

            var result = await l.Fordward(this.ActionContext)
                   .Send<DTO>();

            return this.Ok(result);
        }

        [Route("test2")]
        [HttpGet]
        public IHttpActionResult Test2()
        {
            return this.Ok(new DTO() { Name = "test " });
        }

        [Route("test3/{hello}")]
        [HttpGet]
        public IHttpActionResult Test3(string hello)
        {
            return this.Ok(new DTO() { Name = hello });
        }
    }

    public class DTO
    {
        public string Name { get; set; }
    }
}
