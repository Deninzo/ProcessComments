using Microsoft.EntityFrameworkCore;
using ProcessComments.Entities;

namespace ProcessComments.Contexts
{
    public class DataStorageContext : DbContext
    {
        private string _connectionString;

        public DbSet<Data> Data { get; set; }
        
        public DataStorageContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
