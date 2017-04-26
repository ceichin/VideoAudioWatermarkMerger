// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace WatermarkToVideoCreator
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton btnVideoFromImage { get; set; }


        [Outlet]
        UIKit.UIButton btnVideoWithAudio { get; set; }


        [Outlet]
        UIKit.UIButton btnVideoWithAudioAndWatermark { get; set; }


        [Outlet]
        UIKit.UILabel lblStatus { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnVideoFromImage != null) {
                btnVideoFromImage.Dispose ();
                btnVideoFromImage = null;
            }

            if (btnVideoWithAudio != null) {
                btnVideoWithAudio.Dispose ();
                btnVideoWithAudio = null;
            }

            if (btnVideoWithAudioAndWatermark != null) {
                btnVideoWithAudioAndWatermark.Dispose ();
                btnVideoWithAudioAndWatermark = null;
            }

            if (lblStatus != null) {
                lblStatus.Dispose ();
                lblStatus = null;
            }
        }
    }
}