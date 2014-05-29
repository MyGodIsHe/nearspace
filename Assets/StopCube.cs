using UnityEngine;
using System.Collections;

public class StopCube : Cube {

	// Use this for initialization
	void Start () {
	}
	
	void FixedUpdate () {
		castByDirection(Vector3.forward);
		castByDirection(Vector3.back);
		castByDirection(Vector3.left);
		castByDirection(Vector3.right);
	}

	void castByDirection(Vector3 direction) {
		var fwd = transform.TransformDirection (direction);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, fwd, out hit)) {
			if (hit.collider.gameObject.CompareTag("Cube")) {
				hit.collider.gameObject.GetComponent<Cube>().Freeze(transform.position + direction * (hit.distance + 0.5f));
			}
		}
	}
}
