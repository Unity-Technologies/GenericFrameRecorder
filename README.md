# Frame Recorder
### Brief
The *Frame Recorder* is a project that facilitates recording of Unity artifacts from Unity. The framework does not define what can be recorded, but defines a standard way of how to define and setup a recorder and takes care of aspects common to all recorders (time managenent, Timeline integration, record windows, etc). 

Extensibility is a prim concideration and since not all use cases can be thought of in advance, whenever relevant, the frameworks base classes strive to always offer an easy way to override the default beahviour of the system. where recorder types are detected at run time and made available to the recording framework dynamically. This is accomplished through inheritance and class attributes. 

Another key consideration is UX, by defining a standard pattern and basic classes, lets the framework treat all recorders equally and display them consistently. This allows for a generic “recorder window” that is provided and takes care of configuring and starting a “recording session” from edit mode.

Code reusability and easy of use for developers is also a prime consideration. As much as possible, modularization in a Lego mentality is promoted so that work done for one specific recorder, say MP4 recording, can be reused by an other type of recorder, say PNG or WAV recorders.

Of note is the control of flow of time: is variable frame rate requested or fixed? This impacts all recorders that record more than one frame’s worth of data and so is taken care of by the framework. 
And last but not least, is the ensuring that at any point a custom recorder can override any standard behaviour of the service to allow full flexibility and not constrain what can be recorded or how.

### Current limitations
..* Recorders are Player standalone friendly, but not the editors.
..* Framerate is set at the Recorder level which makes for potential conflict when multiple recorders are active simultaneously.

## Triggering a Recording

### Through the Editor's Record window

1. Select a type of recording and open the recorder window
![](GenericFrameRecorder/docs/images/recorder-menu.png)
2. Edit the recorders settings
![](GenericFrameRecorder/docs/images/RecorderWindow.png)
3. Click "Start Recording" to lauch recording.

Note that this can be done  from edit mode and from game mode...
a
### From a timeline track
1. Create a timeline asset and add it to the scene.
2. Add a "Frame Recorder track" to the timeline.
3. Add a "Frame Recorder clip" to the track.
4. Select the newly added slip
![](GenericFrameRecorder/docs/images/TimelineTrack.png)
5. Edit the clip's settings
![](GenericFrameRecorder/docs/images/RecorderClip.png)
6. Enter play mode and trigger the timeline through behaviours...

## Design

### Conceptual blocks
The Recording framework is composed of three conceptual groups:
![](GenericFrameRecorder/docs/images/ConceptualBlocks.PNG)
* Recorders: the part that takes data feeds (Inputs) and transform them into whatever format they want (Image input -> mp4 file). They do NOT deal with gathering the data from Unity: that is the Inputs task. Every recorder is broken down into three peices: Recorder, Settings and Settings Editor.
* Inputs: specizalied classes that know how to gather a given type of data from unity and how to pre-package that data in a way that is ready from consumption by the Recorders. Like recorders, Inputs are borken down into three parts: Input, Setttings and Settings Editor.
* Support: holds the FrameRecorder's logic, UI, timeline integration and services.
