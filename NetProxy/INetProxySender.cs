using System.Threading.Tasks;

namespace NetFwkProxy
{
    public interface INetProxySender
    {
        Task<T> Send<T>();
    }
}
