using IT.WebHost.Core.Models;
using IT.WebHost.Core.Services;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.App.Controllers
{
    public class ContentController : Controller
    {
        private readonly ContentInterface.ContentInterfaceClient _contentClient;
        private readonly SiteSettingsService _siteSettingsService;

        public ContentController(ContentInterface.ContentInterfaceClient contentClient, SiteSettingsService siteSettingsService)
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
        public IActionResult Search(
            string? q,
            uint? pageSize,
            uint? pageOffset,
            IT.WebServices.Fragments.Content.ContentType? contentType,
            string? categoryId,
            string? channelId,
            string? tag,
            bool? onlyLive)
        {
            return View(new SearchViewModel
            {
                Query = q ?? string.Empty,
                PageSize = pageSize ?? 10,
                PageOffset = pageOffset ?? 0,
                ContentType = contentType ?? IT.WebServices.Fragments.Content.ContentType.ContentNone,
                CategoryId = categoryId ?? string.Empty,
                ChannelId = channelId ?? string.Empty,
                Tags = tag ?? string.Empty,
                OnlyLive = onlyLive ?? false,
                Content = []
            });
        }

        [HttpGet("/members-area")]
        public IActionResult MembersArea()
        {
            return View("~/Views/Content/MembersArea.cshtml");
        }
    }
}
