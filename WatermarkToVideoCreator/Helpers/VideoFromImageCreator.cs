using System;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using UIKit;

namespace WatermarkToVideoCreator
{
    public class ImageToVideoCreator
    {
        public static async Task Create(UIImage image, double duration, string saveToPathWithName)
		{
			var width = (int)image.Size.Width;
            var height = (int)image.Size.Height;

			NSError error = null;
			AVAssetWriter videoWriter = new AVAssetWriter(new NSUrl(saveToPathWithName, false), AVFileType.QuickTimeMovie, out error);

			var videoSettings = new AVVideoSettingsCompressed();
			videoSettings.Codec = AVVideoCodec.H264;
			videoSettings.Width = width;
			videoSettings.Height = height;
			videoSettings.ScalingMode = AVVideoScalingMode.ResizeAspectFill; // resize the input

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
