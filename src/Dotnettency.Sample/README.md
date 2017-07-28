Start this sample running, then browse to:

- http://localhost:5001/Index.html
- http://localhost:5002/Index.html
- http://localhost:5003/Index.html

on ports 5000 and 5001 (Both are tenant BAR) you get this:

![image](https://user-images.githubusercontent.com/3176632/28713866-d240b8c2-7388-11e7-932c-a638cfa7514f.png)

On 5002 (Tenant Foo) you get this: 

![image](https://user-images.githubusercontent.com/3176632/28713886-e702d45c-7388-11e7-85c4-d8d052667a3a.png)

The project structure looks like this - but focus on where Index.html is which demonstrates that the file can be overridden on a per tenant basis (The GUID's are tenant's id's)

![image](https://user-images.githubusercontent.com/3176632/28713742-4b6ef37c-7388-11e7-9214-41988fafdbba.png)

Each tenant gets an `IHostingEnvironment` which has a ContentRootFileProvider and WebRootFileProvider which is a composite IFileProvider encapsulating the host level folder, plus its tenant specific one, ie. for tenant foo, its WebRootFileProvider sees these root directories:-

- wwwroot
- wwwroot/.tenants/049c8cc4-3660-41c7-92f0-85430452be22/

Placing the tenant's root directory i a subfolder that has a period in the folder name i.e `.tenants` ensures its a hidden directory in terms of the first `IFileProvider` looking at `wwwroot`. This means tenants cannot access each others directories virtue of navigating through the first root folder path.

This means tenants can also have files that are only visible for that tenant - they just need to be in the tenant specific folder only.

![image](https://user-images.githubusercontent.com/3176632/28714079-ae16fd02-7389-11e7-8717-ad6c7a8a96ad.png)

This isn't necessarily anything new or exciting in terms of how IFileProviders work. However in terms of how static files middleware works, you can see in the sample, that all I do is add the static files middleware with its vanilla / default configuration (didn't have to configure it's IFileProvider etc) to the tenant middleware pipeline - and now it behaves like this automatically, virtue of PerTenantHostingEnvironment. 

I would expect all other middlewares to behave similar - i.e when added to the per tenant middleware pipeline (in conjunction with using PerTenantHostingEnvironment), they only get a Tenant view of the world. 
I am going to experiment with an MVC sample next to see what that does in terms of things like razor, view components, etc. In theory I think it means all the same concepts should apply - i.e you will be able to override all those things on a per tenant basis too, without having to configure any custom services. This is great if you need
to customise a particular tenants home razor page for example.

The same concept also applies to the "Content" file system.. browse on the various ports (5000-5003) on the default url to see a json result which includes the contents of reading a content file on `/Info.txt` - for some tenants `/Info.txt` resolves to the `tenant isolated` file. For others it resolves to the system level (i.e common) file.

Let me know if you find this interesting or useful!


