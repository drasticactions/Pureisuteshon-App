using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using PlayStation_App.Common;

namespace PlayStation_App.Commands.Messages
{
    public class AttachImageCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            var file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            var stream = await file.OpenAsync(FileAccessMode.Read);
            Locator.ViewModels.MessagesVm.AttachedImage = await ImageToBytes(stream);
            Locator.ViewModels.MessagesVm.IsImageAttached = true;
            Locator.ViewModels.MessagesVm.ImagePath = file.Path;
        }

        private async Task<byte[]> ImageToBytes(IRandomAccessStream sourceStream)
        {
            byte[] imageArray;

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);

            var transform = new BitmapTransform { ScaledWidth = decoder.PixelWidth, ScaledHeight = decoder.PixelHeight };
            PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Rgba8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.DoNotColorManage);

            using (var destinationStream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream);
                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, decoder.PixelWidth,
                    decoder.PixelHeight, 96, 96, pixelData.DetachPixelData());
                await encoder.FlushAsync();

                BitmapDecoder outputDecoder = await BitmapDecoder.CreateAsync(destinationStream);
                await destinationStream.FlushAsync();
                imageArray = (await outputDecoder.GetPixelDataAsync()).DetachPixelData();
            }
            return imageArray;
        }
    }

    

    public class RemoveImageCommand : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            Locator.ViewModels.MessagesVm.IsImageAttached = false;
            Locator.ViewModels.MessagesVm.AttachedImage = null;
        }
    }
}
