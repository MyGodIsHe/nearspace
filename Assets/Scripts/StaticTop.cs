using UnityEngine;
using System.Collections;


public class StaticTop : MonoBehaviour {

	GameObject target;
	const float distance = 2;
	Vector3 center = Vector3.zero;
	Vector3 velocity = Vector3.zero;
	Rect range;
	Vector2 viewRange;
	GameController gameController;
	float maxRange;
	float targetOrthographicSize = 1;
	Vector3 baseView;

	void Awake() {
		gameController = Camera.main.GetComponent<GameController>();
		camera.orthographicSize = 1;
		baseView = camera.ViewportToWorldPoint(Vector3.one) - camera.ViewportToWorldPoint(Vector3.zero);
	}

	void Update () {
		if (target == null)
			return;
		Vector3 newPosition = target.rigidbody2D.transform.position;
		newPosition += gameController.GetViewDirection() * maxRange;

		if (newPosition.x < range.x + viewRange.x)
			newPosition.x = range.x + viewRange.x;
		else if (newPosition.x > range.x + range.width - viewRange.x)
			newPosition.x = range.x + range.width - viewRange.x;

		if (newPosition.y > range.y - viewRange.y)
			newPosition.y = range.y - viewRange.y;
		else if (newPosition.y < range.y - range.height + viewRange.y)
			newPosition.y = range.y - range.height + viewRange.y;

		center = Vector3.SmoothDamp(
			center,
			newPosition,
			ref velocity,
			0.15f);
		
		transform.position = center + Vector3.back * distance;

		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetOrthographicSize, 0.05f);
		//viewRange = new Vector2(baseView.x / 2, baseView.y / 2) * camera.orthographicSize;
	}
	
	public void SetTarget(GameObject target) {
		this.target = target;
		center = target.transform.position;
	}

	public void SetRange(Rect rect) {
		range = rect;
		maxRange = Mathf.Max(range.width, range.height);
	}
	
	public void SetViewRange(float roomSize) {
		float save = camera.orthographicSize;
		camera.orthographicSize = save;
		targetOrthographicSize = roomSize / Mathf.Max(baseView.x, baseView.y);
		viewRange = new Vector2(baseView.x / 2, baseView.y / 2) * targetOrthographicSize;
	}
}