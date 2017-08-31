using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScript : MonoBehaviour {

	private SpriteRenderer sr;
	private float timer;
	private float framesPerSecond;
	private int index;
	private List<Sprite> spriteSheet = new List<Sprite>();
	private SpriteLookupScript lookup;

    public string spriteSheetName;

	public EditorAnimation[] editorAnimations;
	private Dictionary<string, Animation> animations;
	private Animation currentAnimation;

	void Awake () {
		lookup = GameObject.FindGameObjectWithTag("sprite_controller").GetComponent<SpriteLookupScript>();
		sr = GetComponent<SpriteRenderer>();
		timer = 0;
		framesPerSecond = 15;
		animations = new Dictionary<string, Animation>();
		foreach(EditorAnimation a in editorAnimations) {
			animations.Add(a.name, a.animation);
		}
		animations.Add("default", new Animation());
		SetAnimation("default");     
    }

    private void Start() {
        SetSpriteSheet(spriteSheetName);
    }

    void Update () {
		if (spriteSheet.Count > 0 && framesPerSecond > 0) {
			timer += Time.deltaTime;

			int newIndex;
			if (currentAnimation.length == -1) {
				newIndex = (int) ((timer * framesPerSecond) % spriteSheet.Count);
			} else {
				newIndex = (int) ((timer * framesPerSecond) % currentAnimation.length) + currentAnimation.startFrame;
			}
			if (newIndex != index) {
				index = newIndex;
				UpdateSprite();
			}
		}
	}

	// Sets the spritesheet to the texture in /Assets/Resources/Sprites/
	// named 'spriteName'
	public void SetSpriteSheet(string spriteName) {
		spriteSheet.Clear();
		foreach(Sprite s in lookup.GetSpriteSheet(spriteName)){
			spriteSheet.Add(s);
		}
		UpdateSprite();
	}

	// Sets the speed of the animation to the given float in frames per second.
	// e.g., if 5 is passed in, the frame will change 5 times every second.
	// if 0.5 is passed in, it will change once every two seconds.
	public void SetFramesPerSecond(float framesPerSecond) {
		//timer = 0; // Possibly too simple. This should maybe be re-buffed later.
		this.framesPerSecond = framesPerSecond;
		UpdateSprite();
	}

	// Manually sets the index of the frame to be shown.
	// On passing in a parameter n, will set the image to the nth frame of
	// the current animation.
	// Resets the timing on the current frame (such that the frame that it is
	// now set to will appear for the full amount of time that it should to
	// meet the correct frames per second).
	public void SetIndex(int index) {
		timer = 0;
		this.index = index;
		UpdateSprite();
	}

	private void UpdateSprite() {
		sr.sprite = spriteSheet[index % spriteSheet.Count];
	}

	// Sets the current animation to the animation associated with the name
	// passed in as an argument.
	// New animations can be added in the editor or by using AddAnimation.
	public void SetAnimation(string name) {
		currentAnimation = animations[name];
	}

    public void SetAnimation(string name, float fps) {
        SetFramesPerSecond(fps);
        currentAnimation = animations[name];
    }

    // Creates a new animation for this spritesheet with the data of
    // 'animation' and the name 'name'. It can be set as the active 
    public void AddAnimation(string name, Animation animation) {
		animations.Add(name, animation);
	}

	// Class for defining a custom animation for a sprite sheet.
	// The animation will start on frame 'startFrame' and consist
	// of 'length' frames. To use this animation on a gameObject with
	// a SpriteScript, either create one using the constructor and
	// passing it into AddAnimation or by defining it in the editor
	// (which will add it to the set of animations assignable using
	// SetAnimation automatically).
	[System.Serializable]
	public class Animation {
		public int startFrame;
		public int length;

		public Animation() : this(0, -1) {}

		public Animation(int startFrame, int length) {
			this.startFrame = startFrame;
			this.length = length;
		}
	}

	[System.Serializable]
	public class EditorAnimation {
		public string name;
		public Animation animation;
	}
}
