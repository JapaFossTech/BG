
//* To be used at the UI or API
//Microsoft.EntityFrameworkCore.Design
//Microsoft.EntityFrameworkCore.Tools       //* Install at the class lib to create migrations from it

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using BG.Model.Core;
using PrjBase.SecurityBase;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace BG.Repo.EF;

public partial class BGDbContext : IdentityDbContext<AppUser> //DbContext
{
    private static IConfigurationRoot? _configuration;

    public BGDbContext() { }
    public BGDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json"
                                        , optional: true, reloadOnChange: true);

            _configuration = builder.Build();
            var cnstr = _configuration.GetConnectionString("BGConnString");
            optionsBuilder.UseSqlServer(cnstr);

            //ChangeTracker.QueryTrackingBehavior = 
            //                    QueryTrackingBehavior.NoTracking;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Domain Index

        modelBuilder.Entity<Domain>()
            .HasIndex(model => new { model.DomainDesc })
            .IsUnique()
            .IsClustered(false);

        #endregion

        #region Mechanic Index

        modelBuilder.Entity<Mechanic>()
            .HasIndex(model => new { model.MechanicDesc })
            .IsUnique()
            .IsClustered(false);

        #endregion
    }

}

public partial class BGDbContext : IdentityDbContext<AppUser>  //DbContext
{
    public DbSet<BoardGame> BoardGames { get; set; }
    public DbSet<Domain> Domains { get; set; }
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<BoardGamesDomain> BoardGamesDomains { get; set; }
    public DbSet<BoardGamesMechanic> BoardGamesMechanics { get; set; }
}

//* Migrations
//* Package Manager Console. Set 'Default Project' to: BG.Repo.EF
//* Make sure package Microsoft.EntityFrameworkCore.Tools is (Nuget) installed
//* PM> add-migration first_migration_CreateTables
//* From a CLI or any app that provides the connection string
//* PM> update-database or create a Migrator.CLI
//* PM> remove-migration
