using System;
using System.Drawing;
using System.IO;
using Foundation;
using UIKit;

namespace WatermarkToVideoCreator.Helpers
{
    public enum ImageType { JPG, PNG }

    public static class ImageHelper
    {
        /// <summary>
        /// Saves UIImage as a file.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="toDirectory">To directory to save.</param>
        /// <param name="type">Image type.</param>
        public static void SaveImageAsFile(this UIImage img, string pathToSaveWithName, ImageType type)
		{
            if (img == null) throw new ArgumentException("Image cannot be null");

            NSData imgData = null;
            switch (type)
            {
				case ImageType.JPG:
					imgData = img.AsJPEG();
					break;
                case ImageType.PNG:
					imgData = img.AsPNG();
					break;
                default:
                    throw new ArgumentException("Not supported image type");
            }

			if (!imgData.Save(pathToSaveWithName, false))
			{
				throw new ArgumentException("There was a problem saving the image");
			}
		}

        /// <summary>
        /// Converts an UIView to an image file.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pathToSaveWithName">Path to save with name.</param>
		public static void ToImageFile(this UIView view, string pathToSaveWithName)
		{
			UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 0);
			view.DrawViewHierarchy(view.Bounds, true);
			var newImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
            // Save it as to use it directly may cause problems.
			newImage.SaveImageAsFile(pathToSaveWithName, ImageType.PNG);
		}

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="img">Image.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static UIImage ResizeImage(this UIImage img, float width, float height)
		{
			UIGraphics.BeginImageContext(new SizeF(width, height));
			img.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}
    }
}
