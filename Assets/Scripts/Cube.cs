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
		return new Vector3 (Mathf.Round (v.x),
		                    Mathf.Round (v.y),
		                    Mathf.Round (v.z));
	}

	protected static void stop(GameObject obj) {
		if (obj.rigidbody.isKinematic)
			return;
		obj.rigidbody.velocity = Vector3.zero;
		obj.transform.position = round(obj.transform.position);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player" && attached == null) {
			attached = other.gameObject;
			transform.parent.gameObject.GetComponent<Room>().Lock();
		}
		if (other.gameObject.tag == "Cube") {
			audio.pitch = 1 + Random.Range(-0.1f, 0.1f);
			audio.PlayOneShot(collisionSound);
			stop(gameObject);
			stop(other.gameObject);
		} else if (other.gameObject.tag == "Wall") {
			audio.pitch = 1 + Random.Range(-0.1f, 0.1f);
			audio.PlayOneShot(collisionSound);
			stop(gameObject);
		}
	}

	void OnDestroy() {
		if (attached != null)
			attached.GetComponent<Player>().AttachedDestroy();
	}
	
	void freezeProcess() {
		if (!rigidbody.isKinematic)
			return;
		if (transform.position != freezePosition) {
			var distCovered = (Time.time - startFreeze) * 0.5f;
			var frac = distCovered / freezeLength;
			transform.position = Vector3.Lerp(transform.position, freezePosition, frac);
		}
		if (Time.time - lastHitFreeze > FREEZE_TIME) {
			rigidbody.isKinematic = false;
			startFreeze = 0;
		}
	}

	public void Freeze(Vector3 position) {
		lastHitFreeze = Time.time;
		if (startFreeze == 0) {
			startFreeze = Time.time;
			rigidbody.isKinematic = true;
			freezePosition = position;
			freezeLength = Vector3.Distance(transform.position, position);
		}
	}
	
	public void UnAttach() {
		attached = null;
	}
}
