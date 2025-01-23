using System.Reactive;

namespace Models.Services.Database.Tables
{
    public interface IUserTable
    {
        public IObservable<Unit> Add(User entity);
        public IObservable<Unit> Delete();
        public IObservable<Models.User> Get();
        public IObservable<Unit> Update(User entity);
    }
}
