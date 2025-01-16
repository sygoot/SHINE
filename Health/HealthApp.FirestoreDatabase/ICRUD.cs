namespace HealthApp.FirestoreDatabase
{
    public interface ICRUD<T> where T : class
    {
        public void Add(T entity);
        public T Get<T2>() where T2 : T;
        public IEnumerable<T2> GetAll<T2>() where T2 : T;
        public T Update(T entity);
        public void Delete(T entity);

    }
}
