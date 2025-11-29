using System.Linq.Expressions;

namespace EStockManager.Repositories.Interfaces
{
    // T, projede kullandığımız herhangi bir Entity sınıfını (User, Product, Order) temsil eder.
    public interface IGenericRepository<T> where T : class
    {
        // read metotları
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        // create metodu
        Task AddAsync(T entity);

        // delete metodları
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // update metodu
        T Update(T entity);

        // commit (kaydetme) metodu
        Task<int> SaveChangesAsync();
    }
}