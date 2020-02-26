using System;

namespace Dotnettency
{
    public class HttpContextValueSelectorFunc : IHttpContextValueSelector
    {
        private readonly Func<HttpContextBase, string> _selectValue;
        public HttpContextValueSelectorFunc(Func<HttpContextBase, string> selectValue)
        {
            _selectValue = selectValue;
        }

        public string SelectValue(HttpContextBase httpContext)
        {
            return _selectValue?.Invoke(httpContext);
        }
    }
}