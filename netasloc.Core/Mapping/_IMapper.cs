using netasloc.Core.DTO;
using netasloc.Data.Entity;

namespace netasloc.Core.Mapping
{
    public interface _IMapper<K,V> where K : _IAuditable where V : _IDTO
    {
        K CoreToData(V item);
        V DataToCore(K item);
    }
}
