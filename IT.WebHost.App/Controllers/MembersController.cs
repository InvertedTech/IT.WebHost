using IT.WebHost.Core.Clients;
using IT.WebHost.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.App.Controllers
{
    public class MembersController : Controller
    {
        private readonly ContentClient _contentClient;
        private readonly SiteSettingsService _siteSettingsService;

        public MembersController(ContentClient contentClient, SiteSettingsService siteSettingsService)
        {
            _contentClient = contentClient;
            _siteSettingsService = siteSettingsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
