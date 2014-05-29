using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {
	
	void OnTriggerEnter(Collider other) {
		Camera.main.GetComponent<Map>().UnlockRoom();
		Destroy(this.gameObject);
	}
}
