using Nancy;
//using AppFunc = Func<IDictionary<string, object>, Task>;

namespace Dotnettency.Modules.Nancy
{
    public class TestNancyModule : NancyModule
    {



        public TestNancyModule()
        {
            Get("/greet/{name}", x =>
            {
                return string.Concat("Hello ", x.name);
            });


            //  IApplicationBuilder appBuilder = null;

            //foreach (var module in NancyModules)
            //{
            //    // Create an owin bridge for each owin module.
            //    module.OwinBridge = appBuilder.UseOwin(addToPipeline =>
            //    {
            //        addToPipeline(next =>
            //        {

            //            var owinAppBuilder = new AppBuilder();
            //            owinAppBuilder.Properties["builder.DefaultApp"] = next;
            //            // Uses the owin middleware.
            //            owinAppBuilder.UseNancy((options) => { options.Bootstrapper = new CustomBootstrapper(); });


            //            return owinAppBuilder.Build<AppFunc>();

            //        });

            //    }).Build();



            //    module.OwinPipeline = appBuilder.UseOwin(pipeline =>
            //    {
            //        pipeline(next =>
            //        {
            //            // do something before
            //            app.UseNancy();
            //            // do something after
            //        });
            //    });
            //}
        }
    }
}
