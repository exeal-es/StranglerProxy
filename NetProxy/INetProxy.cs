using System.Web.Http.Controllers;

namespace NetFwkProxy
{
    public interface INetProxy
    {
        INetProxySender Fordward(HttpActionContext context);
    }
}
