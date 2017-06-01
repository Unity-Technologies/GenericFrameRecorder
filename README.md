# Frame Recorder
### Brief
The *Frame Recorder* is a project that facilitates recording of Unity artifacts. The framework does not define what can be recorded, but defines a standard way of how to record any artifact and takes care of aspects common to all recorders (time managenent, Timeline integration, record windows, etc). 

An important aspect is extendibility, where recorder types are detected at run time and made available to the recording framework dynamically. This is accomplished through inheritance and class attributes. 

Another key consideration is UX, by defining a standard pattern and basic classes, lets the framework treat all recorders equally and display them consistently. This allows for a generic “recorder window” that is provided and takes care of configuring and starting a “recording session” from edit mode.

Code reusability and easy of use for developers is also a prime consideration. As much as possible, modularization in a Lego mentality is promoted so that work done for one specific recorder, say MP4 recording, can be reused by an other type of recorder, say PNG or WAV recorders.

Of note is the control of flow of time: is variable frame rate requested or fixed? This impacts all recorders that record more than one frame’s worth of data and so is taken care of by the framework. 
And last but not least, is the ensuring that at any point a custom recorder can override any standard behaviour of the service to allow full flexibility and not constrain what can be recorded or how.

### Main limitations of current state
..* Recorders are Player standalone friendly, but not the editors.
..* Framerate is set at the Recorder level which makes for potential conflict when multiple recorders are active simultaneously.

