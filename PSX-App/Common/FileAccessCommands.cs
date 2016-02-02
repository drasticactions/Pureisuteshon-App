using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace PlayStation_App.Common
{
    public class FileAccessCommands
    {
        public static async Task SaveStreamToCameraRoll(Stream streamToSave, string fileName)
        {
            var file = await KnownFolders.CameraRoll.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                const int bufferSize = 1024;
                var buf = new byte[bufferSize];

                var bytesread = 0;
                while ((bytesread = await streamToSave.ReadAsync(buf, 0, bufferSize)) > 0)
                {
                    await fileStream.WriteAsync(buf, 0, bytesread);
                }
            }
        }
    }
}
