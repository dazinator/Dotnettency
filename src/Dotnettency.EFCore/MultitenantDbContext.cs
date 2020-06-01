using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnettency.EFCore
{

    public abstract class MultitenantDbContext<TDbContext, TTenant, TIdType> : DbContext, IMultitenantDbContext<TIdType>
        where TDbContext : DbContext, IMultitenantDbContext<TIdType>
        where TTenant : class
    {
        private readonly Task<TTenant> _tenant;
        private Lazy<TIdType> _tenantId;

        public TIdType TenantId
        {
            get
            {
                return _tenantId.Value;
            }
        }

        public Guid InstanceId { get; set; } = Guid.NewGuid();

        private static List<Action<IMultitenantDbContext<TIdType>>> _setTenantIdOnSaveCallbacks = new List<Action<IMultitenantDbContext<TIdType>>>();

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

            Action<IMultitenantDbContext<TIdType>> action = (db) =>
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
           // SetTenantIdOnSave();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetTenantIdOnSave();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private static void SetTenantIdProperty(string propertyName, EntityEntry entity, TIdType id)
        {
            entity.Property(propertyName).CurrentValue = id;
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