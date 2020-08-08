using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebMVC.ViewComponents
{
    public class RegisterUrlViewComponent : ViewComponent
    {
        private readonly string _identityRegisterUrl;

        public RegisterUrlViewComponent(IOptions<AppSettings> settings)
        {
            _identityRegisterUrl = settings.Value.IdentityRegisterUrl;
        }

        public string Invoke()
        {
            return _identityRegisterUrl + "?returnUrl=" + HttpContext.Request.GetEncodedUrl();
        }
    }
}
