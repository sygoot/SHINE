namespace Models.Services.Database
{
    public interface IChangesListenable<T>
    {
        public abstract IObservable<T> ListenForChanges(long entityId);
    }
}
