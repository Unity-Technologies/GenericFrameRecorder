# GenericRecorder-MovieRecorderPlugin

### Brief

FrameRecorder recorder plugin to record mp4 and webm movies with audio using Unity APIs only.

It uses the same native APIs as Unity's video transcoding implementation, made available through the UnityEditor.MediaEncoder C# class.

### Features

* webm container using the VP8 video codec and Vorbis audio codec.
* mp4 container using the H.264 video codec and AAC audio codec.
* Embedded alpha in webm container using [Google's specification](http://wiki.webmproject.org/alpha-channel)
* Seamless non-realtime audio capture from Unity, allowing perfect capture even when frames cannot render in real time.

See the [GenericFrameRecorder](https://github.com/Unity-Technologies/GenericFrameRecorder) project for the framework doc and source.

### Current limitations

* Only available in editor.
* Only supports constant frame rate.
* Bitrate is fixed.