using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
// using website.Migrations;

namespace website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, MigrateDBConfiguration>());
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
