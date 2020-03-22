using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Dotnettency.Tests
{
    public class MultitenantDbContextTests
    {
        [Fact]
        public async Task CanUseMultitenantIdentityDbContext()
        {
            var services = new ServiceCollection();
            string tenantToReturn = "http://foo.com";

            services.AddDbContext<TestIdentityDbContext>((options) =>
            {
                options.UseInMemoryDatabase(nameof(MultitenantDbContextTests));
            });

            var identityBuilder = services.AddIdentityServices<User, Role>()
                     .AddEntityFrameworkStores<TestIdentityDbContext>()
                    // .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                    .AddDefaultTokenProviders();

            services.AddMultiTenancy<Tenant>((multitenancyOptions) =>
            {
                multitenancyOptions
                .AddAspNetCore()
                .SetMockHttpContextProvider(()=>new Uri(tenantToReturn))
                .Map<int>((builder) =>
                {
                    builder.SelectRequestHost()
                    .WithMapping((tenants) =>
                    {
                        tenants.Add(1, "foo.com")
                               .Add(2, "bar.com");
                    })
                    .UsingDotNetGlobPatternMatching();
                })
                 .Get(key =>
                 {
                     return Task.FromResult(new Tenant() { Id = key });                     
                 });
            });




            // .AddJwtRefreshTokenDataProtectionProvider<User>(Configuration);
            //services.add

            // Create role for tenant  1.
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var role = new Role() { Name = "Foo", NormalizedName = "Foo" };
                var result = await roleManager.CreateAsync(role);

                var newRole = await roleManager.Roles.FirstOrDefaultAsync(a => a.Name == "Foo");
                Assert.NotNull(newRole);
            }

            // switch tenant
            tenantToReturn = "http://bar.com";
            using (var scope = sp.CreateScope())
            {
                // role should not exist for this tenant
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                var newRole = await roleManager.Roles.FirstOrDefaultAsync(a => a.Name == "Foo");
                Assert.Null(newRole);

                // Create role with the same name but for this tenant - should succeed.
                var role = new Role() { Name = "Foo", NormalizedName = "Foo" };
                var result = await roleManager.CreateAsync(role);

                newRole = await roleManager.Roles.FirstOrDefaultAsync(a => a.Name == "Foo");
                Assert.NotNull(newRole);
            }

        }

    }


    public class Role : IdentityRole<int>
    {
        public Role()
        {
            Claims = new HashSet<RoleClaim>();
        }

        public ICollection<RoleClaim> Claims { get; set; }
    }

    public class RoleClaim : IdentityRoleClaim<int>
    {
        // public AppPermission AppPermission { get; set; }
        // public int? AppPermissionId { get; set; }
    }

    public class User : IdentityUser<int>
    {

    }

    public class UserClaim : IdentityUserClaim<int>
    {
        //  public AppPermission AppPermission { get; set; }
        //  public int? AppPermissionId { get; set; }
    }

    public class UserExternalLogin : IdentityUserLogin<int>
    {

    }

    public class UserRole : IdentityUserRole<int>
    {

    }

    public class UserToken : IdentityUserToken<int>
    {

    }





}
