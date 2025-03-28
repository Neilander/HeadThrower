using System;
using UnityEngine;

namespace MovingPlatformMaker2D {

	public class AnimationEasingCurve : EasingCurve {

		private AnimationCurve curve;

		public AnimationEasingCurve() {
		}

		public void SetCurve(AnimationCurve curve) {
			this.curve = curve;
		}

		public float Evaluate(float time) {
			return curve.Evaluate(time);
		}
	}

}