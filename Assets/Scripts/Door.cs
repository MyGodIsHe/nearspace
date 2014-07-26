using UnityEngine;
using System.Collections;

public class Door : Wall {

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
	
	new void OnTriggerEnter2D(Collider2D other) {
		if (!isOpen) {
			base.OnTriggerEnter2D(other);
		}
	}
}
