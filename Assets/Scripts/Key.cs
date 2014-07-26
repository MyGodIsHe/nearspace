using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {
	
	void OnTriggerEnter2D(Collider2D other) {
		Camera.main.GetComponent<Map>().UnlockRoom();
		Destroy(this.gameObject);
	}
}
