using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JetDevs.Api.Models.DbEntities;
using System;

namespace JetDevs.Api.Context
{
    /// <summary>
    /// DB Context
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Creates an instance
        /// </summary>
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Configure ModelBuilder
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DateTime defaultDate = new DateTime(1900, 1, 1);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);
                entity.Property(e => e.CreationDate).HasDefaultValue(defaultDate);
                entity.Property(e => e.UserName).HasMaxLength(80).IsRequired();
                entity.Property(e => e.NormalizedUserName).HasMaxLength(80);
                entity.Property(e => e.Email).HasMaxLength(80);
                entity.Property(e => e.NormalizedEmail).HasMaxLength(80);
                entity.Property(e => e.PasswordHash).HasMaxLength(200);
                entity.Property(e => e.SecurityStamp).HasMaxLength(50);
                entity.Property(e => e.ConcurrencyStamp).HasMaxLength(40);
                entity.Property(e => e.PhoneNumber).HasMaxLength(16);
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);
                entity.Property(e => e.Name).HasMaxLength(40);
                entity.Property(e => e.NormalizedName).HasMaxLength(40);
                entity.Property(e => e.ConcurrencyStamp).HasMaxLength(40);
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(e => e.RoleId).HasMaxLength(40);
                entity.Property(e => e.ClaimType).HasMaxLength(100);
                entity.Property(e => e.ClaimValue).HasMaxLength(200);
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(40);
                entity.Property(e => e.ClaimType).HasMaxLength(100);
                entity.Property(e => e.ClaimValue).HasMaxLength(200);
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(40);
                entity.Property(e => e.LoginProvider).HasMaxLength(40);
                entity.Property(e => e.ProviderKey).HasMaxLength(40);
                entity.Property(e => e.ProviderDisplayName).HasMaxLength(40);
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(40);
                entity.Property(e => e.RoleId).HasMaxLength(40);
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(40);
                entity.Property(e => e.RoleId).HasMaxLength(40);
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(40);
                entity.Property(e => e.LoginProvider).HasMaxLength(40);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Value).HasMaxLength(200);
            });

           


            modelBuilder.Entity<User>()
           .Property(b => b.ModifiedDate)
           .HasDefaultValue(defaultDate);
        }

        /// <summary>
        /// Portal Users
        /// </summary>
        public DbSet<User> PortalUsers { get; set; }

    }
}