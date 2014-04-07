using UnityEngine;
using System.Collections;

public class EndCube : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<Player>().SetOrientation(Vector3.up);
		}
	}
}
