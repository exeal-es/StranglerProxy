using System.Threading.Tasks;

namespace NetProxy
{
    public interface INetProxySender
    {
        Task<T> Send<T>();
    }
}
