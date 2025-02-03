namespace Models.Services.Database.Tables
{
    public interface ITargetTable : ICRUD<Target>, IChangesListenable<Target>
    {
        public IObservable<Target?> FindTargetByDate(DateTime date);
    }
}
