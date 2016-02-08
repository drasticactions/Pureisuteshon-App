using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using PlayStation_App.Models.Events;
using PlayStation_App.Models.RecentActivity;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.TrophyDetail;

namespace PlayStation_App.Tools.Debug
{
    public class SampleData
    {
        public static async Task<Feed[]> GetSampleRecentActivityFeed()
        {
            var sampleFile = @"Assets\Sample\RecentActivityFeed.txt";
            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await installationFolder.GetFileAsync(sampleFile);
            var sampleDataText = await FileIO.ReadTextAsync(file);
            var response = JsonConvert.DeserializeObject<RecentActivityResponse>(sampleDataText);
            return response.Feed;
        }

        public static async Task<TrophyTitle[]> GetSampleTrophyFeed()
        {
            var sampleFile = @"Assets\Sample\RecentActivityFeed.txt";
            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await installationFolder.GetFileAsync(sampleFile);
            var sampleDataText = await FileIO.ReadTextAsync(file);
            var response = JsonConvert.DeserializeObject<TrophyDetailResponse>(sampleDataText);
            return response.TrophyTitles;
        }

        public static async Task<Event[]> GetEventFeed()
        {
            var sampleFile = @"Assets\Sample\Events.txt";
            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await installationFolder.GetFileAsync(sampleFile);
            var sampleDataText = await FileIO.ReadTextAsync(file);
            var response = JsonConvert.DeserializeObject<EventsResponse>(sampleDataText);
            return response.Events;
        }
    }
}
