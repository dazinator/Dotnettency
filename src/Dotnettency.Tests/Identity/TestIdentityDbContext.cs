using Dotnettency.EFCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Dotnettency.Tests
{
    public class TestIdentityDbContext : MultitenantIdentityDbContext<TestIdentityDbContext, Tenant, int?, User, Role, int, UserClaim, UserRole, UserExternalLogin, RoleClaim, UserToken>
    {

        private const string TenantIdPropertyName = "TenantId";


        public TestIdentityDbContext(DbContextOptions<TestIdentityDbContext> options, Task<Tenant> tenant)
       : base(options, tenant)
        {
        }           

        protected override int? GetTenantId(Tenant tenant)
        {
            return tenant?.Id;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            HasTenantIdFilter<User>(builder, TenantIdPropertyName, (b) => EF.Property<int?>(b, TenantIdPropertyName));
            HasTenantIdFilter<Role>(builder, TenantIdPropertyName, (b) => EF.Property<int?>(b, TenantIdPropertyName));

            builder.Entity<User>(b =>
            {
                // b.ToTable("User");
                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasMany<UserClaim>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();

                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();

                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);
                b.HasIndex(u => u.NormalizedEmail).HasName("UserEmailIndex").IsUnique();

                b.Property(u => u.PasswordHash).HasMaxLength(256);
                b.Property(u => u.SecurityStamp).HasMaxLength(100);
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken().HasMaxLength(36);

                b.Property(u => u.PhoneNumber).HasMaxLength(15);

            });

            builder.Entity<Role>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
                //  b.ToTable("Role");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken().HasMaxLength(36);

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<RoleClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.Property(rc => rc.ClaimType).HasMaxLength(100);
                b.Property(rc => rc.ClaimValue).HasMaxLength(256);
                // b.ToTable("AspNetRoleClaims");
            });

            builder.Entity<UserClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.Property(rc => rc.ClaimType).HasMaxLength(100);
                b.Property(rc => rc.ClaimValue).HasMaxLength(256);
                // b.ToTable("AspNetRoleClaims");
            });

            builder.Entity<UserRole>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                //  b.ToTable("AspNetUserRoles");
            });

            ////This will singularize all table names
            //// ef core 2.2
            //foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
            //{
            //    entityType.Relational().TableName = entityType.DisplayName();
            //}

            ////  ef core 3.1.0
            foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.DisplayName());
            }

        }

    }





}
