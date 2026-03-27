using IT.WebHost.Core.Clients;
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

        [HttpGet("{id:guid}")]
        public IActionResult ById(Guid id)
        {
            // Forward to the existing Blazor Post route
            return Redirect($"/post/{id}");
        }
    }
}
