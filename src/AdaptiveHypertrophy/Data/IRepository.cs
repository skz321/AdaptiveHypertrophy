using System.Collections.Generic;

namespace AdaptiveHypertrophy.Data
{
    public interface IRepository<T>
    {
        void Save(T entity);

        List<T> GetAll();
    }
}