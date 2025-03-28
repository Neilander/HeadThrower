using System;

namespace MovingPlatformMaker2D {

	public interface EasingCurve {
		float Evaluate(float time);
	}

}