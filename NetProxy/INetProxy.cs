using System.Web.Http.Controllers;

namespace NetProxy
{
    public interface INetProxy
    {
        INetProxySender Fordward(HttpActionContext context);
    }
}
