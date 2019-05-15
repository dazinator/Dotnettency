namespace Dotnettency.Middleware
{
    public class MultitenancyMiddlewareOptionsBuilder<TTenant>
    {
        public MultitenancyMiddlewareOptionsBuilder(AppBuilderAdaptorBase app)
        {
            ApplicationBuilder = app;
        }

        public AppBuilderAdaptorBase ApplicationBuilder { get; }


    }
}