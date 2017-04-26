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
    public class AudioToVideoMerger
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
        /// <param name="audioPath">Local audio file path.</param>
        /// <param name="videoPath">Local video file path.</param>
        /// <param name="savePathWithName">File path with name where the merged video will be saved.</param>
        public async Task Merge(string audioPath, string videoPath, string savePathWithName)
        {
            NSError error;
            var composition = new AVMutableComposition();

            var timeRange = new CMTimeRange()
            {
                Start = CMTime.Zero
            };

			// Add video
			var videoComposition = composition.AddMutableTrack(AVMediaType.Video, 0);
			var videoUrl = new NSUrl(videoPath, true);
			AVUrlAsset videoAsset = new AVUrlAsset(videoUrl, new NSDictionary());
            timeRange.Duration = videoAsset.Duration; // <-- Duration of video, change as needed
			AVAssetTrack trackVideo = videoAsset.TracksWithMediaType(AVMediaType.Video)[0];
			videoComposition.InsertTimeRange(timeRange, trackVideo, CMTime.Zero, out error);

			// Add audio
            var audioComposition = composition.AddMutableTrack(AVMediaType.Audio, 0);
            var audioUrl = new NSUrl(audioPath, true);
            AVUrlAsset audioAsset = new AVUrlAsset(audioUrl, new NSDictionary());
			AVAssetTrack track = audioAsset.TracksWithMediaType(AVMediaType.Audio)[0];
			audioComposition.InsertTimeRange(timeRange, track, CMTime.Zero, out error);

			// Export
			AVAssetExportSession assetExport = new AVAssetExportSession(composition, AVAssetExportSession.PresetPassthrough);
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
