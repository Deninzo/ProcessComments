using Microsoft.EntityFrameworkCore;
using ProcessComments.Entities;

namespace ProcessComments.Contexts
{
    public class TransactionsContext : DbContext
    {
        private string _connectionString;

        public DbSet<Transaction> Transactions { get; set; }
        
        public DbSet<FileContent> FileContents { get; set; }
        
        public DbSet<TransactionComment> TransactionComments { get; set; }
        
        public DbSet<File> Files { get; set; }
        
        public TransactionsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
