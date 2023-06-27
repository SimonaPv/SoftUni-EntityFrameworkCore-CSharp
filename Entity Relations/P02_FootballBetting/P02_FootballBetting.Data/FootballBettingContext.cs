﻿using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data.Common;
using P02_FootballBetting.Data.Models;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public DbSet<Team> Teams { get; set; } = null!;

        public DbSet<Color> Colors { get; set; } = null!;

        public DbSet<Town> Towns { get; set; } = null!;

        public DbSet<Country> Countries { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        public DbSet<Position> Positions { get; set; } = null!;

        public DbSet<PlayerStatistic> PlayersStatistics { get; set; } = null!;

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Bet> Bets { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;


        //Use it when developing the app
        //When we test the app locally on our PC
        public FootballBettingContext()
        {

        }

        //Used by Judge
        //Loading of the DbContext with DI -> in real apps it is useful
        public FootballBettingContext(DbContextOptions options) : base(options)
        {

        }

        //Connection config 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbConfig.connString);
            }
        }

        //Fluent API and Entities config
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(ps => new { ps.GameId, ps.PlayerId });
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity
                .HasOne(t => t.PrimaryKitColor)
                .WithMany(t => t.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.NoAction);
            
                entity
                .HasOne(t => t.SecondaryKitColor)
                .WithMany(t => t.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasOne(g => g.HomeTeam)
                .WithMany(g => g.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.NoAction);

                entity
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}