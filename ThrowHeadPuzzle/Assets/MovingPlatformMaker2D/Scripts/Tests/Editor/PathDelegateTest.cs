using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace MovingPlatformMaker2D {

/**
 * Unit tests of the PathDelegate class.
 * 
 * UnityTestTools is required to run the tests. Download it on the link below. Ctrl+T to run the tests.
 * https://www.assetstore.unity3d.com/en/#!/content/13802
 */ 

[TestFixture]
public class PathDelegateTest {

	Vector2[] points = new Vector2[] {
		new Vector2(0, 0),
		new Vector2(0, 1),
		new Vector2(1, 1),
		new Vector2(1, 0)
	};

	Vector2[] cyclicPoints = new Vector2[] {
		new Vector2(0, 0),
		new Vector2(0, 1),
		new Vector2(1, 1),
		new Vector2(1, 0),
		new Vector2(0, 0)
	};

	[Test]
	public void CalculateDirections() {
		PathDelegate path = Create ();

		Assert.AreEqual (4, path.Directions.Length);
		Assert.AreEqual (new Vector2 (0, 1), path.Directions [0]);
		Assert.AreEqual (new Vector2 (1, 0), path.Directions [1]);
		Assert.AreEqual (new Vector2 (0, -1), path.Directions [2]);
		Assert.AreEqual (new Vector2 (0, -1), path.Directions [3]); 
	}

	[Test]
	public void CalculateDirectionsCyclic() {
		PathDelegate path = CreateCyclic ();

		Assert.AreEqual (5, path.Directions.Length);
		Assert.AreEqual (new Vector2 (0, 1), path.Directions [0]);
		Assert.AreEqual (new Vector2 (1, 0), path.Directions [1]);
		Assert.AreEqual (new Vector2 (0, -1), path.Directions [2]);
		Assert.AreEqual (new Vector2 (-1, 0), path.Directions [3]);
		Assert.AreEqual (new Vector2 (0, 0), path.Directions [4]);
	}

	[Test]
	public void CalculateMagnitudes() {
		PathDelegate path = Create();

		Assert.AreEqual (4, path.Magnitudes.Length);
		Assert.AreEqual (1f, path.Magnitudes [0]);
		Assert.AreEqual (1f, path.Magnitudes [1]);
		Assert.AreEqual (1f, path.Magnitudes [2]);
		Assert.AreEqual (0f, path.Magnitudes [3]);
	}

	[Test]
	public void CalculateMagnitudesCyclic() {
		PathDelegate path = CreateCyclic();

		Assert.AreEqual (5, path.Magnitudes.Length);
		Assert.AreEqual (1f, path.Magnitudes [0]);
		Assert.AreEqual (1f, path.Magnitudes [1]);
		Assert.AreEqual (1f, path.Magnitudes [2]);
		Assert.AreEqual (1f, path.Magnitudes [3]);
		Assert.AreEqual (0f, path.Magnitudes [4]);
	}

	[Test]
	public void Length() {
		PathDelegate path = Create ();

		Assert.AreEqual (3f, path.Length);
	}

	[Test]
	public void LengthCyclic() {
		PathDelegate path = CreateCyclic ();

		Assert.AreEqual (4f, path.Length);
	}

	[Test]
	public void GetPosition_Progress0() {
		PathDelegate path = Create ();

		Vector2 position = path.GetPosition (points, 0f, new EasingCurveMock (0f));

		Assert.AreEqual (Vector2.zero, position);
	}

	[Test]
	public void GetPosition_Progress025() {
		PathDelegate path = CreateCyclic ();

		Vector2 position = path.GetPosition (points, 0.25f, new EasingCurveMock (0f));

		Assert.AreEqual (new Vector2(0f, 1f), position);
	}

	[Test]
	public void GetPosition_Progress1() {
		PathDelegate path = Create ();

		Vector2 position = path.GetPosition (points, 1f, new EasingCurveMock (1f));

		Assert.AreEqual (new Vector2(1f, 0f), position);
	}

	[Test]
	public void GetPosition_Progress1_Cyclic() {
		PathDelegate path = CreateCyclic();

		Vector2 position = path.GetPosition (cyclicPoints, 1f, new EasingCurveMock (1f));

		Assert.AreEqual (new Vector2(0f, 0f), position);
	}

	[Test]
	public void GetPosition_ProgressHalf() {
		PathDelegate path = Create ();

		PathDelegate.Delta delta = path.GetCurrentDelta (0.5f, PathDirection.Forward);
		Assert.AreEqual (1, delta.index);
		Assert.AreEqual (0.5f, delta.offset);

		Vector2 position = path.GetPosition (points, 0.5f, new EasingCurveMock (0f));

		Assert.AreEqual (new Vector2(0.5f, 1f), position);
	}

	[Test]
	public void GetPosition_Progress125() {
		PathDelegate path = Create ();

		Vector2 position = path.GetPosition (points, 0.1f, new EasingCurveMock (0f));

		Assert.AreEqual (new Vector2(0f, 0.3f), position);
	}

	[Test]
	public void GetPosition_NegativeProgress() {
		PathDelegate path = Create ();

		Vector2 position = path.GetPosition (points, -0.1f, new EasingCurveMock (1f));

		Assert.AreEqual (Vector2.zero, position);
	}

	[Test]
	public void GetPosition_ProgressHigherThan1() {
		PathDelegate path = Create ();

		Vector2 position = path.GetPosition (points, 1.1f, new EasingCurveMock (1f));

		Assert.AreEqual (new Vector2(1f, 0f), position);
	}

	[Test] 
	public void GetCurrentDelta_Progress0() {
		PathDelegate path = Create ();

		PathDelegate.Delta delta = path.GetCurrentDelta (0f, PathDirection.Forward);

		Assert.AreEqual (0, delta.index);
		Assert.AreEqual (0f, delta.offset);
	}

	[Test] 
	public void GetCurrentDelta_Progress1() {
		PathDelegate path = Create ();

		PathDelegate.Delta delta = path.GetCurrentDelta (1f, PathDirection.Backward);

		Assert.AreEqual (3, delta.index);
		Assert.AreEqual (0f, delta.offset);
	}

	[Test]
	public void GetProgress_0() {
		PathDelegate path = Create ();

		float progress = path.GetProgress (points, new Vector2(0f, 0f), new EasingCurveMock (0f));

		Assert.AreEqual (0f, progress);
	}

	[Test]
	public void GetProgress_1() {
		PathDelegate path = Create ();

		float progress = path.GetProgress (points, new Vector2(1f, 0f), new EasingCurveMock (0f));

		Assert.AreEqual (1f, progress);
	}

	[Test]
	public void GetProgress_1_Cyclic() {
		PathDelegate path = CreateCyclic ();

		float progress = path.GetProgress (cyclicPoints, new Vector2(1f, 0f), new EasingCurveMock (0f));

		Assert.AreEqual (0.75f, progress);
	}

	[Test]
	public void GetNextWaypoint_0() {
		PathDelegate path = Create ();

		Vector2 next = path.GetNextWaypoint (points, 1f, PathDirection.Forward);

		Assert.AreEqual (new Vector2 (1f, 1f), next);
	}

	[Test]
	public void GetNextWaypoint_025() {
		PathDelegate path = CreateCyclic ();

		Vector2 next = path.GetNextWaypoint (points, 0.25f, PathDirection.Forward);

		Assert.AreEqual (new Vector2 (1f, 1f), next);
	}

	[Test]
	public void GetNextWaypoint_1() {
		PathDelegate path = Create ();

		Vector2 next = path.GetNextWaypoint (points, 1f, PathDirection.Backward);

		Assert.AreEqual (new Vector2 (1f, 1f), next);
	}

	[Test]
	public void GetNextWaypoint_Cyclic() {
		PathDelegate path = CreateCyclic ();

		Vector2 next = path.GetNextWaypoint (cyclicPoints, 0.75f, PathDirection.Forward);

		Assert.AreEqual (new Vector2 (0f, 0f), next);
	}

	[Test]
	public void GetNextWaypoint_Cyclic_Previows() {
		PathDelegate path = CreateCyclic ();

		Vector2 next = path.GetNextWaypoint (cyclicPoints, 0f, PathDirection.Backward);
		Assert.AreEqual (new Vector2 (0f, 0f), next);

		Vector2 next2 = path.GetNextWaypoint (cyclicPoints, 1f, PathDirection.Backward);
		Assert.AreEqual (new Vector2 (1f, 0f), next2);
	}

	[Test]
	public void GetDirection() {
		PathDelegate path = CreateCyclic ();

		Assert.AreEqual (new Vector2 (0f, 1f), path.GetDirection (0f));
		Assert.AreEqual (new Vector2 (1f, 0f), path.GetDirection (0.25f));
		Assert.AreEqual (new Vector2 (0f, -1f), path.GetDirection (0.5f));
		Assert.AreEqual (new Vector2 (-1f, 0f), path.GetDirection (0.75f));
	}

	[Test]
	public void TwoPoints() {

		Vector2[] twoPoints = new Vector2[] {
			new Vector2(0f,0f),
			new Vector2(10f,0f)
		};
		PathDelegate path = new PathDelegate ();
		path.CalculateDirectionsAndMagnitudes (twoPoints);

		Vector2 position = path.GetPosition (twoPoints, 0.2f, new EasingCurveMock (0f));

		Assert.AreEqual (new Vector2 (2f, 0f), position);
	}

	[Test]
	public void TwoPoints_Ease1() {

		Vector2[] twoPoints = new Vector2[] {
			new Vector2(0f,0f),
			new Vector2(10f,0f)
		};
		PathDelegate path = new PathDelegate ();
		path.CalculateDirectionsAndMagnitudes (twoPoints);

		Vector2 position = path.GetPosition (twoPoints, 0.3f, new EasingCurveMock (1f));

		Assert.IsTrue (1.55f.Equals(Utils.Round2(position.x)));
	}

	[Test]
	public void TwoPoints_Ease1_Middle() {

		Vector2[] twoPoints = new Vector2[] {
			new Vector2(0f,0f),
			new Vector2(10f,0f)
		};
		PathDelegate path = new PathDelegate ();
		path.CalculateDirectionsAndMagnitudes (twoPoints);

		Vector2 position = path.GetPosition (twoPoints, 0.5f, new EasingCurveMock (1f));

		Assert.AreEqual (new Vector2 (5f, 0f), position);
	}

	[Test]
	public void EnterAtTheEnd() {
		PathDelegate path = Create ();

		PathDelegate.Delta delta = path.GetCurrentDelta (1f, PathDirection.Backward);
		Vector2 next = path.GetNextWaypoint (points, 1f, PathDirection.Backward);
		float progress = path.GetProgress (points, next, new EasingCurveMock (0f));

		Assert.AreEqual (3, delta.index);
		Assert.AreEqual (0f, delta.offset);
		Assert.AreEqual (new Vector2 (1f, 1f), next);
		Assert.AreEqual (0.67f, progress);
	}

	[Test]
	public void GetNextWaypoint_OfTwo() {
		Vector2[] twoPoints = new Vector2[] {
			new Vector2(0f,0f),
			new Vector2(10f,10f)
		};
		PathDelegate path = new PathDelegate ();
		path.CalculateDirectionsAndMagnitudes (twoPoints);

		PathDelegate.Delta delta = path.GetCurrentDelta (0.5f, PathDirection.Backward);
		Vector2 next = path.GetNextWaypoint (twoPoints, 0.5f, PathDirection.Backward);

		Assert.AreEqual (1, delta.index);
		Assert.AreEqual (7.07f, delta.offset);
		Assert.AreEqual (new Vector2 (0f, 0f), next);
	}

	[Test]
	public void GetNextWaypoint_OfThree() {
		Vector2[] twoPoints = new Vector2[] {
			new Vector2(0f,0f),
			new Vector2(10f,10f),
			new Vector2(20f,0f)
		};
		PathDelegate path = new PathDelegate ();
		path.CalculateDirectionsAndMagnitudes (twoPoints);

		PathDelegate.Delta delta = path.GetCurrentDelta (0.25f, PathDirection.Backward);
		Vector2 next = path.GetNextWaypoint (twoPoints, 0.25f, PathDirection.Backward);

		Assert.AreEqual (1, delta.index);
		Assert.AreEqual (7.07f, delta.offset);
		Assert.AreEqual (new Vector2 (0f, 0f), next);
	}

	[Test]
	public void GetMagnitudeWhenEmpty() {
		PathDelegate path = new PathDelegate ();

		Assert.AreEqual (0, path.Magnitudes.Length);
	}

	[Test]
	public void GetLengthWhenEmpty() {
		PathDelegate path = new PathDelegate ();

		Assert.AreEqual (0, path.Length);
	}

	[Test]
	public void ChangeTypeFromPingPongToCyclic() {
		PathDelegate path = Create ();

		Vector2[] newPoints = path.ChangeType (PathType.Cyclic, points);

		Assert.AreEqual (5, newPoints.Length);
		Assert.AreEqual (newPoints[0], newPoints[newPoints.Length-1]);
	}

	[Test]
	public void ChangeTypeFromCyclicToPingPong() {
		PathDelegate path = CreateCyclic ();

		Vector2[] newPoints = path.ChangeType (PathType.PingPong, cyclicPoints);

		Assert.AreEqual (4, newPoints.Length);
		Assert.AreNotEqual (newPoints[0], newPoints[newPoints.Length-1]);
	}

	PathDelegate Create() {
		PathDelegate path = new PathDelegate ();
		path.CalculateDirectionsAndMagnitudes (points);
		return path;
	}

	PathDelegate CreateCyclic() {
		PathDelegate path = new PathDelegate ();
		path.cyclic = true;
		path.CalculateDirectionsAndMagnitudes (cyclicPoints);
		return path;
	}

}

}