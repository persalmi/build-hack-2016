using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SnapFeud.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StorageFile lastPhoto;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
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
                    lastPhoto = destinationFile;

                    await AnalyzeImage(destinationFile);
                }
            }
            catch (Exception ex)
            {
                resultText.Text = $"Failed: {ex.Message}";
            }
        }

        private async Task ResizeImage(StorageFile sourceFile, StorageFile destinationFile)
        {
            resultText.Text = "Resizing...";
            using (var sourceStream = await sourceFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
                var newWidth = Math.Min(800, decoder.PixelWidth);
                var newHeight = newWidth * decoder.PixelHeight / decoder.PixelWidth;

                BitmapTransform transform = new BitmapTransform() { ScaledHeight = newHeight, ScaledWidth = newWidth };
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                using (var destinationStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, newWidth, newHeight, 96, 96, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
                }
            }
        }

        private async Task AnalyzeImage(StorageFile imageFile)
        {
            resultText.Text = "Analyzing...";
            try
            {
                VisionServiceClient visionServiceClient = new VisionServiceClient(File.ReadAllText("subscriptionkey.txt"));
                using (var stream = await imageFile.OpenStreamForReadAsync())
                {
                    VisualFeature[] visualFeatures = new VisualFeature[]
                    {
                        VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description,
                        VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags
                    };

                    AnalysisResult analysisResult =
                        await visionServiceClient.AnalyzeImageAsync(stream, visualFeatures);
                    resultText.Text = string.Join("\n", analysisResult.Tags.OrderByDescending(x => x.Confidence).Select(x => $"{x.Name} ({x.Confidence:F})"));
                }
            }
            catch (Exception ex)
            {
                resultText.Text = $"Failed: {ex.Message}";
            }
        }

        private async Task ShowImage(StorageFile photo)
        {
            resultText.Text = "Showing...";
            using (IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied);
                SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

                imageControl.Source = bitmapSource;
            }
        }

        private async void imageControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (lastPhoto != null)
            {
                var fileSavePicker = new FileSavePicker();
                fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                fileSavePicker.FileTypeChoices.Add("JPG Image", new List<string>() {".jpg"});

                var filePicker = await fileSavePicker.PickSaveFileAsync();
                if (filePicker != null)
                {
                    await lastPhoto.CopyAndReplaceAsync(filePicker);
                }
            }
        }
    }
}
