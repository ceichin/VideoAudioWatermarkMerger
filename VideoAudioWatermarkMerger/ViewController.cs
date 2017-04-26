using System;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using AVKit;
using CoreGraphics;
using Foundation;
using UIKit;
using WatermarkToVideoCreator.Helpers;

namespace WatermarkToVideoCreator
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            Console.WriteLine($"See generated files here: {FileSystemHelper.TemporaryDir}");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Button to create a video from an image
            btnVideoFromImage.TouchUpInside += async (sender, e) =>
            {
                SetStatus("Initilizing...");

                try
                {
                    FileSystemHelper.CleanDir(FileSystemHelper.TemporaryDir);

                    var imgPath = "dogSanta.jpg";
                    var img = UIImage.FromFile(imgPath);
                    img = img.ResizeImage(768, 1280);
                    var resultVideoPath = Path.Combine(FileSystemHelper.TemporaryDir, "video.mov");

                    SetStatus("Creating video...");

                    await VideoFromImageCreator.Create(img, 10, resultVideoPath, 768, 1280);

                    SetStatus("Video finished!");

                    PlayVideo(resultVideoPath);
                }
                catch(Exception ex)
                {
                    SetStatus($"Error: {ex.Message}");
                }
            };

			// Button to merge an audio with a video
			btnVideoWithAudio.TouchUpInside += async (sender, e) =>
            {
                SetStatus("Initilizing...");

                try
                {
                    FileSystemHelper.CleanDir(FileSystemHelper.TemporaryDir);

                    var videoPath = "lion drinking.mov";
                    var audioPath = "lion drinking.aac";
                    var resultVideoPath = Path.Combine(FileSystemHelper.TemporaryDir, "video.mov");

                    SetStatus("Creating video...");

                    var merger = new AudioVideoMerger();
                    merger.ProgressAction += (progress) => {
                        SetStatus($"Creating video... {progress}%");
                    };
                    await merger.Merge(audioPath, videoPath, resultVideoPath);

                    SetStatus("Video finished!");

                    PlayVideo(resultVideoPath);
                }
                catch (Exception ex)
                {
                    SetStatus($"Error: {ex.Message}");
                }
            };

			// Button to merge a video with an audio and a watermark
			btnVideoWithAudioAndWatermark.TouchUpInside += async (sender, e) =>
            {
                SetStatus("Initilizing...");

                try
                {
                    FileSystemHelper.CleanDir(FileSystemHelper.TemporaryDir);

                    // Create watermark image from UIView
                    var watermarkView = WatemarkView.Create();
                    watermarkView.Frame = new CGRect(0, 0, 1280, 768);
                    var watermarkPath = Path.Combine(FileSystemHelper.TemporaryDir, "watermark.png");
                    watermarkView.ToImageFile(watermarkPath);
                    var watermarkImage = UIImage.FromFile(watermarkPath);

                    var videoPath = "lion drinking.mov";
                    var audioPath = "lion drinking.aac";
                    var resultVideoPath = Path.Combine(FileSystemHelper.TemporaryDir, "video.mov");

                    SetStatus("Creating video...");

                    var merger = new AudioImageVideoMerger();
                    merger.ProgressAction += (progress) => {
                        SetStatus($"Creating video... {progress}%");
                    };
                    await merger.Merge(audioPath, videoPath, watermarkImage, resultVideoPath);

                    SetStatus("Video finished!");

                    PlayVideo(resultVideoPath);
                }
                catch (Exception ex)
                {
                    SetStatus($"Error: {ex.Message}");
                }
            };
        }

        void SetStatus(string statusText)
        {
            InvokeOnMainThread(() => lblStatus.Text = statusText);
        }

        void PlayVideo(string videoPath)
        {
            var videoUrl = NSUrl.FromFilename(videoPath);
            var player = new AVPlayer(videoUrl);

            var playerController = new AVPlayerViewController();
            playerController.Player = player;

            PresentViewController(playerController, true,() => { 
                player.Play();
            });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}
