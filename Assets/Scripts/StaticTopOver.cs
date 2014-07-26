using UnityEngine;
using System.Collections;


public class StaticTopOver : MonoBehaviour {

	const float distance = 2;
	Vector3 center = Vector3.zero;
	Vector3 velocity = Vector3.zero;
	Vector3 newPosition = Vector3.zero;

	void Awake() {
		transform.position = center + Vector3.up * distance;
		transform.LookAt(center, Vector3.forward);
	}

	void FixedUpdate () {
		center = Vector3.SmoothDamp(
			center,
			newPosition,
			ref velocity,
			0.15f);
		
		transform.position = center + Vector3.up * distance;
		transform.LookAt(center, Vector3.forward);
	}
	
	public void SetTarget(GameObject target) {
		center = target.transform.position;
	}

	public void SetRange(Rect rect) {
		newPosition = new Vector3(rect.x + rect.width / 2, 0, rect.y - rect.height / 2);
	}
	
	public void SetViewRange(float roomSize) {
		camera.orthographicSize = 1;
		Vector3 view = camera.ViewportToWorldPoint(Vector3.one) - camera.ViewportToWorldPoint(Vector3.zero);
		camera.orthographicSize = roomSize / Mathf.Min(view.x, view.y);
	}
}