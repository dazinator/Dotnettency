using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dotnettency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sample.Pages.T1
{
    public class IndexModel : PageModel
    {

        public bool IsRestarted { get; set; }

        public void OnGet()
        {

        }

        public async Task OnPost([FromServices]ITenantShellRestarter<Tenant> restarter)
        {
            await restarter.Restart();
            IsRestarted = true;
            this.Redirect("/");
        }
    }
}
