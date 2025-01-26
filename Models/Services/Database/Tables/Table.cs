

using System.Reactive;

namespace Models.Services.Database.Tables
{
    public abstract class Table<T> : ICRUD<T>, IChangesListenable<T> where T : Entity
    {
        public abstract IObservable<long> Add(T entity);
        public abstract IObservable<Unit> Delete(T entity);
        public abstract IObservable<Unit> Delete(long entityId);
        public abstract IObservable<Unit> DeleteAll();
        public abstract IObservable<IEnumerable<T>> GetAll();
        public abstract IObservable<T> GetById(long entityId);
        public abstract IObservable<Unit> Update(T entity);

        public abstract IObservable<T> ListenForChanges(long entityId);
    }
}
