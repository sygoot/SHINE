using HealthApp.Models;

namespace HealthApp.Services.Database.Tables
{
    public sealed class UserTable : ICRUD<User>
    {
        public void Add(User entity) => throw new NotImplementedException();
        public void Delete(User entity) => throw new NotImplementedException();
        public User Update(User entity) => throw new NotImplementedException();
        User ICRUD<User>.Get<T2>() => throw new NotImplementedException();
        IEnumerable<T2> ICRUD<User>.GetAll<T2>() => throw new NotImplementedException();
    }
}
