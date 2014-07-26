using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	protected void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.GetComponent<Player>().OnTriggerWall(gameObject);
		}
	}
}
