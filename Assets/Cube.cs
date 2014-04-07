using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {

	public AudioClip collisionSound;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	static Vector3 round(Vector3 v) {
		return new Vector3 (Mathf.Round (v.x),
		                    Mathf.Round (v.y),
		                    Mathf.Round (v.z));
	}

	static void stop(GameObject obj) {
		obj.rigidbody.velocity = Vector3.zero;
		obj.transform.position = round(obj.transform.position);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Cube") {
			/*float magnitude = Mathf.Max(gameObject.rigidbody.velocity.sqrMagnitude,
			                            other.gameObject.rigidbody.velocity.sqrMagnitude);
			if (magnitude > 0.4f)*/
			audio.PlayOneShot(collisionSound);
			stop(gameObject);
			stop(other.gameObject);
		}
	}
}
