using IT.WebHost.Core.Models;
using IT.WebHost.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.App.Controllers
{
    public class MerchController : Controller
    {
        private readonly SiteSettingsService _siteSettings;

        public MerchController(SiteSettingsService siteSettings)
        {
            _siteSettings = siteSettings;
        }

        [HttpGet("/merch")]
        public IActionResult Index()
        {
            ViewData["Title"] = $"Merch - {_siteSettings.SiteTitle}";
            return View(new MerchViewModel());
        }
    }
}
