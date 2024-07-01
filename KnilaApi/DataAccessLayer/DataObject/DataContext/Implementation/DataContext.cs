using KnilaApi.DataAccessLayer.DataObject.DataContext.Interface;
using KnilaApi.DataAccessLayer.DataObject.Entity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace KnilaApi.DataAccessLayer.DataObject.DataContext.Implementation
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> option) : base(option) { }

        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<ContactEntity> Contact { get; set; }

    }
}
