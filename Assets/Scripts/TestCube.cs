using UnityEngine;
using System.Collections;

public class TestCube : MonoBehaviour {

	public Vector3 punchDirection = Vector3.zero;
	public float punchTime = 1;
	public AudioClip collisionSound;

	float startTime;
	bool isPunch = false;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		if (punchDirection == Vector3.zero)
			isPunch = true;
		freeze();
	}

	void OnCollisionEnter2D(Collision2D other) {
		rigidbody2D.velocity = Vector3.zero;
		other.rigidbody.velocity = Vector3.zero;
		freeze();
		freeze(other);
	}

	void Update() {
		if (!isPunch && Time.time - startTime > punchTime) {
			isPunch = true;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, punchDirection, out hit)) {
				if (hit.distance > 0.5f) {
					unFreeze();
					rigidbody2D.AddForce(punchDirection * 500);
				}
			}
		}
	}
	
	static Vector3 round(Vector3 v) {
		return new Vector3 (Mathf.Round (v.x), Mathf.Round (v.y), v.z);
	}
	
	static void freeze(Collision2D obj) {
		obj.rigidbody.isKinematic = true;
		obj.transform.position = round(obj.transform.position);
	}

	void freeze() {
		rigidbody2D.isKinematic = true;
		transform.position = round(transform.position);
	}
	
	void unFreeze() {
		rigidbody2D.isKinematic = false;
	}
}
