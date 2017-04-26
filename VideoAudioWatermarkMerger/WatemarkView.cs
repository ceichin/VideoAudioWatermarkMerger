using Foundation;
using System;
using ObjCRuntime;
using UIKit;

namespace WatermarkToVideoCreator
{
    public partial class WatemarkView : UIView
    {
		public static WatemarkView Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("WatermarkView", null, null);
            var view = Runtime.GetNSObject<WatemarkView>(arr.ValueAt(0));
			return view;
		}

        public WatemarkView (IntPtr handle) : base (handle)
        {
        }
    }
}