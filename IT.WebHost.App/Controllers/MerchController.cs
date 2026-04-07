using IT.WebHost.Core.Models;
using IT.WebHost.Core.Services;
using IT.WebServices.Fragments.Merch;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.App.Controllers
{
    public class MerchController : Controller
    {
        private readonly SiteSettingsService _siteSettings;
        private readonly MerchInterface.MerchInterfaceClient _merchClient;

        public MerchController(SiteSettingsService siteSettings, MerchInterface.MerchInterfaceClient merchCllient)
        {
            _siteSettings = siteSettings;
            _merchClient = merchCllient;
        }

        [HttpGet("/merch")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = $"Merch - {_siteSettings.SiteTitle}";
            var req = new SearchMerchRequest
            {
                Query = Request.Query["query"].ToString() ?? "",
                PageSize = uint.TryParse(Request.Query["pageSize"], out var ps) ? ps : 10,
                PageOffset = uint.TryParse(Request.Query["pageOffset"], out var po) ? po : 0,
                InternalStoreId = Request.Query["internalStoreId"].ToString() ?? "",
                Tag = Request.Query["tag"].ToString() ?? "",
                OnlyInStock = bool.TryParse(Request.Query["onlyInStock"], out var ois) && ois
            };
            var res = await _merchClient.SearchMerchAsync(req);

            return View(new MerchViewModel()
            {
                Query = req.Query,
                PageSize = req.PageSize.ToString(),
                PageOffset = res.PageOffsetEnd.ToString(),
                PageTotalItems = res.PageTotalItems,
                Tag = req.Tag,
                OnlyInStock = req.OnlyInStock.ToString(),
                Records = res.Records.ToList()
            });
        }
    }
}
