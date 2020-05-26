using System.Threading.Tasks;

namespace Swaggelot
{
    public interface ISwaggerTransformer
    {
        Task<string> Transform();
    }
}