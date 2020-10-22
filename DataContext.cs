using Microsoft.EntityFrameworkCore;
using website.Models;

namespace website {
    public class DataContext : DbContext
    {
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Meta> Meta { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=submissions.db");
    }
}