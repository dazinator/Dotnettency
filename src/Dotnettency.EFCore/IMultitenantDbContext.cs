using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dotnettency.EFCore
{
    public interface IMultitenantDbContext<TTenantIdType>
    {
        // Task<TTenant> Tenant { get;  }
        TTenantIdType TenantId { get; }

        ChangeTracker ChangeTracker { get; }
    }
}