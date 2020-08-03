This is the documentation for OmniLantern.

（Password:koikekoike）
## Hardwars:
- On ProCam:
	- Camera(OmniLantern) connect to PC
	- Projector(OmniLantern) connect to PC
	- Charging Cable(Projecor) could be unplug after charging
	- LED Cable must bu connected through using
- On PC:
	- Box PC, have to use keyboard, monitor and mouse, also may need a adapter to provide the extra HDMI

### P.S
- There may be some unstable connection of the LED cable, waving the cable to get a brighter light.
- The position of camera maybe dislocation
- Don't smash the hardware while moving it,it is fragile

## Software
### Preparing
- The view of Camera could be seen through "Point Gray FlyCap2"(in start menu)
- The project is in Unity--OmniLantern

### Unity
- Please import "OpenCVforUnity"
- I remove the texture from the Github page, for it is too large to upload, so please prepare your own textures
  

Scenes: I prepared three scenes in Assets
- HelloOmniProCam
- OpenMenu
- LampShade

#### HelloOmniProCam
A basic function of OmniLantern, project something based on the markers
- OmniProCam
	- Donot change this part, it connects to the .dll which Sato-sensei made
- ArMarker
	- Functions: Including the image processing function and marker detection function, you can find it in "\scripts\markers". You can change the result of marker detection in "MarkerDetection.cs"
	- ARGameObjects: I choose four objects connect to four markers, you can change the relationship in "MarkerDetection.cs". The marker use the "\Projection\OmniLantern.shader"(can change to "\Projection\FisheyeProjection", the distortion will be effected by the transform), and the mesh is "default-cube", you can change it to "default-sphere" for spherical display.
	- DepthCameras: I use it to mask the other parts for cube display


#### OpenMenu
Show an menu view, users could choose the methods by pinching gesture, you can see the code in "\scripts\UI"
- HandDetectionUI: To detect the pinching gesture
- VitualKeyborad: I connected each gesture to a key, to press the button in UI
- You can load different scene in "\scripts\UI\ButtonControl.cs"

#### LampShade
There will have 5s(can be changed) waiting for covering the lampshade, and take the first frame as template, and detect the single finger touching. You can find the code in "\scripts\Shade"

#### Others
- There is a .cs file for thresholding the things in front of the camera, which is in "\scripts\FaceThreshold"
- There is some other unused code in "\scripts\Trucks"
- You can adjust the camera's position when the coaxical of the ProCam is dislocated


