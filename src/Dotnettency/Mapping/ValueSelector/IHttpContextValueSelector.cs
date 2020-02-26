namespace Dotnettency
{
    public interface IHttpContextValueSelector
    {
        string SelectValue(HttpContextBase httpContext);
    }
}