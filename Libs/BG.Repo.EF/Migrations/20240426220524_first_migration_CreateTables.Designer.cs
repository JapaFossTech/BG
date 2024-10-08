﻿// <auto-generated />
using BG.Repo.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BG.Repo.EF.Migrations
{
    [DbContext(typeof(BGDbContext))]
    [Migration("20240426220524_first_migration_CreateTables")]
    partial class first_migration_CreateTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BG.Model.Core.BoardGame", b =>
                {
                    b.Property<int>("BoardGameID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BoardGameID"));

                    b.Property<int>("BGGRank")
                        .HasColumnType("int");

                    b.Property<double>("ComplexityAverage")
                        .HasColumnType("float");

                    b.Property<int>("MaxPlayers")
                        .HasColumnType("int");

                    b.Property<int>("MinAge")
                        .HasColumnType("int");

                    b.Property<int>("MinPlayers")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(127)
                        .HasColumnType("nvarchar(127)");

                    b.Property<int>("OwnedUsers")
                        .HasColumnType("int");

                    b.Property<int>("PlayTime")
                        .HasColumnType("int");

                    b.Property<double>("RatingAverage")
                        .HasColumnType("float");

                    b.Property<int>("UsersRated")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("BoardGameID");

                    b.ToTable("BoardGames");
                });

            modelBuilder.Entity("BG.Model.Core.BoardGamesDomain", b =>
                {
                    b.Property<int>("BoardGamesDomainID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BoardGamesDomainID"));

                    b.Property<int>("BoardGameID")
                        .HasColumnType("int");

                    b.Property<int>("DomainID")
                        .HasColumnType("int");

                    b.HasKey("BoardGamesDomainID");

                    b.HasIndex("BoardGameID");

                    b.HasIndex("DomainID");

                    b.ToTable("BoardGamesDomains");
                });

            modelBuilder.Entity("BG.Model.Core.BoardGamesMechanic", b =>
                {
                    b.Property<int>("BoardGamesMechanicID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BoardGamesMechanicID"));

                    b.Property<int>("BoardGameID")
                        .HasColumnType("int");

                    b.Property<int>("MechanicID")
                        .HasColumnType("int");

                    b.HasKey("BoardGamesMechanicID");

                    b.HasIndex("BoardGameID");

                    b.HasIndex("MechanicID");

                    b.ToTable("BoardGamesMechanics");
                });

            modelBuilder.Entity("BG.Model.Core.Domain", b =>
                {
                    b.Property<int>("DomainID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DomainID"));

                    b.Property<string>("DomainDesc")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("DomainID");

                    b.HasIndex("DomainDesc")
                        .IsUnique();

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("DomainDesc"), false);

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("BG.Model.Core.Mechanic", b =>
                {
                    b.Property<int>("MechanicID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MechanicID"));

                    b.Property<string>("MechanicDesc")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("MechanicID");

                    b.HasIndex("MechanicDesc")
                        .IsUnique();

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("MechanicDesc"), false);

                    b.ToTable("Mechanics");
                });

            modelBuilder.Entity("BG.Model.Core.BoardGamesDomain", b =>
                {
                    b.HasOne("BG.Model.Core.BoardGame", "BoardGame")
                        .WithMany("BoardGamesDomains")
                        .HasForeignKey("BoardGameID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BG.Model.Core.Domain", "Domain")
                        .WithMany("BoardGamesDomains")
                        .HasForeignKey("DomainID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BoardGame");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("BG.Model.Core.BoardGamesMechanic", b =>
                {
                    b.HasOne("BG.Model.Core.BoardGame", "BoardGame")
                        .WithMany("BoardGamesMechanics")
                        .HasForeignKey("BoardGameID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BG.Model.Core.Mechanic", "Mechanic")
                        .WithMany("BoardGamesMechanics")
                        .HasForeignKey("MechanicID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BoardGame");

                    b.Navigation("Mechanic");
                });

            modelBuilder.Entity("BG.Model.Core.BoardGame", b =>
                {
                    b.Navigation("BoardGamesDomains");

                    b.Navigation("BoardGamesMechanics");
                });

            modelBuilder.Entity("BG.Model.Core.Domain", b =>
                {
                    b.Navigation("BoardGamesDomains");
                });

            modelBuilder.Entity("BG.Model.Core.Mechanic", b =>
                {
                    b.Navigation("BoardGamesMechanics");
                });
#pragma warning restore 612, 618
        }
    }
}
