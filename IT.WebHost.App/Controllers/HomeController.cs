using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IT.WebHost.App.Models;
using IT.WebHost.Core.Clients;
using IT.WebHost.Core.Services;
using IT.WebServices.Fragments.Content;

namespace IT.WebHost.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContentClient _contentClient;
        private readonly SiteSettingsService _siteSettings;

        public HomeController(ContentClient contentClient, SiteSettingsService siteSettings)
        {
            _contentClient = contentClient;
            _siteSettings = siteSettings;
        }

        public async Task<IActionResult> Index()
        {
            var req = new GetAllContentRequest
            {
                // TODO: Remove offset
                PageOffset = 0,
                PageSize = 10,
            };

            var res = await _contentClient.GetAllContentAsync(req);
            var contentListRecords = new List<ContentListRecord>();

            if (res.PageTotalItems > 0)
            {
                contentListRecords.AddRange(res.Records);
            }

            var model = new HomeViewModel
            {
                ContentListRecords = contentListRecords,
                Layout = _siteSettings.Settings?.CMS?.DefaultLayout ?? LayoutEnum.List,
                NextPageOffset = res.PageOffsetEnd,
                PageSize = req.PageSize,
                TotalItems = res.PageTotalItems
            };

            ViewData["Title"] = $"Home - {_siteSettings.SiteTitle}";
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}