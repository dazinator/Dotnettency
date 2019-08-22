using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnettency.EFCore.Identity
{
    public abstract class MultitenantIdentityDbContext<TDbContext, TTenant, TIdType, TUser, TRole, TKey> : MultitenantIdentityDbContext<TDbContext, TTenant, TIdType, TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TDbContext : IMultitenantDbContext<TIdType>
    {

        public MultitenantIdentityDbContext(DbContextOptions<MultitenantIdentityDbContext<TDbContext, TTenant, TIdType, TUser, TRole, TKey>> options, Task<TTenant> tenant)
       : base(options, tenant)
        {

        }

    }



    public abstract class MultitenantIdentityDbContext<TDbContext, TTenant, TTenantIdType, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IMultitenantDbContext<TTenantIdType>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TUserToken : IdentityUserToken<TKey>
    where TDbContext : IMultitenantDbContext<TTenantIdType>
    {

        private readonly Task<TTenant> _tenant;
        private Lazy<TTenantIdType> _tenantId;

        private static List<Action<IMultitenantDbContext<TTenantIdType>>> _setTenantIdOnSaveCallbacks = new List<Action<IMultitenantDbContext<TTenantIdType>>>();


        public MultitenantIdentityDbContext(DbContextOptions options, Task<TTenant> tenant)
       : base(options)
        {
            _tenant = tenant;
            _tenantId = new Lazy<TTenantIdType>(() =>
            {
                var t = _tenant.Result;
                return GetTenantId(t);
            });
        }

        protected abstract TTenantIdType GetTenantId(TTenant tenant);

        public TTenantIdType TenantId
        {
            get
            {
                return _tenantId.Value;
            }
        }

        // public Task<TTenant> Tenant { get => return _tenant; }

        protected void HasTenantIdFilter<T>(ModelBuilder modelBuilder, string tenantIdPropertyName, Expression<Func<T, TTenantIdType>> idExpression)
    where T : class
        {

            modelBuilder.Entity<T>().Property<TTenantIdType>(tenantIdPropertyName);

            var newExp = Expression.Lambda<Func<T, bool>>(
                    Expression.Equal(idExpression.Body, Expression.Property(Expression.Constant(this),
                    typeof(MultitenantIdentityDbContext<TDbContext, TTenant, TTenantIdType, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>),
                    nameof(TenantId))), idExpression.Parameters);

            modelBuilder.Entity<T>().HasQueryFilter(newExp);
            
            Action<IMultitenantDbContext<TTenantIdType>> action = (db) =>
            {
                foreach (var item in db.ChangeTracker.Entries<T>())
                {
                    if (item.State == EntityState.Added)
                    {
                        var id = db.TenantId;
                        SetTenantIdProperty(tenantIdPropertyName, item, id);
                    }
                }
            };
            _setTenantIdOnSaveCallbacks.Add(action);

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetTenantIdOnSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            SetTenantIdOnSave();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetTenantIdOnSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private static void SetTenantIdProperty(string propertyName, EntityEntry entity, TTenantIdType id)
        {
            // var multiTenantDbContext = (MultitenantIdentityDbContext<TDbContext,TTenant,TTenantIdType,)db;
            entity.Property(propertyName).CurrentValue = id;

            //foreach (var item in db.ChangeTracker.Entries<TEntity>())
            //{
            //    if (item.State == EntityState.Added)
            //    {
            //        item.Property(propertyName).CurrentValue = db.TenantId;
            //    }
            //}
        }

        private void SetTenantIdOnSave()
        {
            ChangeTracker.DetectChanges();
            foreach (var item in _setTenantIdOnSaveCallbacks)
            {
                item(this);
            }
        }


    }

}
