using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D
{

	public class SwitchButton : MonoBehaviour
	{

		public bool isOn;
		public Sprite offSprite;
		public Sprite onSprite;
		public Path origin;
		public Path connectedToWhenOn;
		public Path connectedToWhenOff;

		private SpriteRenderer rend;

		void Awake()
		{
			rend = GetComponent<SpriteRenderer>();
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			//Debug.Log(1);
			if (other.tag != "Bullet")
				return;

			ChangeSprite();
			//Connect ();
		}

		void ChangeSprite()
		{
			isOn = !isOn;

			rend.sprite = isOn ? onSprite : offSprite;
		}

		void Connect()
		{
			if (isOn)
				origin.connected = connectedToWhenOn;
			else
				origin.connected = connectedToWhenOff;
		}
	}

}