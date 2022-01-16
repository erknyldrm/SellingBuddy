namespace OrderService.Domain.SeedWork
{
    public interface IRepository<T>
    {
        IUnitOfWork UnitOfWork { get; } 
    }
}
