This project contains many example which show how to use the different libraries
of the DigitalRune Engine.

Projects
--------
The solution contains several "DigitalRune.Samples" projects:
- DigitalRune.Samples(XNA,Win): 
    Samples built for Windows (desktop) using XNA.
- DigitalRune.Samples(XNA,Xbox) 
    Samples built for Xbox 360.
- DigitalRune.Samples(XNA,WP) 
    Samples built for Windows Phone 7. 
    Note: Most samples are not optimized for the phone and do not have touch 
    controls.
- DigitalRune.Samples(XNA,Win,Kinect) 
    Kinect samples. 
    Requires Microsoft Kinect SDK.
- DigitalRune.Samples(MG,Win) 
    Samples built for Windows (desktop) using MonoGame.
    Requires XNA only to build the content (see project ContentBuilder).
    At runtime MonoGame is used instead of XNA.

Usage
-----
The controls depend on the currently selected sample. The general controls are:
- Using keyboard and mouse:
    Hold <Alt> to bring up the mouse cursor and a menu window.
    Hold <F1> to show Help screen.
    Use the <Page Up>/<Page Down> arrow keys to switch samples.
    Double-click <Escape> or press <Alt>-<F4> to exit the application.
- Using game pad:
    Press <Start> to select a game pad.
    Hold <Left Stick> to show Help screen.
    Use <DPad Left>/<DPad Right> to switch samples.
    Double-click <Back> to exit the application.
- On Windows Phone:
    Flick left/right to switch samples.

The Help screen shows a description of the current sample and list of additional
controls. (Note: Most samples only support mouse and keyboard.)

Sample Framework
----------------
The types in the folder "Sample Framework" implement the infrastructure of the
application (menu, help, etc.). The MenuComponent (derived from GameComponent)
automatically discovers samples using reflection and provides means to switch 
between samples at runtime. (It also defines which sample is loaded at startup.)

Samples
-------
Samples are derived from the XNA class GameComponent and need to be marked with
the SampleAttribute. Example:
  [Sample(...)]
  class MySample : GameComponent { ... }
The solution also provides several abstract base classes (Sample, BasicSample,
PhysicsSample, etc.) that samples can inherit. These base classes implement 
common tasks such as retrieving the game services, setting up a basic graphics 
screen for rendering, and more.
When a sample is disposed, it has to undo all changes made to game services 
(e.g. remove all game objects from the game object service, remove all rigid 
bodies from the physics simulation, dispose scene nodes, etc.) to avoid side 
effects for other samples.
At any time only a single sample instance is active.

Game Objects
------------
Classes derived from DigitalRune.Game.GameObject implement game logic. 
Examples: Camera logic, loading and updating model and rigid body, etc.
Game objects are created and added to the game object service by samples and 
removed when the sample is disposed. The project contains several GameObjects 
that are reused in different samples.
Game objects load resources (graphics, physics objects, particle systems and 
animations) in GameObject.OnLoad() and release the resources in 
GameObject.OnUnload(). Game logic is implemented in GameObject.OnUpdate() which 
is executed in every frame.

Graphics Screens
----------------
The project contains several GraphicsScreen implementations that are reused in 
different samples. DigitalRune GraphicsScreens are explained in the DigitalRune 
Graphics documentation and samples.

DigitalRune Graphics Content
----------------------------
The DigitalRune.Samples project references its own content projects and the 
content project which contains the default DigitalRune Graphics content! The 
DigtalRune Graphics content project provides pre-built resources (shaders and 
textures) required by DigitalRune Graphics. It is located in the folder 
  <DigitalRune Engine Folder>\Content\XNA\Windows\  (for Windows games)
or 
  <DigitalRune Engine Folder>\Content\XNA\Xbox360\  (for Xbox games).

(Multithreaded) Game Loop
-------------------------
The class SampleGame implements the game loop. The game loop executes the
different sub-systems of the game (input, physics, animation, graphics, etc.). 
Optionally, the game loop can run services in parallel to better utilize 
multi-core CPUs. Further, each subsystem (especially geometry, physics, 
animation, particles) uses multi-threading internally.
