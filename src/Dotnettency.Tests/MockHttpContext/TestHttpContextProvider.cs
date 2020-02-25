namespace Dotnettency.Tests
{
    public class TestHttpContextProvider : IHttpContextProvider
    {
        public TestHttpContextProvider(HttpContextBase httpContextBase)
        {
            CurrentHttpContext = httpContextBase;
        }

        public HttpContextBase CurrentHttpContext { get; set; }

        public HttpContextBase GetCurrent()
        {
            return CurrentHttpContext;

        }       


    }
}




