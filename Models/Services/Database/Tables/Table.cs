

using System.Reactive;

namespace Models.Services.Database.Tables
{
    public abstract class Table<T> : ICRUD<T> where T : class, IEntity
    {
        public abstract IObservable<long> Add(T entity);
        public abstract IObservable<Unit> Delete(T entity);
        public abstract IObservable<Unit> Delete(long entityId);
        public abstract IObservable<IEnumerable<T>> GetAll();
        public abstract IObservable<T> GetById(long entityId);
        public abstract IObservable<Unit> Update(T entity);
    }
}
