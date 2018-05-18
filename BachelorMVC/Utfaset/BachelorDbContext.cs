using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BachelorMVC.Models;
using Microsoft.EntityFrameworkCore;


namespace BachelorMVC.Persistence
{
    public class BachelorDbContext : DbContext
    {

        public BachelorDbContext(DbContextOptions<BachelorDbContext> options): base(options)
        {   
        }

        public DbSet<Dokument> Dokumenter { get; set; }
        public DbSet<Bruker> Bruker { get; set; }

    }
}
