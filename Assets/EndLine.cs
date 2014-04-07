using UnityEngine;
using System.Collections;

public class EndLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player")
			Camera.main.GetComponent<Map> ().RestartLevel ();
		else
			Destroy (other.gameObject);
	}
}
