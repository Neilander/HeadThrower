using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D {

	public class CameraScript : MonoBehaviour {

		public Transform target;
		public bool followY = true;
		private Vector3 pos;

		void Start(){
			pos = transform.position;
		}

		void Update(){
			float newX = Mathf.SmoothStep(pos.x, target.position.x, Time.timeSinceLevelLoad );
			float newY = pos.y;
			if(followY)
				newY = Mathf.SmoothStep(pos.y, target.position.y, Time.timeSinceLevelLoad );

			transform.position = new Vector3(newX, newY, pos.z);
		}
	}

}