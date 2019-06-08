using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnettency.EFCore
{
    public abstract class MultitenantDbContext<TDbContext, TTenant, TIdType> : DbContext
        where TDbContext : DbContext
        where TTenant : class
    {
        private readonly Task<TTenant> _tenant;
        private Lazy<TIdType> _tenantId;

        private TIdType TenantId
        {
            get
            {
                return _tenantId.Value;
            }
        }

        private static List<Action<DbContext>> _setTenantIdOnSaveCallbacks = new List<Action<DbContext>>();

        public MultitenantDbContext(DbContextOptions<TDbContext> options, Task<TTenant> tenant) : base(options)
        {
            _tenant = tenant;
            _tenantId = new Lazy<TIdType>(() =>
            {
                var t = _tenant.Result;
                return GetTenantId(t);
            });
        }

        protected virtual TIdType GetTenantId(TTenant tenant)
        {
            return default(TIdType);
        }


        protected void HasTenantIdFilter<T>(ModelBuilder modelBuilder, string tenantIdPropertyName, Expression<Func<T, TIdType>> idExpression)
          where T : class
        {

            modelBuilder.Entity<T>().Property<TIdType>(tenantIdPropertyName);

            var newExp = Expression.Lambda<Func<T, bool>>(
                    Expression.Equal(idExpression.Body, Expression.Property(Expression.Constant(this),
                    typeof(MultitenantDbContext<TDbContext, TTenant, TIdType>),
                    nameof(TenantId))), idExpression.Parameters);

            modelBuilder.Entity<T>().HasQueryFilter(newExp);

            Action<DbContext> action = (db) =>
            {
                SetTenantIdProperty<T>(tenantIdPropertyName, TenantId, db);
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SetTenantIdOnSave();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetTenantIdOnSave();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private static void SetTenantIdProperty<TEntity>(string propertyName, TIdType id, DbContext db)
            where TEntity : class
        {
            foreach (var item in db.ChangeTracker.Entries<TEntity>())
            {
                if (item.State == EntityState.Added)
                {
                    item.Property(propertyName).CurrentValue = id;
                }
            }
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