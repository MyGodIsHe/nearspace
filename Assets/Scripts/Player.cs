using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	GameObject attached;
	Vector3 prevuseOrientation = Vector3.zero;
	Vector3 orientation;

	Vector3 newPosition;
	GameController gameController;
	Light lightsource;
	GameGUI gameGUI;

	Vector3 from;
	const float EXPLOSION_FORCE = 200;
	const float ALTITUDE = 0.75f;
	
	void Awake() {
		gameGUI = Camera.main.GetComponent<GameGUI>();
		gameController = Camera.main.GetComponent<GameController>();
		lightsource = GetComponentInChildren<Light>();
		lightsource.enabled = false;
	}

	void Update () {
		if (State.IsPause)
			return;
		if (attached) {
			updateOrientation();

			transform.position = Vector3.MoveTowards(
				transform.position,
				attached.transform.position + orientation * ALTITUDE,
				0.15f);

			if (Input.GetKeyUp(KeyCode.Space) && orientation != Vector3.back) {
				if (!attached.rigidbody2D.isKinematic &&
				    (attached.rigidbody2D.velocity == Vector2.zero ||
				     orientation != Vector3.zero)) {
					RaycastHit2D hit = Physics2D.Raycast(attached.transform.position - orientation * Cube.WORLD_HALF_SIZE, -orientation);
					if (hit && hit.fraction > 0.1f) {
						attached.rigidbody2D.AddForce(-orientation * EXPLOSION_FORCE);
					}
				}
				Jump();
			}
		}

		fixParticles();
		animateDetonate();
	}

	void animateDetonate() {
		float progress = gameController.GetDetonateProgress();
		if (progress > 0) {
			lightsource.enabled = true;
			lightsource.intensity = progress * 10;
		} else {
			lightsource.enabled = false;
		}
	}

	void fixParticles() {
		Vector2 localVelocity = attached ? attached.rigidbody2D.velocity: Vector2.zero;
		foreach (Transform obj in GetComponentInChildren<Transform>()) {
			if (obj.name == "Tail") {
				foreach (ParticleEmitter pe in obj.GetComponentsInChildren<ParticleEmitter>()) {
					pe.localVelocity = localVelocity;
				}
			}
		}
	}

	void updateOrientation() {
		Vector3 save = orientation;
		if (Input.GetKeyDown (KeyCode.A)) {
			if (orientation == Vector3.right) {
				//orientation = Vector3.back;
			} else {
				orientation = Vector3.left;
			}
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			if (orientation == Vector3.down) {
				//orientation = Vector3.back;
			} else {
				orientation = Vector3.up;
			}
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			if (orientation == Vector3.left) {
				//orientation = Vector3.back;
			} else {
				orientation = Vector3.right;
			}
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			if (orientation == Vector3.up) {
				//orientation = Vector3.back;
			} else {
				orientation = Vector3.down;
			}
		}
		if (save != orientation) {
			prevuseOrientation = save;
			if (orientation != Vector3.back) {
				RaycastHit2D hit = Physics2D.Raycast(attached.transform.position + orientation * Cube.WORLD_HALF_SIZE, orientation);
				if (hit && hit.fraction < 0.1f) {
					switch(hit.collider.gameObject.tag) {
					case "Cube":
						Attach (hit.collider.gameObject);
						orientation = save;
						break;
					default:
						orientation = save;
						break;
					}
				}
			}
		}
	}
	
	public void Jump() {
		transform.position = attached.transform.position + orientation * ALTITUDE;
		rigidbody2D.isKinematic = false;
		rigidbody2D.AddForce(orientation * EXPLOSION_FORCE * 2);
		attached.GetComponent<Cube>().UnAttach();
		attached = null;
		gameGUI.Life--;
	}
	
	public void Teleport(Vector3 position) {
		rigidbody2D.isKinematic = false;
		attached.GetComponent<Cube>().UnAttach();
		attached = null;
		transform.position = position + Vector3.back * 0.5f;
	}
	
	public void Teleport(GameObject portal) {
		rigidbody2D.isKinematic = false;
		attached.GetComponent<Cube>().UnAttach();
		attached = null;
		transform.position = portal.transform.parent.transform.position + Vector3.back * 0.5f;
	}

	public void AttachedDestroy() {
		rigidbody2D.isKinematic = false;
		rigidbody2D.velocity = attached.rigidbody2D.velocity;
		attached = null;
	}

	public void OnTriggerWall(GameObject wall) {
		if (attached)
			orientation = prevuseOrientation;
		else {
			Vector3 normal = wall.transform.rotation * Vector3.right;
			rigidbody2D.velocity = Vector3.Reflect(rigidbody2D.velocity, normal);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Cube" && attached != other.gameObject) {
			Attach (other.gameObject);
		}
	}

	Vector3 FindOrientation(Vector3 targetPosition) {
		Vector3 colVector = transform.position - targetPosition;
		Vector3[] sides = new Vector3[] {Vector3.back, Vector3.up, Vector3.left, Vector3.right, Vector3.down};
		Vector3 answer = Vector3.back;
		float min = Vector3.SqrMagnitude(answer - colVector);
		foreach (Vector3 v in sides) {
			float m = Vector3.SqrMagnitude(v - colVector);
			if (min > m) {
				min = m;
				answer = v;
			}
		}
		return answer;
	}

	public void SetOrientation(Vector3 value) {
		orientation = value;
	}

	public void Attach(GameObject target) {
		rigidbody2D.isKinematic = true;
		if (attached != null)
			attached.GetComponent<Cube>().UnAttach();
		attached = target;
		orientation = FindOrientation (target.transform.position);
		from = transform.position - attached.transform.position;
		from.Normalize ();
		prevuseOrientation = Vector3.zero;
	}
}
