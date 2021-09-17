using System.Threading.Tasks;
using System.Web.Http;

namespace WebTest.Controllers
{
    public class TestController : ApiController
    {
        [Route("test")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var l = new NetProxy.NetProxy(new System.Uri("https://localhost:44391/"));

            var result = await l.Fordward(this.ActionContext)
                   .Send<DTO>();

            return this.Ok(result);
        }
    }

    public class DTO
    {
        public string Name { get; set; }
    }
}
