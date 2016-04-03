using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SnapFeud.UWP.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly GameServiceProxy _gameProxy;

        public GameViewModel(INavigationService navigationService, SnapFeudContext context)
        {
            _navigationService = navigationService;
            _gameProxy = new GameServiceProxy(context.WebApiBaseUri);
            Context = context;
        }

        public SnapFeudContext Context { get; }

        private RelayCommand _photoCommand;
        public ICommand PhotoCommand => _photoCommand ?? (_photoCommand = new RelayCommand(TakePhoto));

        private SoftwareBitmapSource _imageSource;

        public SoftwareBitmapSource ImageSource
        {
            get { return _imageSource; }
            set { Set(ref _imageSource, value); }
        }

        public string StatusText => string.Format($"{Context.UserName} - Score: {Context.CurrentGame.Score}{Context.ResultText}");

        private async void TakePhoto()
        {
            try
            {
                var capture = new CameraCaptureUI();
                capture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Png;
                var photo = await capture.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photo != null)
                {
                    var destinationFile = await StorageFile.GetFileFromPathAsync(Path.GetTempFileName());
                    await ResizeImage(photo, destinationFile);
                    await ShowImage(destinationFile);
                    var data = await GetImageData(destinationFile);
                    Context.ResultText = " - Working...";
                    RaisePropertyChanged(() => StatusText);

                    var newGame = await _gameProxy.SubmitAnswer(Context.CurrentGame.Id, data);
                    if (newGame != null)
                    {
                        Context.CurrentGame = newGame.Item2;
                        Context.ResultText = newGame.Item1 ? string.Empty : " - Wrong answer!";
                    }
                    else
                    {
                        Context.ResultText = " - Error";
                    }

                    RaisePropertyChanged(() => StatusText);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task ResizeImage(StorageFile sourceFile, StorageFile destinationFile)
        {
            using (var sourceStream = await sourceFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
                var newWidth = Math.Min(800, decoder.PixelWidth);
                var newHeight = newWidth*decoder.PixelHeight/decoder.PixelWidth;

                BitmapTransform transform = new BitmapTransform() {ScaledHeight = newHeight, ScaledWidth = newWidth};
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                using (var destinationStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder =
                        await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, newWidth, newHeight, 96,
                        96, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
                }
            }
        }

        private async Task ShowImage(StorageFile photo)
        {
            using (IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied);
                SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);
                ImageSource = bitmapSource;
            }
        }

        private async Task<byte[]> GetImageData(StorageFile imageFile)
        {
            using (var stream = await imageFile.OpenStreamForReadAsync())
            {
                var data = new byte[(int)stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return data;
            }
        }
    }
}
