namespace Models.Services.Database.Tables
{
    public interface IEntity
    {
        public long? Id { get; }
        public IEntity CloneWithId(long? id);
    }
}
