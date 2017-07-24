# Dotnettency
Mutlitenancy library for dotnet applications.

Heavily inspired by [saaskit](https://github.com/saaskit/saaskit)

See the sample app for usage.

Let's you inject `ITenantAccessor<TTenant>` in order to lazily access the current tenant (which could be null).

Let's you inject `ITenantShellAccessor<TTenant>` in order to access a wrapper around the tenant, which is primarily used by:

- Extensions (such as Middleware, or Container) - which cache thigns for the tenant in the tenants concurrent property bag.
- Tenant Admin screens - if you need to "Restart" a tenant, then the idea is extension methods on the shell can be exposed to allow you to rebuild the tenants container, or rebuild the tenant's pipeline on the next request etc.
