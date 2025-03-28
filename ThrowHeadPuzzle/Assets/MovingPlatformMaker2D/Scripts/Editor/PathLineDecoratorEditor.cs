using UnityEditor;
using UnityEngine;

namespace MovingPlatformMaker2D {

	[CustomEditor(typeof(PathLineDecorator))]
	public class PathLineDecoratorEditor : Editor {

		private PathLineDecorator decorator;

		void OnSceneGUI() {
			if (decorator == null)
				decorator = target as PathLineDecorator;

			decorator.UpdateLines ();
		}

	}

}