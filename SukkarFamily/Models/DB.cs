using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SukkarFamily.Models;

namespace SukkarFamily.Models
{
    public class DB:DbContext
    {

        public DB(DbContextOptions options)
            : base(options)
        {

        }

   
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Data Source =.; Initial Catalog = SukkarFamily; Integrated Security = True");
        //}
        public DbSet<Persone> persones { set; get; }

   
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Data Source =.; Initial Catalog = SukkarFamily; Integrated Security = True");
        //}
        public DbSet<SukkarFamily.Models.News> News { get; set; }
    }
}
