# NyoVT
This project has started to make 3d vtubing more accessible on linux, windows isn't our focus.  
Ofcourse with it being Unity windows should work more than fine but this is our disclaimer.

It's heavily inspired by VSeeFace but I was missing some features,  
one was streaming to web which I'm still working on and a way to make the background green for green screen.  
I normally set the red ambience on VSeeFace higher so I can chroma key the gray background,  
this is one of the compromises to using it on linux, but by using the green screen it would be more than possible.  

Why the browser streaming? On linux, specifically Wayland the loading of windows in obs is quite annoying,  
this makes that browser sources are more reliable most of the time.

# Usage
To use VTubeStudio, add your phone's ip in the settings and it'll connect to vtube studio.  
To use WebCam, select your camera which should automatically be correct, then in settings click on use webcam.  

# Web Renderer
To use the web renderer set the obs web source to https://ch4rli.me/nyovt/  
  
Don't worry, the data isn't sent from the app to the internet, instead the page reads it from your local pc so everything stays local.  
If you want to have this render on a different pc, set the web source to your local ip, for example: http://192.168.178.10:7829/ and refresh (auto refresh only works for https://ch4rli.me/nyovt/)  
Why would you use web render?  
**cross compatibility** and **privacy**,  
the web renderer does not render ui, allowing you to make changes without anyone seeing and it works on basically every platform.
