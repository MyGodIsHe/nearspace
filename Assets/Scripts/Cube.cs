using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {

	public AudioClip collisionSound;

	public const float WORLD_SIZE = 1;
	public const float WORLD_HALF_SIZE = WORLD_SIZE / 2;

	GameObject attached;
	const float FREEZE_TIME = 1;
	float startFreeze = 0;
	float lastHitFreeze = 0;
	Vector3 freezePosition;
	float freezeLength;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		freezeProcess();
	}

	protected static Vector3 round(Vector3 v) {
		return new Vector3 (Mathf.Round (v.x), Mathf.Round (v.y), v.z);
	}

	protected static void stop(GameObject obj) {
		if (obj.rigidbody2D.isKinematic)
			return;
		obj.rigidbody2D.velocity = Vector3.zero;
		obj.transform.position = round(obj.transform.position);
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		switch (other.gameObject.tag) {
		case "Player":
			if (attached == null) {
				attached = other.gameObject;
				transform.parent.gameObject.GetComponent<Room>().Lock();
			}
			break;
		case "Cube":
			audio.pitch = 1 + Random.Range(-0.1f, 0.1f);
			audio.PlayOneShot(collisionSound);
			stop(gameObject);
			stop(other.gameObject);
			break;
		case "Wall":
			audio.pitch = 1 + Random.Range(-0.1f, 0.1f);
			audio.PlayOneShot(collisionSound);
			stop(gameObject);
			break;
		}
	}

	void OnDestroy() {
		if (attached != null)
			attached.GetComponent<Player>().AttachedDestroy();
	}
	
	void freezeProcess() {
		if (!rigidbody2D.isKinematic)
			return;
		if (transform.position != freezePosition) {
			var distCovered = (Time.time - startFreeze) * 0.5f;
			var frac = distCovered / freezeLength;
			transform.position = Vector3.Lerp(transform.position, freezePosition, frac);
		}
		if (Time.time - lastHitFreeze > FREEZE_TIME) {
			rigidbody2D.isKinematic = false;
			startFreeze = 0;
		}
	}

	public void Freeze(Vector3 position) {
		lastHitFreeze = Time.time;
		if (startFreeze == 0) {
			startFreeze = Time.time;
			rigidbody2D.isKinematic = true;
			freezePosition = position;
			freezeLength = Vector3.Distance(transform.position, position);
		}
	}
	
	public void UnAttach() {
		attached = null;
	}
}
