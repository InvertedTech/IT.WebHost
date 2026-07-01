using IT.WebServices.Clients.CMS;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages
{
    public partial class MembersArea
    {
        [Inject] public ContentClient contentClient { get; set; } = null!;
        [Inject] public ChannelHelper channelHelper { get; set; } = null!;
        private Dictionary<string, List<ContentListRecord>> featuredChannels = new Dictionary<string, List<ContentListRecord>>();
        private bool isLoading { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await LoadChannels();
            isLoading = false;
            StateHasChanged();
        }

        private async Task LoadChannels()
        {
            featuredChannels.Clear();
            var channels = channelHelper.GetAll();
            foreach (var channel in channels)
            {
                var records = await LoadChannelFeatured(channel.ChannelId);

                if (records is null) continue;
                featuredChannels.Add(channel.ChannelId, records);
            }
        }

        private async Task<List<ContentListRecord>?> LoadChannelFeatured(string channelId)
        {
            var req = new SearchContentRequest
            {
                SubscriptionSearch = new()
                {
                    MinimumLevel = 1,
                    MaximumLevel = 9999
                },
                ChannelId = channelId,
                PageSize = 5
            };

            var res = await contentClient.Search(req);
            if (res == null || !res.Records.Any())
                return null;

            return res.Records.ToList();
        }
    }
}
