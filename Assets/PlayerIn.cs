using UnityEngine;
using System.Collections;

public class PlayerIn : MonoBehaviour {

	private GameObject attached;
	private Vector3 orientation;

	private Vector3 newPosition;

	private Vector3 from;
	const float EXPLOSION_FORCE = 200;

	
	void Update () {
		if (Input.GetKey (KeyCode.Escape))
			Application.Quit ();
		if (attached) {
			updateOrientation();

			from = Vector3.RotateTowards(from, orientation, Time.deltaTime * 5, Time.deltaTime);
			transform.position = attached.transform.position + from;

			if (Input.GetKeyUp(KeyCode.Space) && orientation != Vector3.up) {
				attached.rigidbody.AddForce(-orientation * EXPLOSION_FORCE);
				Jump();
			}
		} else if (rigidbody.isKinematic) {
			Camera.main.GetComponent<Map> ().RestartLevel ();
		}


		fixParticles ();
	}

	void fixParticles() {
		Vector3 localVelocity = attached ? attached.rigidbody.velocity: Vector3.zero;
		foreach (ParticleEmitter pe in GetComponentsInChildren<ParticleEmitter>()) {
			pe.localVelocity = localVelocity;
		}
	}

	void updateOrientation() {
		if (Input.GetKeyUp (KeyCode.A)) {
			if (orientation == Vector3.right) {
				orientation = Vector3.up;
			} else {
				orientation = Vector3.left;
			}
		}
		if (Input.GetKeyUp (KeyCode.W)) {
			if (orientation == Vector3.back) {
				orientation = Vector3.up;
			} else {
				orientation = Vector3.forward;
			}
		}
		if (Input.GetKeyUp (KeyCode.D)) {
			if (orientation == Vector3.left) {
				orientation = Vector3.up;
			} else {
				orientation = Vector3.right;
			}
		}
		if (Input.GetKeyUp (KeyCode.S)) {
			if (orientation == Vector3.forward) {
				orientation = Vector3.up;
			} else {
				orientation = Vector3.back;
			}
		}
	}
	
	public void Jump() {
		transform.position = attached.transform.position + orientation;
		rigidbody.isKinematic = false;
		rigidbody.velocity = attached.rigidbody.velocity;
		rigidbody.AddForce(orientation * EXPLOSION_FORCE * 2);
		attached = null;
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Cube") {
			//if (!attached)
			//	other.gameObject.rigidbody.AddForce(rigidbody.velocity * 10);
			Attach (other.gameObject);
		}
	}

	Vector3 FindOrientation(Vector3 targetPosition) {
		Vector3 colVector = transform.position - targetPosition;
		Vector3[] sides = new Vector3[] {Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.down};
		Vector3 answer = Vector3.up;
		float min = Vector3.SqrMagnitude(answer - colVector);
		foreach (Vector3 v in sides) {
			float m = Vector3.SqrMagnitude(v - colVector);
			if (min > m) {
				min = m;
				answer = v;
			}
		}
		if (answer == Vector3.down)
			answer = Vector3.up;
		return answer;
	}

	public void SetOrientation(Vector3 value) {
		orientation = value;
	}

	public void Attach(GameObject target) {
		rigidbody.isKinematic = true;
		attached = target;
		orientation = FindOrientation (target.transform.position);
		from = transform.position - attached.transform.position;
	}
}
