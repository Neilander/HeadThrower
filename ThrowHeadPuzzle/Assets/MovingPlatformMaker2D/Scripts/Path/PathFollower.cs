using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D {

	[AddComponentMenu("Moving Platform Maker 2D/Path Follower")]
	public class PathFollower : MonoBehaviour {

		public bool active = true;
		public PathDirection direction = PathDirection.Forward;
		[Range(0f,20f)] public float speed = 1f;
		public float gravity = 1f;
		public bool lookAtNextWaypoint = false;

		private bool following = true;
		private bool isKinematic = false;
		private float progress = 0f;
		private Path path;
		private Vector3 target;
		private Vector3 velocity = Vector3.zero;
		private bool useGravity = true;

		void Reset() {
			Collider2D col = GetComponent<Collider2D>();
			if (col == null) {
				col = gameObject.AddComponent<BoxCollider2D> ();
				col.isTrigger = true;
			}

			Rigidbody2D body = GetComponent<Rigidbody2D> ();
			if (body == null) {
				body = gameObject.AddComponent<Rigidbody2D> ();
			}
			body.isKinematic = true;
		}

		void Awake () {
			Reset ();
			target = transform.position;
		}

		void Update () {
			if (!active)
				return;
			
			if (!isKinematic && useGravity) {
				SimulateGravity ();
			}

			transform.position += velocity * Time.deltaTime;

			if (!following || path == null || speed == 0) {
				return;
			}

			UpdateProgress ();
			LookAt ();
		}

		void SimulateGravity() {
			velocity += Vector3.down * gravity;
			velocity.y = Mathf.Max (velocity.y, -speed*2);
		}

		void UpdateProgress() {
			progress += (int)direction * Time.deltaTime * speed / path.Length;
			progress = Mathf.Clamp01(progress);

			if (progress == 0f || progress == 1f) {
				if (path.IsCyclic ())
					progress = 1f - progress;
				else if (path.Type == PathType.Connected && path.connected != null && progress == 1f) {
					target = path.connected.GetWorldPoints (path.connected.transform)[0];
					SetVelocity((target - transform.position).normalized * speed);
					following = false;
					useGravity = false;
				}
				else if(PathType.OneWay.Equals(path.Type)) {
					if(progress == 1f && PathDirection.Forward.Equals(direction)) {
						progress = 0f;
						Teleport(transform, target);
					}
					else if(progress == 0f && PathDirection.Backward.Equals(direction)) {
						progress = 1f;
						Teleport(transform, target);
					}
				}
				else
					ReverseOrFly ();
			}

			UpdatePosition ();
		}

		void Teleport(Transform transform, Vector3 target) {
			GameObject player = GameObject.FindWithTag("Player");
			if(player != null) {
				if(player.transform.IsChildOf(transform)) {
					player.transform.parent = null;
				}
			}
			transform.position = target;
		}

		void ReverseOrFly() {
			if ((progress == 0f && !path.openStart) || (progress == 1f && !path.openEnd))
				Reverse ();
			else
				Fly ();
		}

		void Fly() {
			SetVelocity (path.GetDirection(progress) * speed * (int) direction);
			following = false;
			useGravity = true;
		}

		public void Reverse() {
			direction = (direction == PathDirection.Forward) ? PathDirection.Backward : PathDirection.Forward;
		}

		void UpdatePosition() {
			if (target != transform.position) {
				target = path.GetPosition (progress);
				transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
			} else {
				transform.position = path.GetPosition(progress);
				target = transform.position;
			}
		}

		void LookAt() {
			if (lookAtNextWaypoint) {
				Vector3 localScale = transform.localScale;
				Vector3 target = path.GetNextWaypoint (progress, direction);
				if (target.x > transform.position.x && transform.localScale.x > 0)
					localScale.x = -localScale.x;
				else if (target.x < transform.position.x && transform.localScale.x < 0)
					localScale.x = -localScale.x;
				transform.localScale = localScale;
			}
		}

		void SetVelocity(Vector2 velocity) {
			this.velocity = velocity;
		}

		void OnTriggerEnter2D(Collider2D other) {
			if(path != null)
				return;

			path = other.GetComponent<Path>();
	        if(path != null) {
	    		following = true;
				isKinematic = true;

				if (transform.position.z != 0) {
					Debug.LogWarning ("The Z position of the object "+transform.name+" is not equals to 0. In order to avoid undesired behaviour it'll be set to 0.");
					transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
				}
				progress = path.GetProgress(transform.position);
				target = path.GetPosition (progress);

				CalculateIncomingDirection(progress);

				SetVelocity(Vector2.zero);
	        }
		}

		void OnTriggerExit2D(Collider2D other) {
			if (other.GetComponent<Path> () == path) {
				path = null;
				isKinematic = false;
			}
		}
		
		//Calcula a direção que a plataforma está entrando no path
		void CalculateIncomingDirection(float enterProgress) {
			float dot = Vector2.Dot (velocity, path.GetDirection (enterProgress));
			if (dot > 0)
				direction = PathDirection.Forward;
			else if (dot < 0)
				direction = PathDirection.Backward;
		}

	}

}