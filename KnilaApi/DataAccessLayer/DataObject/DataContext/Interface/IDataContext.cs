using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;
using KnilaApi.DataAccessLayer.DataObject.Entity;

namespace KnilaApi.DataAccessLayer.DataObject.DataContext.Interface
{
    public interface IDataContext
    {
        public IDbConnection Connection { get; }
        DatabaseFacade Database { get; }
        public DbSet<ContactEntity> Contact { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellation);
    }
}
