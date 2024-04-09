using DiplomeApi.DataRepository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Transactions;

namespace DiplomeApi
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public DbSet<IpAddress> IpAddresses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
