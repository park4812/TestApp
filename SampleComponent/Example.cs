using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.Storage;

using Windows.Foundation.Diagnostics;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SampleComponent
{
    public sealed class Example
    {
        int MyNumber;

        MediaCapture mediaCapture;
        bool isPreviewing;

        LoggingChannel channel;

        public async void Init()
        {
            channel = new LoggingChannel("my provider", null, new Guid("4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a"));

            channel.LogMessage("This is a log message Info", LoggingLevel.Information);
            channel.LogMessage("This is a log message Verbose", LoggingLevel.Verbose);
            channel.LogMessage("This is a log message Warning", LoggingLevel.Warning);
            channel.LogMessage("This is a log message Error", LoggingLevel.Error);
            channel.LogMessage("This is a log message Critical", LoggingLevel.Critical);

            mediaCapture = new MediaCapture();

            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.VideoCapture);

            var device = devices.FirstOrDefault();

            var settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = device.Id
            };

            await mediaCapture.InitializeAsync(settings);
            Log("카메라 초기화 완료");

            var allVideoProperties = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord)
             .OfType<VideoEncodingProperties>()
             .ToList();

            foreach (var resolution in allVideoProperties)
            {
                var message = $"Resolution: {resolution.Width}x{resolution.Height} {resolution.Subtype}";
            
                Log(message);
            }

                mediaCapture.Failed += MediaCapture_Failed;
        }

        public async void Capture()
        {
            var myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            StorageFile file = await myPictures.SaveFolder.CreateFileAsync("photo.jpg", CreationCollisionOption.GenerateUniqueName);

            using (var captureStream = new InMemoryRandomAccessStream())
            {
                await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);

                using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var decoder = await BitmapDecoder.CreateAsync(captureStream);
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

                    var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
        };
                    await encoder.BitmapProperties.SetPropertiesAsync(properties);

                    await encoder.FlushAsync();
                }
            }
        }

        public IAsyncAction CaptureAsync()
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                var myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
                StorageFile file = await myPictures.SaveFolder.CreateFileAsync("photo.jpg", CreationCollisionOption.GenerateUniqueName);

                using (var captureStream = new InMemoryRandomAccessStream())
                {
                    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);

                    using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var decoder = await BitmapDecoder.CreateAsync(captureStream);
                        var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

                        var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
        };
                        await encoder.BitmapProperties.SetPropertiesAsync(properties);

                        await encoder.FlushAsync();
                    }
                }
            });
        }


        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            throw new NotImplementedException();
        }

        public string GetMyString()
        {
            return $"This is call #: {++MyNumber}";
        }

        public void Log(string message)
        {
            channel.LogMessage(message, LoggingLevel.Information);
        }
    }

}
