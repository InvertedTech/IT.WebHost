using IT.WebHost.Core.Clients;
using IT.WebHost.Core.Models;
using IT.WebHost.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.App.Controllers
{
    public class ContentController : Controller
    {
        private readonly ContentClient _contentClient;
        private readonly SiteSettingsService _siteSettingsService;

        public ContentController(ContentClient contentClient, SiteSettingsService siteSettingsService)
        {
            _contentClient = contentClient;
            _siteSettingsService = siteSettingsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/content/{**slug}")]
        public IActionResult ViewContent(string slug)
        {
            return View(new ViewContentViewModel { Slug = slug });
        }

        [HttpGet("/search")]
        public IActionResult Search(string? q)
        {
            return View(new SearchViewModel { Query = q ?? string.Empty });
        }

        [HttpGet("/members-area")]
        public IActionResult MembersArea()
        {
            return View("~/Views/Content/MembersArea.cshtml");
        }
    }
}
