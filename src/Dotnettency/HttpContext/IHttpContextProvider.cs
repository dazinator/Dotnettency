namespace Dotnettency
{
    public interface IHttpContextProvider
    {
        HttpContextBase GetCurrent();
    }
}
