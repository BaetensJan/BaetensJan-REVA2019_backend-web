namespace ApplicationCore.Interfaces
{
    public interface IJoinEntity<TEntity>
    {
        TEntity Navigation { get; set; }
    }
}