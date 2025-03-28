using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D {

public class SwitchButton : MonoBehaviour {

	public bool isOn;
	public Sprite offSprite;
	public Sprite onSprite;
	public Path origin;
	public Path connectedToWhenOn;
	public Path connectedToWhenOff;

	private SpriteRenderer rend;

	void Awake () {
		rend = GetComponent<SpriteRenderer> ();
	}
	
	void OnCollisionEnter2D (Collision2D other) {
		if (other.collider.tag != "Player")
			return;

		ChangeSprite ();
		Connect ();
	}

	void ChangeSprite() {
		isOn = !isOn;

		rend.sprite = isOn ? onSprite : offSprite;
	}

	void Connect() {
		if (isOn)
			origin.connected = connectedToWhenOn;
		else
			origin.connected = connectedToWhenOff;
	}
}

}