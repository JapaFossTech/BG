using BG.Repo.EF;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using BG.Model.Core;

namespace BG.Cli;

class Program
{
    static IConfigurationBuilder _ConfigurationBuilder = default!;
    static IConfigurationRoot _ConfigurationRoot = default!;
    static DbContextOptionsBuilder<BGDbContext> _DbContextOptionsBuilder = default!;

    static void BuildOptions()
    {
        _ConfigurationBuilder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile(
                                        "appsettings.json",
                                        optional: true,
                                        reloadOnChange: true);
        _ConfigurationRoot = _ConfigurationBuilder.Build();
        _DbContextOptionsBuilder = new DbContextOptionsBuilder<BGDbContext>();
        _DbContextOptionsBuilder.UseSqlServer(
                                _ConfigurationRoot.GetConnectionString("BGConnString"));
    }
    static void ApplyMigrations()
    {
        using var db = new BGDbContext(_DbContextOptionsBuilder.Options);
        db.Database.Migrate();
    }
    static async Task Import()
    {
        using var db = new BGDbContext(_DbContextOptionsBuilder.Options);

        #region SETUP

        // SETUP
        var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
        {
            HasHeaderRecord = true,
            Delimiter = ";",
        };

        string dataDir = @"C:\Dev\Prj\Net8\BG\BG.Api\Data";
        using var reader = new StreamReader(
            System.IO.Path.Combine(dataDir, "bgg_dataset.csv"));
        using var csv = new CsvReader(reader, config);

        var existingBoardGames = await db.BoardGames
                                .ToDictionaryAsync(bg => bg.BoardGameID);
        Dictionary<string, Domain> existingDomains = await db.Domains
                                .ToDictionaryAsync(d => d.DomainDesc!);
        Dictionary<string, Mechanic> existingMechanics = await db.Mechanics
                                .ToDictionaryAsync(m => m.MechanicDesc!);

        var now = DateTime.Now;

        #endregion

        #region EXECUTE

        // EXECUTE
        var records = csv.GetRecords<BggRecord>();
        var skippedRows = 0;
        foreach (var record in records)
        {
            if (!record.ID.HasValue
                || string.IsNullOrEmpty(record.Name)
                || existingBoardGames.ContainsKey(record.ID.Value))
            {
                skippedRows++;
                continue;
            }

            var boardgame = new BoardGame()
            {
                BoardGameID = record.ID.Value,
                Name = record.Name,
                BGGRank = record.BGGRank ?? 0,
                ComplexityAverage = record.ComplexityAverage ?? 0,
                MaxPlayers = record.MaxPlayers ?? 0,
                MinAge = record.MinAge ?? 0,
                MinPlayers = record.MinPlayers ?? 0,
                OwnedUsers = record.OwnedUsers ?? 0,
                PlayTime = record.PlayTime ?? 0,
                RatingAverage = record.RatingAverage ?? 0,
                UsersRated = record.UsersRated ?? 0,
                Year = record.YearPublished ?? 0
                //,CreatedDate = now
                //,LastModifiedDate = now
            };
            db.BoardGames.Add(boardgame);

            if (!string.IsNullOrEmpty(record.Domains))
                foreach (var domainName in record.Domains
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    var domain = existingDomains.GetValueOrDefault(domainName);
                    if (domain == null)
                    {
                        domain = new Domain()
                        {
                            DomainDesc = domainName
                            //CreatedDate = now,
                            //LastModifiedDate = now
                        };
                        db.Domains.Add(domain);
                        existingDomains.Add(domainName, domain);
                    }
                    db.BoardGamesDomains.Add(new BoardGamesDomain()
                    {
                        BoardGame = boardgame,
                        Domain = domain,
                        //CreatedDate = now
                    });
                }

            if (!string.IsNullOrEmpty(record.Mechanics))
                foreach (var mechanicName in record.Mechanics
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    var mechanic = existingMechanics.GetValueOrDefault(mechanicName);
                    if (mechanic == null)
                    {
                        mechanic = new Mechanic()
                        {
                            MechanicDesc = mechanicName
                            //CreatedDate = now,
                            //LastModifiedDate = now
                        };
                        db.Mechanics.Add(mechanic);
                        existingMechanics.Add(mechanicName, mechanic);
                    }
                    db.BoardGamesMechanics.Add(new BoardGamesMechanic()
                    {
                        BoardGame = boardgame,
                        Mechanic = mechanic,
                        //CreatedDate = now
                    });
                }
        }

        #endregion

        #region SAVE
        
        // SAVE
        using var transaction = db.Database.BeginTransaction();
        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGames ON");
        await db.SaveChangesAsync();
        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGames OFF");
        transaction.Commit();

        #endregion

        Console.WriteLine($"BoardGames count: {db.BoardGames.Count()}");
        Console.WriteLine($"Domains count: {db.Domains.Count()}");
        Console.WriteLine($"Mechanics count: {db.Mechanics.Count()}");
        Console.WriteLine($"skippedRows: {skippedRows}");
    }

    static async Task Main(string[] args)
    {
        BuildOptions();
        ApplyMigrations();
        //await Import();
        //ExecuteCustomSeedData();
        await Task.CompletedTask;
    }

}
