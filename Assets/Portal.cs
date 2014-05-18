using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour {

	public List<GameObject> portals;

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.Space)) {
			Player player = other.gameObject.GetComponent<Player>();
			int n = portals.IndexOf(this.gameObject);
			if (n == portals.Count - 1 )
				n = 0;
			else
				n++;
			GameObject portal = portals[n];
			player.Teleport(portal);
		}
	}
}
