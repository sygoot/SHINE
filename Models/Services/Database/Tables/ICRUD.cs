using Models.Services.Database.Tables;
using System.Reactive;

namespace Models.Services.Database
{
    public interface ICRUD<T> where T : class, IEntity
    {
        public IObservable<long> Add(T entity);
        public IObservable<T> GetById(long entityId);
        public IObservable<IEnumerable<T>> GetAll();
        public IObservable<Unit> Update(T entity);
        public IObservable<Unit> Delete(T entity);
        public IObservable<Unit> Delete(long entityId);
    }
}
