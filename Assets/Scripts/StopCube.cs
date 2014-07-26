using UnityEngine;
using System.Collections;

public class StopCube : Cube {

	public GameObject rayPrefab;

	Ray[] rays = new Ray[4];

	// Use this for initialization
	void Start () {
		rays[0] = new Ray("LeftRay", Vector3.left, transform, rayPrefab);
		rays[1] = new Ray("RightRay", Vector3.right, transform, rayPrefab);
		rays[2] = new Ray("UpRay", Vector3.up, transform, rayPrefab);
		rays[3] = new Ray("DownRay", Vector3.down, transform, rayPrefab);
	}

	void FixedUpdate () {
		foreach(Ray ray in rays)
			ray.Cast();
	}

	class Ray {

		Vector3 direction;
		Transform transform;
		GameObject ray;

		public Ray(string name, Vector3 direction, Transform transform, GameObject prefab) {
			ray = Instantiate(prefab) as GameObject;
			ray.name = name;
			ray.transform.parent = transform;
			float angle = Vector3.Angle(Vector3.right, direction);
			float sign = Mathf.Sign(Vector3.Dot(Vector3.right, direction));
			ray.transform.rotation = Quaternion.Euler(0, 0, sign * angle);
			this.direction = direction;
			this.transform = transform;
		}
		
		public void Cast() {
			var fwd = transform.TransformDirection (direction);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, fwd, out hit)) {
				if (hit.collider.gameObject.CompareTag("Cube")) {
					hit.collider.gameObject.GetComponent<Cube>().Freeze(transform.position + direction * (hit.distance + Cube.WORLD_HALF_SIZE));
				}
			}
			float distance = hit.distance - Cube.WORLD_HALF_SIZE;
			ray.transform.position = transform.position + direction * (Cube.WORLD_HALF_SIZE + distance / 2);
			ray.transform.localScale = new Vector3(distance, ray.transform.localScale.y, ray.transform.localScale.z);
		}
	}
}
