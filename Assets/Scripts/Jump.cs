﻿using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			Player player = other.gameObject.GetComponent<Player>();
			player.Jump();
			// to next level
		}
	}
}