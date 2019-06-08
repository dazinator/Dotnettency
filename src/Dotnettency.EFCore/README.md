
## Who is this for?

This is for people who want to use EFCore and a single database, so store data for multiple tenants.
When using their DbContext to query the database, it will automatically filter any relevant records based on the current Tenant ID.
When inerting new records, it will automatically set the Tenant ID on those records based on the current Tenant ID.

## Show Me

Ok, but you can also refer to the [sample](https://github.com/dazinator/Dotnettency.Samples/tree/aspnetcore20/src/Sample.EFCore.MultitenantDb)

The following assumes you have already defined your `Tenant` class as per the basic sample and you are now adding EF Core to the mix.

Create an Entity:

```csharp
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public List<Post> Posts { get; set; }
    }
```

Notice it doesn't have a TenantId  property. 

Derive your DbContext:

```

    public class SampleMultitenantDbContext : MultitenantDbContext<SampleMultitenantDbContext, Tenant, Guid>
    {

        private const string TenantIdPropertyName = "TenantId";

        public SampleMultitenantDbContext(DbContextOptions<SampleMultitenantDbContext> options, Task<Tenant> tenant) : base(options, tenant)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        protected override Guid GetTenantId(Tenant tenant)
        {
            return tenant.TenantGuid;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            HasTenantIdFilter<Blog>(modelBuilder, TenantIdPropertyName, (b) => EF.Property<Guid>(b, TenantIdPropertyName));
        }
        
    }
```


That's it. Use your `DbContext` has normal.
You will find that when you query blog entities, they will automatically be filtered by the current tenant ID.
You will find that when you insert new blog entities, they will automatically get a TenantID set to the current tenent and you can see this in the database.


## Explain

Most of the magic happens in `OnModelCreating` when you call `HasTenantIdFilter<Blog>` method from the base class.
This get's EF to configure:

1. A shadow property for the entity to hold the TenantId value.
2. A global query filter for that entity. This means whenever you query this entity it will automatically apply a filter based on the current tenant ID.

`MultitenantDbContext` doesn't stop there. It also records the fact that `Blog` is an entity that requires a TenantId.
When you call `SaveChanges` on your DbContext, it will look for any new `Blog` entities to be inserted, and will automatically set the shadow property TenantId to the
current Tenant Id value before they are insterted into the database.

Notice that you have to override `GetTenantId()`. This is the actual value that will be stored in the database, and used in filter queries.
You are supplied with the current dotnettency `Tenant` so you can use whatver properties of your Tenant class to decide what Id value should be returned.

