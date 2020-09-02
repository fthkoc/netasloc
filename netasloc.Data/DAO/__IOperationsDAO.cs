using System.Collections.Generic;

namespace netasloc.Data.DAO
{
    public interface __IOperationsDAO<T>
    {
        T GetByID(uint id);
        IEnumerable<T> GetAll();
        bool Create(T item);
        bool Update(uint id, T item);
        bool Delete(uint id);
    }
}
