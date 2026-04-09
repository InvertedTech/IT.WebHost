using IT.WebHost.Admin.Models.Settings.Cms;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.Admin.Controllers
{
    public class SettingsController : Controller
    {
        private readonly SettingsInterface.SettingsInterfaceClient settingsClient;
        private readonly ONUserHelper userHelper;

        public SettingsController(SettingsInterface.SettingsInterfaceClient settingsClient, ONUserHelper userHelper)
        {
            this.settingsClient = settingsClient;
            this.userHelper = userHelper;
        }

        [HttpGet("/settings/cms")]
        public async Task<IActionResult> CMSSettings()
        {
            return View();
        }

        [HttpGet("/settings/cms/channels")]
        public async Task<IActionResult> CMSSettingsChannels()
        {
            return View(new SettingsCmsChannelsViewModel
            {
                Channels = new List<ChannelRecord>()
            });
        }

        [HttpGet("/settings/merch")]
        public async Task<IActionResult> MerchSettings()
        {
            return View();
        }

        [HttpGet("/settings/personalization")]
        public async Task<IActionResult> PersonalizationSettings()
        {
            return View();
        }

        [HttpGet("/settings/subscription")]
        public async Task<IActionResult> SubscriptionSettings()
        {
            return View();
        }
    }
}
