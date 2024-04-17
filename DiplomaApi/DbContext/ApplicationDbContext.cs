using DiplomaApi.DataRepository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Transactions;

namespace DiplomaApi
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<IpAddress> IpAddresses { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
