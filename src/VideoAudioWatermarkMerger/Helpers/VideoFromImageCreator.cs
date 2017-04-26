using System;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using UIKit;

namespace WatermarkToVideoCreator.Helpers
{
    /// <summary>
    /// Creates a video with a static image for a certain duration of time.
    /// </summary>
    public class VideoFromImageCreator
    {
		/// <summary>
		/// Create a static video from an image. extension must be .mov.
		/// </summary>
		/// <returns>The create.</returns>
		/// <param name="image">Image.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="saveToPathWithName">Save to path with file name included.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static async Task Create(UIImage image, double duration, string saveToPathWithName, int width, int height)
        {
            NSError error = null;
            AVAssetWriter videoWriter = new AVAssetWriter(new NSUrl(saveToPathWithName, false), AVFileType.QuickTimeMovie, out error);
            if (error != null) throw new Exception(error.ToString());

            var videoSettings = new AVVideoSettingsCompressed();
            videoSettings.Codec = AVVideoCodec.H264;
            videoSettings.Width = width;
            videoSettings.Height = height;

            var writerInput = new AVAssetWriterInput(AVMediaType.Video, videoSettings);

            AVAssetWriterInputPixelBufferAdaptor adaptor = new AVAssetWriterInputPixelBufferAdaptor(writerInput, new NSDictionary());

            // Start session
            videoWriter.AddInput(writerInput);
            videoWriter.StartWriting();
            videoWriter.StartSessionAtSourceTime(CMTime.Zero);

            // Write samples
            CVPixelBuffer buffer = pixelBufferFromCGImage(image.CGImage, new CGSize(width, height));
            adaptor.AppendPixelBufferWithPresentationTime(buffer, CMTime.Zero);
            adaptor.AppendPixelBufferWithPresentationTime(buffer, CMTime.FromSeconds(duration, 1000));

            // Finish session
            writerInput.MarkAsFinished();
            videoWriter.EndSessionAtSourceTime(CMTime.FromSeconds(duration, 1000));
            await videoWriter.FinishWritingAsync();
            if (videoWriter.Error != null) throw new Exception(videoWriter.Error.ToString());
        }

        static CVPixelBuffer pixelBufferFromCGImage(CGImage image, CGSize size)
        {
            NSObject[] keys = { CVPixelBuffer.CGImageCompatibilityKey,
                CVPixelBuffer.CGBitmapContextCompatibilityKey };

            NSObject[] objects = { new NSNumber(true), new NSNumber(true) };
            NSDictionary options = NSDictionary.FromObjectsAndKeys(objects, keys);
            CVPixelBufferAttributes attr = new CVPixelBufferAttributes(options);
            CVPixelBuffer pxbuffer = new CVPixelBuffer((int)size.Width, (int)size.Height,
                                                        CVPixelFormatType.CV32ARGB, attr);
            if (pxbuffer != null)
            {
                pxbuffer.Lock(CVPixelBufferLock.None);
                IntPtr pxdata = pxbuffer.BaseAddress;
                if (pxdata != IntPtr.Zero)
                {
                    using (var rgbColorSpace = CGColorSpace.CreateDeviceRGB())
                    {
                        using (CGBitmapContext context = new CGBitmapContext(pxdata, (int)size.Width, (int)size.Height, 8, pxbuffer.BytesPerRow, rgbColorSpace, CGImageAlphaInfo.NoneSkipFirst))
                        {
                            if (context != null)
                            {
                                context.DrawImage(new CGRect(0, 0, image.Width, image.Height), image);
                                //imgBufferdanGelen.Image = UIImage.FromImage (context.ToImage ());
                            }
                        }
                    }
                }
                pxbuffer.Unlock(CVPixelBufferLock.None);

            }
            return pxbuffer;
        }
    }
}
