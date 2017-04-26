# Video, Audio and Watermark Merger
Quick and simple project to demostrate how to generate videos from an image, merge a video with sound, or add a watermark image to a video using Xamarin.iOS. I spent several hours to make these work and I wanted to share it in case someone else want to do the same under the iOS and Xamarin platforms.

## Create Video From Image
Given an image and a duration, a video is created. AVAssetWriter is used for this.

## Merge Audio to a Video
Given a video and an audio, a new video is created. The video sound will now be the given audio. AVMutableComposition and AVAssetExportSession is used for this. You could add as many audio tracks as you want.

## Merge Audio and Watermark Image to a Video
Given a video, an audio and an image, a new video is created. The video sound will now be the given audio and an overlay image layer is added to the video. This could be used as a watermark, to add a title, or for anything static.

In this example we create the watermark image in the XCode interface builder, allowing us to create images dynamically depending. This way you could choose the name printed on the video, or change a watermark image right in execution time. For this we convert an UIView to an image, and then we add it to the video.

#### Other Notes
AVPlayerViewController is used for video playback. The project contains a video bigger than 7MB.