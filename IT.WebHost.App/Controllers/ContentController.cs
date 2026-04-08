using IT.WebHost.App.Models;
using IT.WebHost.Core.Models;
using IT.WebHost.Core.Services;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Comment;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.App.Controllers
{
    public class ContentController : Controller
    {
        private readonly ContentInterface.ContentInterfaceClient _contentClient;
        private readonly CommentInterface.CommentInterfaceClient _commentClient;
        private readonly SiteSettingsService _siteSettingsService;
        private readonly ONUserHelper userHelper;
        public ContentController(
            ContentInterface.ContentInterfaceClient contentClient,
            CommentInterface.CommentInterfaceClient commentClient,
            SiteSettingsService siteSettingsService,
            ONUserHelper userHelper)
        {
            _contentClient = contentClient;
            _commentClient = commentClient;
            _siteSettingsService = siteSettingsService;
            this.userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/content/{**slug}")]
        public async Task<IActionResult> ViewContent(string slug)
        {
            // TODO: Figure out unauthenticated members slugs
            var req = new GetContentByUrlRequest()
            {
                ContentUrl = "/" + slug
            };

            // TODO: Make these safer
            var res = await _contentClient.GetContentByUrlAsync(req, userHelper.GetGrpcCallOptions());
            return View(new ViewContentViewModel { Slug = slug, Record = res.Record });
        }

        [HttpPost("/content/{contentId}/comment")]
        public async Task<IActionResult> MakeCommentForContent(string contentId, [FromBody] string text)
        {
            var req = new CreateCommentForContentRequest()
            {
                ContentID = contentId,
                Text = text
            };

            var res = await _commentClient.CreateCommentForContentAsync(req, userHelper.GetGrpcCallOptions());
            return View();
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
        public async Task<IActionResult> MembersArea()
        {
            var req = new GetAllContentRequest
            {
                PageOffset = 0,
                PageSize = 10,
                SubscriptionSearch = new SubscriptionLevelSearch
                {
                    MinimumLevel = 10,
                    MaximumLevel = 9999,
                }
            };

            var res = await _contentClient.GetAllContentAsync(req, userHelper.GetGrpcCallOptions());
            var records = new List<ContentListRecord>();

            if (res.PageTotalItems > 0)
                records.AddRange(res.Records);

            var model = new HomeViewModel
            {
                ContentListRecords = records,
                Layout = _siteSettingsService.Settings?.CMS?.DefaultLayout ?? LayoutEnum.List,
                NextPageOffset = res.PageOffsetEnd,
                PageSize = req.PageSize,
                TotalItems = res.PageTotalItems,
            };

            return View("~/Views/Content/MembersArea.cshtml", model);
        }
    }
}
