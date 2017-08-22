Simple SpriteScript Tutorial:

Step 1: Adding your spritesheets.
The first thing you need to do to use Simple SpriteScript is create
spritesheets in Unity. Simple SpriteScript uses Unity's built-in 2D textures as
spritesheets. Simply add a new image to YOUR-PROJECT/Assets/Resources/Sprites.
Then, split the image into sprties using Unity's sprite editor tool.

Step 2: Setting up Scenes/gameObjects
In each scene where you wish to use Simple SpriteScript, add the included 
gameObject called "sprite_controller" into the scene. For each object that
you want to animate, add a SpriteRenderer component and a SpriteScript
component.

Step 3: Adding Animations (Optional)
Each SpriteScript can have multiple Animations. By default, there is a
single "default" animation that loops through each sprite in the sheet
consecutively. You can create new Animations by defining the necessary
fields in the Inspector pane for SpriteScript or by using the Animation
class constructor in conjuction with the AddAnimation method of SpriteScript.
Check out the reference below for more information.

Step 4: Animating!
Call setSpriteSheet on the SpriteScrip of the gameObject you want to animate
to begin animating! If you want to use a different animation, use SetAnimation.
If you want to manually manipulate the frame of the animation that appears, use
SetIndex. If you want to change the speed, use SetFramesPerSecond. Check out
the reference below for more information.

Function/Class Reference:

public void SetSpriteSheet(string spriteName)
Sets the spritesheet to the texture in /Assets/Resources/Sprites/
named 'spriteName'

public void SetFramesPerSecond(float framesPerSecond)
Sets the speed of the animation to the given float in frames per second.
e.g., if 5 is passed in, the frame will change 5 times every second.
if 0.5 is passed in, it will change once every two seconds.

public void SetIndex(int index)
Manually sets the index of the frame to be shown.
On passing in a parameter n, will set the image to the nth frame of
the current animation.
Resets the timing on the current frame (such that the frame that it is
now set to will appear for the full amount of time that it should to
meet the correct frames per second).

public void SetAnimation(string name)
Sets the current animation to the animation associated with the name
passed in as an argument.
New animations can be added in the editor or by using AddAnimation.

public class Animation
Class for defining a custom animation for a sprite sheet.
The animation will start on frame 'startFrame' and consist
of 'length' frames. To use this animation on a gameObject with
a SpriteScript, either create one using the constructor and
passing it into AddAnimation or by defining it in the editor
(which will add it to the set of animations assignable using
SetAnimation automatically).