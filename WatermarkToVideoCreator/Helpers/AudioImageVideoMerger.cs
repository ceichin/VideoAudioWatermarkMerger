using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using UIKit;

namespace WatermarkToVideoCreator
{
    /// <summary>
    /// Classes that contains logic to merge a video, sound and adds a watermark image.
    /// </summary>
    public class WatermarkAndAudioToVideoMerger
    {
		/// <summary>
		/// Notifies about progress of the video merge every second.
		/// </summary>
		/// <value>The progress action. Conatins the progress percentage.</value>
		public Action<int> ProgressAction { get; set; }

		Timer timer;

        /// <summary>
        /// Merge the specified audioPath, videoPath, toPath and myImage.
        /// </summary>
        /// <returns>The merge.</returns>
        /// <param name="audioPath">Local audio file path.</param>
        /// <param name="videoPath">Local video file path.</param>
        /// <param name="savePathWithName">File path with name where the merged video will be saved.</param>
        /// <param name="watermarkImage">Watermark image to add over the video.</param>
        public async Task Merge(string audioPath, string videoPath, UIImage watermarkImage, string savePathWithName)
		{
			NSError error = null;
			var compositionMix = new AVMutableComposition();

			// Add audio
			var audioUrl = new NSUrl(audioPath, true);
			AVUrlAsset audioAsset = new AVUrlAsset(audioUrl, new NSDictionary());
			var audioCompositionTrack = compositionMix.AddMutableTrack(AVMediaType.Audio, 0);
			AVAssetTrack audioTrack = audioAsset.TracksWithMediaType(AVMediaType.Audio)[0];
			var audioTimeRange = new CMTimeRange()
			{
				Start = CMTime.Zero,
				Duration = audioAsset.Duration
			};
			audioCompositionTrack.InsertTimeRange(audioTimeRange, audioTrack, CMTime.Zero, out error);
			if (error != null) throw new Exception(error.ToString());
			
            // With this video generator, the resulting video will have 
            //      the same duration as the given audio 
            var finalVideoDuration = audioAsset.Duration;

			// Add video
			var vAsset = new NSUrl(videoPath, true);
			AVUrlAsset videoAsset = new AVUrlAsset(vAsset, new NSDictionary());
			AVAssetTrack trackVideo = videoAsset.TracksWithMediaType(AVMediaType.Video)[0];
			var sizeOfVideo = videoAsset.NaturalSize;

			// Loop video to match audio duration
			CMTime currentTime = CMTime.Zero;
			var instructions = new List<AVMutableVideoCompositionLayerInstruction>();
			while (currentTime < finalVideoDuration)
			{
				var nextCurrentTime = currentTime + videoAsset.Duration;

				var loopDuration = videoAsset.Duration;
				if (nextCurrentTime > finalVideoDuration)
				{
					loopDuration = finalVideoDuration - currentTime;
				}

				var thisLoopTimeRange = new CMTimeRange()
				{
					Start = CMTime.Zero,
					Duration = loopDuration
				};

                // Add track
				var videoCompositionTrack = compositionMix.AddMutableTrack(AVMediaType.Video, 0);
				videoCompositionTrack.InsertTimeRange(thisLoopTimeRange, trackVideo, currentTime, out error);
				if (error != null) throw new Exception(error.ToString());

                // Add layer instruction to "hide" track layer after it ends
				var instr = AVMutableVideoCompositionLayerInstruction.FromAssetTrack(videoCompositionTrack);
				instr.SetOpacity(0.0f, nextCurrentTime);
				instructions.Add(instr);

				currentTime = nextCurrentTime;
			}

			// Create the layer with the watermark image
			CALayer watermarkLayer = new CALayer();
			watermarkLayer.Contents = watermarkImage.CGImage;
			watermarkLayer.Frame = new CGRect(0, 0, sizeOfVideo.Width, sizeOfVideo.Height);
			watermarkLayer.Opacity = 1.0f;

			// Sort layers in proper order
			CALayer parentLayer = new CALayer();
			CALayer videoLayer = new CALayer();
			var videoFrame = new CGRect(0, 0, sizeOfVideo.Width, sizeOfVideo.Height);
			parentLayer.Frame = videoFrame;
			videoLayer.Frame = videoFrame;
			parentLayer.AddSublayer(videoLayer);
			parentLayer.AddSublayer(watermarkLayer);

			// Create video composition
			AVMutableVideoComposition videoComposition = new AVMutableVideoComposition();
			videoComposition.FrameDuration = new CMTime(1, 30);
			videoComposition.RenderSize = new CGSize(sizeOfVideo.Width, sizeOfVideo.Height);
			videoComposition.AnimationTool = AVVideoCompositionCoreAnimationTool.FromLayer(videoLayer, parentLayer);

			// Instruction to insert layer
			AVMutableVideoCompositionInstruction mainInstruction = new AVMutableVideoCompositionInstruction();
			mainInstruction.TimeRange = new CMTimeRange()
			{
				Start = CMTime.Zero,
				Duration = compositionMix.Duration
			};

			// These 3 following lines are maybe not necessary...
			AVAssetTrack videoAssetTrack = compositionMix.TracksWithMediaType(AVMediaType.Video)[0];
			AVMutableVideoCompositionLayerInstruction layerInstruction = AVMutableVideoCompositionLayerInstruction.FromAssetTrack(videoAssetTrack);
			instructions.Add(layerInstruction);

			mainInstruction.LayerInstructions = instructions.ToArray();
			videoComposition.Instructions = new AVVideoCompositionInstruction[] { mainInstruction };

			// Export
			AVAssetExportSession assetExport = new AVAssetExportSession(compositionMix, AVAssetExportSessionPreset.HighestQuality);
			assetExport.VideoComposition = videoComposition;
			assetExport.OutputFileType = AVFileType.QuickTimeMovie;
			assetExport.ShouldOptimizeForNetworkUse = true;
			assetExport.OutputUrl = NSUrl.FromFilename(savePathWithName);
			StartProgress(assetExport);
			try
			{
				await assetExport.ExportTaskAsync();
			}
			catch
			{
				throw;
			}
			finally
			{
				StopProgress();
			}
			if (assetExport.Error != null) throw new Exception(assetExport.Error.ToString());
		}

		void StartProgress(AVAssetExportSession export)
		{
			timer = timer ?? new Timer(1000);
			timer.Elapsed += (sender, e) =>
			{
				ProgressAction?.Invoke((int)(export.Progress * 100f));
			};
			timer.Start();
		}

		void StopProgress()
		{
			timer.Stop();
		}
    }
}
