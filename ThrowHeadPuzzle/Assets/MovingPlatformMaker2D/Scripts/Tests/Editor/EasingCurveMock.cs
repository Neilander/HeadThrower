using System;
using UnityEngine;

namespace MovingPlatformMaker2D {

class EasingCurveMock : EasingCurve {

	private float value = 0f;

	public EasingCurveMock(float value) {
		this.value = value;
	}

	float Ease(float easing, float x) {
		float a = easing + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}

	public float Evaluate(float time) {
		return Ease(value, time);
	}
}

}