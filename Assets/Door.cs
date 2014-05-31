using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	bool isOpen = false;

	void Awake () {
		Open();
	}

	public void Open() {
		gameObject.tag = "Untagged";
		isOpen = true;
		renderer.enabled = false;
	}
	
	public void Close() {
		gameObject.tag = "Wall";
		isOpen = false;
		renderer.enabled = true;
	}
	
	void OnTriggerEnter(Collider other) {
		if (!isOpen && other.gameObject.tag == "Player") {
			other.GetComponent<Player>().OnTriggerWall(gameObject);
		}
	}
}
