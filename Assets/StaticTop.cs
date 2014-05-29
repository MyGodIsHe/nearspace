using UnityEngine;
using System.Collections;


public class StaticTop : MonoBehaviour {

	GameObject target;
	public float distance = 5;
	Vector3 center = Vector3.zero;
	Vector3 velocity = Vector3.zero;
	Rect range;
	Vector2 viewRange;

	void Awake() {
		transform.position = center + Vector3.up * distance;
		transform.LookAt(center, Vector3.forward);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 newPosition = target.transform.position;
		if (newPosition.x < range.x + viewRange.x)
			newPosition.x = range.x + viewRange.x;
		else if (newPosition.x > range.x + range.width - viewRange.x)
			newPosition.x = range.x + range.width - viewRange.x;

		if (newPosition.z > range.y - viewRange.y)
			newPosition.z = range.y - viewRange.y;
		else if (newPosition.z < range.y - range.height + viewRange.y)
			newPosition.z = range.y - range.height + viewRange.y;

		center = Vector3.SmoothDamp(
			center,
			newPosition,
			ref velocity,
			0.15f);
		
		transform.position = center + Vector3.up * distance;
		transform.LookAt(center, Vector3.forward);
	}
	
	public void SetTarget(GameObject target) {
		this.target = target;
		center = target.transform.position;
	}

	public void SetRange(Rect rect) {
		range = rect;
	}
	
	public void SetViewRange(float roomSize) {
		camera.orthographicSize = 1;
		Vector3 view = camera.ViewportToWorldPoint(Vector3.one) - camera.ViewportToWorldPoint(Vector3.zero);
		camera.orthographicSize = roomSize / Mathf.Max(view.x, view.z);
		viewRange = new Vector2(view.x / 2, view.z / 2) * camera.orthographicSize;
	}
}