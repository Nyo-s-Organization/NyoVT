# NyoVT
This project has started to make 3d vtubing more accessible on linux, windows isn't our focus.  
Ofcourse with it being Unity windows should work more than fine but this is our disclaimer.

It's heavily inspired by VSeeFace but I was missing some features,  
one was streaming to web which I'm still working on and a way to make the background green for green screen.  
I normally set the red ambience on VSeeFace higher so I can chroma key the gray background,  
this is one of the compromises to using it on linux, but by using the green screen it would be more than possible.  

Why the browser streaming? On linux, specifically Wayland the loading of windows in obs is quite annoying,  
this makes that browser sources are more reliable.

# Usage
To use VTubeStudio, add your phone's ip in the settings and it'll connect to vtube studio.  
To use WebCam, select your camera which should automatically be correct, then in settings click on use webcam.  

# Web Renderer
To use the web renderer set the obs web source to https://nyo-s-organization.github.io/NyoVT/
  
Don't worry, the data isn't sent from the app to the internet, instead the page reads it from your local pc so everything stays local.  
If you want to have this render on a different pc, set the web source to your local ip, for example: http://192.168.178.10:7829/ and refresh (auto refresh only works for https://nyo-s-organization.github.io/NyoVT/)  
- Why would you use web render?  
  **cross compatibility** and **privacy**,  
  the web renderer does not render ui, allowing you to make changes without anyone seeing and it works on basically every platform.
- When should I use what url in obs or any streaming application?
  The Web renderer is usable in a browser source ofcourse but there's still a few possible links you can use
  - (192.x.x.x:7829) Your local ip can be used if you want the footage to go to a **different pc** on the same network (or farther away with port forwarding)
  - (127.0.0.1:7829) The local host ip, this is the same for everyone, you can use this if you want the full footage **includes greenscreen if set**
  - (https://nyo-s-organization.github.io/NyoVT/) The automatically updating endpoint, **recommended for easy setup**

# Custom Movement Script (Automatization)
Do you want to automate a vtubing character or any character?  
We have a custom api endpoint that lets you connect to the websocket and send custom tracking data, could also be used for mocap and other use cases that require more freedom.  
Personally I'm using this to make an automated vtuber model move on stream and add ambience, can also be used to make something like NeuroSama.  
- Supported blendshapes:
  - eyeBlink_L
  - eyeBlink_R
  - jawOpen
  - mouthSmile_L
  - mouthSmile_R
  - browDown_L
  - browDown_R
- How to use the api as **websocket** (Windows only right now):  
  ```py
  import asyncio
  import websockets
  import json
  
  URI = "ws://localhost:7830/ws/"
  
  async def send_messages():
      async with websockets.connect(URI) as ws:
          print("Connected to Unity WebSocket")
  
          await ws.send(json.dumps({
              "position": [1.0, 2.0, 3.0]
          }))
          await ws.send(json.dumps({
              "rotation": [0.0, 180.0, 0.0]
          }))
          await ws.send(json.dumps({
              "blendshape": ["Smile", 0.75]
          }))
  
  asyncio.run(send_messages())
  ```
- How to use the api with **http requests** (cross platform):
  ```py
  import requests
  import json
  import time
  
  URL = "http://localhost:7830/"
  
  r = requests.post(URL, json={
      "position": [1.0, 2.0, 3.0],
      "rotation": [1.0, 2.0, 3.0],
      "blendshape": ["Smile", 0.75]
  })
  ```
