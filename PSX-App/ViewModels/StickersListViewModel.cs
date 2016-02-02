using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Commands.Messages;
using PlayStation_App.Common;
using PlayStation_App.Database;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.Sticker;
using PlayStation_App.Models.StickerPresetPackage;
using PlayStation_App.Tools;
using PlayStation_App.Tools.Debug;

namespace PlayStation_App.ViewModels
{
    public class StickersListViewModel : NotifierBase
    {
        public ObservableCollection<StickerResponse> StickerList { get; set; } = new ObservableCollection<StickerResponse>();

        private ObservableCollection<StickerSelection> _stickerCollection;

        public ObservableCollection<StickerSelection> StickerCollection
        {
            get { return _stickerCollection; }
            set
            {
                SetProperty(ref _stickerCollection, value);
                OnPropertyChanged();
            }
        }

        public SelectStickerCommand SelectStickerCommand { get; set; } = new SelectStickerCommand();

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
                    foreach (var item in stickerList)
                    {
                        StickerList.Add(item);
                    }
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
                    manifest.ManifestUrl = item.ManifestUrl;
                    stickerList.Add(manifest);
                }

                foreach (var item in stickerList)
                {
                    StickerList.Add(item);
                }
                await _stickersDatabase.InsertStickers(stickerList);
            }
            catch (Exception ex)
            {
                // TODO: Throw error to user.
            }
            IsLoading = false;
        }

        public async Task GetStickers(StickerResponse stickerPack)
        {
            IsLoading = true;
            try
            {
                var manifestResult = await _stickerManager.GetStickerAndManifestPack(stickerPack.ManifestUrl);
                var resultCheck = await ResultChecker.CheckSuccess(manifestResult);
                if (!resultCheck)
                {
                    return;
                }
                var manifest = JsonConvert.DeserializeObject<StickerResponse>(manifestResult.ResultJson);
                var newList = SizetypeConverter.ConvertStringToSizeProperty(manifestResult.ResultJson);
                if (newList.Any())
                {
                    StickerCollection = new ObservableCollection<StickerSelection>();
                    var stickerUrls = newList.First().Urls.ToList();
                    for (int index = 0; index < stickerUrls.Count; index++)
                    {
                        StickerCollection.Add(new StickerSelection()
                        {
                            Number = index + 1,
                            ManifestUrl = stickerPack.ManifestUrl,
                            PackageId = stickerPack.StickerPackageId,
                            Type = stickerPack.Type,
                            ImageUrl = stickerUrls[index]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Throw error to user.
            }
            IsLoading = false;

        }
    }
}
