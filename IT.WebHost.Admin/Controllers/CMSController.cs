using IT.WebHost.Admin.Models.Cms;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.Admin.Controllers
{
    public class CMSController : Controller
    {
        private readonly ONUserHelper userHelper;

        public CMSController(ONUserHelper userHelper)
        {
            this.userHelper = userHelper;
        }

        [HttpGet("/content")]
        public IActionResult ListContent()
        {
            var vm = new ListContentViewModel()
            {
                Content = new List<ContentListRecord>()
            };
            return View();
        }

        [HttpGet("/assets")]
        public IActionResult ListAssets ()
        {
            return View();
        }
    }
}
