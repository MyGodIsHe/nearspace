using UnityEngine;
using System.Collections;


public class FollowTop : MonoBehaviour {

	private GameObject target;
	readonly float distance = 5;
	private Vector3 center;
	private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!target)
			return;

		center = Vector3.SmoothDamp(
			center,
			target.transform.position,
			ref velocity,
			0.15f);

		transform.position = center + Vector3.up * distance;
		transform.LookAt(target.transform.position, Vector3.forward);
	}

	public void SetTarget(GameObject target) {
		this.target = target;
		center = target.transform.position;
	}
}