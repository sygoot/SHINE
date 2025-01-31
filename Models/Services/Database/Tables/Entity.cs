namespace Models.Services.Database.Tables
{
    public abstract record Entity(long? Id) : IEntity
    {
        public IEntity CloneWithId(long? id) => this with { Id = id };
    }
}
