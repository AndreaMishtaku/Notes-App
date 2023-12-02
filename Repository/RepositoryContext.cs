using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public class RepositoryContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public RepositoryContext(DbContextOptions options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int>
        {
            Id = 1,
            Name = "User",
            NormalizedName = "USER"
        });
    }

    public DbSet<Note>? Note { get; set; }

}
