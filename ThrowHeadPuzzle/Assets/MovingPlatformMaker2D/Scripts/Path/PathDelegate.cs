using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D {

	/**
	 * This class was created to be independent of Unity framework and be easily tested with unit tests.
	 * @see PathDelegateTest
	 */ 
	public class PathDelegate {

		public bool cyclic = false;

		private const float searchIncrement = 0.01f;

		private Vector2[] directions = new Vector2[0];
		private float[] magnitudes = new float[0];

		public Vector2[] Directions {
			get {
				return directions;
			}
		}

		public float[] Magnitudes {
			get {
				return magnitudes;
			}
		}

		public float Length {
			get {
				float length = 0f;
				for (int i = 0; i < magnitudes.Length; i++)
					length += magnitudes [i];
				return length;
			}
		}

		public void CalculateDirectionsAndMagnitudes(Vector2[] points) {
			magnitudes = new float[points.Length];
			directions = new Vector2[points.Length];

			int last = points.Length - 1;

			for (int i = 0; i < last; i++) {
				directions [i] = points [i + 1] - points [i];
				magnitudes [i] = directions [i].magnitude;
			}

			if (cyclic) {
				directions [last] = points [0] - points [last];
				magnitudes [last] = directions [last].magnitude;
			} else {
				directions [last] = directions [last - 1]; //Repeat the last one. It's used when the platform flies.
				magnitudes [last] = 0f;
			}
		}

		public float GetProgress(Vector2[] points, Vector2 other, EasingCurve easing) {
			float minDistance = 99999999f;
			float minPosition = 0f;

			for (float i = 0f; i <= 1f; i += searchIncrement) {
				Vector2 pos = GetPosition (points, i, easing);
				float distance = (pos - other).sqrMagnitude;
				if (distance < minDistance) {
					minPosition = i;
					minDistance = distance;
				}
			}

			return Utils.Round2(minPosition);
		}

		public Vector2 GetPosition(Vector2[] points, float progress, EasingCurve easing) {
			progress = Mathf.Clamp01(progress);

			Delta delta = GetCurrentDelta (progress, PathDirection.Forward);

			Vector2 current = points [delta.index];

			if (magnitudes [delta.index] == 0)
				return current;
			
			int nextIndex = GetNextIndex(delta.index);
			Vector2 next = points [nextIndex];

			float offset = delta.offset / magnitudes [delta.index];
			offset = easing.Evaluate (offset);

			return Vector2.Lerp(current, next, offset);
		}

		public Vector2 GetDirection(float progress) {
			Delta delta = GetCurrentDelta (progress, PathDirection.Forward);
			return directions [delta.index].normalized;
		}

		public Vector2 GetNextWaypoint(Vector2[] points, float progress, PathDirection direction) {
			Delta delta = GetCurrentDelta (progress, direction);

			if (direction == PathDirection.Forward)
				return points [GetNextIndex (delta.index)];
			else
				return points [GetPreviowsIndex (delta.index)];
		}

		int GetNextIndex(int index) {
			int nextIndex = (index + 1) % magnitudes.Length;
			if (nextIndex == 0 && !cyclic)
				nextIndex = index-1;
			return nextIndex;
		}

		public int GetPreviowsIndex(int index) {
			int nextIndex = (index - 1) % magnitudes.Length;
			if (nextIndex < 0) {
				if (!cyclic)
					nextIndex = index + 1;
				else
					nextIndex = magnitudes.Length - 1;
			}
			return nextIndex;
		}

		public Delta GetCurrentDelta(float progress, PathDirection direction) {
			
				float offset = Mathf.Clamp01 (progress) * Length;

				int index = 0;

				while (index < magnitudes.Length) {
					if (magnitudes [index] == 0f)
						break;
				
					if (offset < magnitudes [index])
						break;

					offset -= magnitudes [index];
					index++;
				}

			index %= magnitudes.Length;

			if (direction == PathDirection.Backward && index < magnitudes.Length-1 && !cyclic) {
				// Walk backwards when fell into a triangle on the left side
				index++;
			}

			return new Delta (index, Utils.Round2 (offset));
		}

		public Vector2[] ChangeType(PathType type, Vector2[] points) {
			if (PathType.Cyclic.Equals(type)) {
				return ClosePath (points);
			} else {
				return OpenPath (points);
			}
		}

		Vector2[] ClosePath(Vector2[] points) {
			if (points [0].Equals (points [points.Length - 1]))
				return points;

			List<Vector2> newPoints = new List<Vector2> ();
			for(int i = 0; i < points.Length; i++)
				newPoints.Add (points[i]);
			newPoints.Add(points[0]);

			return newPoints.ToArray();
		}

		Vector2[] OpenPath(Vector2[] points) {
			if (! points [0].Equals (points [points.Length - 1]))
				return points;

			List<Vector2> newPoints = new List<Vector2> ();
			for(int i = 0; i < points.Length-1; i++)
				newPoints.Add (points[i]);

			return newPoints.ToArray();
		}

		public struct Delta {
			public int index;
			public float offset;

			public Delta(int index, float offset) {
				this.index = index;
				this.offset = offset;
			}
		}
	}

}