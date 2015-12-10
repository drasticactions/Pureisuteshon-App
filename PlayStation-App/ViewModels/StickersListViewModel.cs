using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Common;
using PlayStation_App.Database;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.StickerPresetPackage;
using PlayStation_App.Tools;
using PlayStation_App.Tools.Debug;

namespace PlayStation_App.ViewModels
{
    public class StickersListViewModel : NotifierBase
    {
        public ObservableCollection<StickerResponse> StickerList { get; set; } = new ObservableCollection<StickerResponse>();

        private readonly StickerManager _stickerManager = new StickerManager();

        private readonly StickersDatabase _stickersDatabase = new StickersDatabase(); 
        public async Task GetStickerPacks()
        {
            IsLoading = true;
            try
            {
                var stickerList = await _stickersDatabase.GetStickers();
                if (stickerList.Any())
                {
                    StickerList = stickerList.ToObservableCollection();
                    IsLoading = false;
                    return;
                }

                var initialResult =
                    await
                        _stickerManager.GetStickerPackList(Locator.ViewModels.MainPageVm.CurrentUser.Region);
                var resultCheck = await ResultChecker.CheckSuccess(initialResult);
                if (!resultCheck)
                {
                    return;
                }
                var packageList = JsonConvert.DeserializeObject<StickerPresetPackageListResponse>(initialResult.ResultJson);
                foreach (var item in packageList.PresetPackageList)
                {
                    var manifestResult = await _stickerManager.GetStickerAndManifestPack(item.ManifestUrl);
                    resultCheck = await ResultChecker.CheckSuccess(manifestResult);
                    if (!resultCheck)
                    {
                        return;
                    }

                    var manifest = JsonConvert.DeserializeObject<StickerResponse>(manifestResult.ResultJson);
                    
                    var metadataResponse = await _stickerManager.GetStickerAndManifestPack(manifest.MetadataUrl);
                    resultCheck = await ResultChecker.CheckSuccess(metadataResponse);
                    if (!resultCheck)
                    {
                        return;
                    }

                    var metadata = JsonConvert.DeserializeObject<MetadataResponse>(metadataResponse.ResultJson);
                    manifest.Copyright = metadata.Copyright;
                    manifest.Description = metadata.Description;
                    manifest.Publisher = metadata.Publisher;
                    manifest.Title = metadata.Title;
                    stickerList.Add(manifest);
                }

                StickerList = stickerList.ToObservableCollection();
                await _stickersDatabase.InsertStickers(stickerList);
            }
            catch (Exception ex)
            {
                // TODO: Throw error to user.
            }
            IsLoading = false;
        }
    }
}
