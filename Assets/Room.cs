using System;
using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour {
	
	public GameObject cubePrefab;
	public GameObject stopCubePrefab;
	public GameObject keyPrefab;
	public GameObject wallPrefab;
	public GameObject doorPrefab;

	const float height = 1;
	Level level;
	List<GameObject> doors = new List<GameObject>();
	Vector3 enterPlayerPosition = Vector3.zero;
	Vector3 enterPlayerVelosity = Vector3.zero;
	bool isDeactivated = false;

	Vector3 V2toV3 (Vector2 pos) {
		Vector3 offset = transform.position;
		offset.x -= level.size.x / 2 - Cube.WORLD_HALF_SIZE;
		offset.z += level.size.y / 2 - Cube.WORLD_HALF_SIZE;
		return new Vector3 (offset.x + pos.x, offset.y, offset.z - pos.y);
	}
	
	void Deploy(Vector3 startPoint) {
		foreach (LevelObject obj in level.cubes) {
			GameObject gameObj;
			switch (obj.type) {
			case ObjectType.Key:
				gameObj = Instantiate(keyPrefab) as GameObject;
				break;
			case ObjectType.StopCube:
				gameObj = Instantiate(stopCubePrefab) as GameObject;
				break;
			default:
				gameObj = Instantiate(cubePrefab) as GameObject;
				break;
			}
			gameObj.transform.position = V2toV3(obj.position);
			gameObj.transform.parent = this.transform;
		}
	}

	GameObject createBlock(Vector2 center, Vector3 scale, Vector2 textureScale) {
		GameObject wall = createBlock(scale, textureScale);
		wall.transform.position = V2toV3(center);
		return wall;
	}
	
	GameObject createBlock(Vector3 scale, Vector2 textureScale) {
		GameObject wall = Instantiate(wallPrefab) as GameObject;
		wall.transform.localScale = scale;
		wall.renderer.material.SetTextureScale ("_MainTex", textureScale);
		wall.renderer.material.SetTextureScale ("_Detail", textureScale);
		wall.transform.parent = this.transform;
		return wall;
	}

	GameObject createWall(int size, bool hasDoor) {
		GameObject block;
		float blocks = (size - 1) / 2;
		GameObject wall = new GameObject("Wall");
		wall.transform.parent = this.transform;

		Vector3 side = Vector3.forward * ((blocks + Cube.WORLD_SIZE) / 2);

		block = createBlock(
			new Vector3(1, height, blocks),
			new Vector2(1, blocks));
		block.transform.position = -side;
		block.transform.parent = wall.transform;
		block.name = "BlockLeft";
		
		block = createBlock(
			new Vector3(1, height, blocks),
			new Vector2(1, blocks));
		block.transform.position = side;
		block.transform.parent = wall.transform;
		block.name = "BlockRight";

		if (hasDoor) {
			block = Instantiate(doorPrefab) as GameObject;
			doors.Add(block);
		} else
			block = Instantiate(wallPrefab) as GameObject;
		block.transform.parent = wall.transform;

		return wall;
	}
	
	void createWalls() {
		GameObject wall;

		wall = createWall((int)level.size.y, (level.directions & Directions.Left) == Directions.Left);
		wall.transform.position = V2toV3(new Vector2(-1, level.size.y / 2 - Cube.WORLD_HALF_SIZE));
		
		wall = createWall((int)level.size.y, (level.directions & Directions.Right) == Directions.Right);
		wall.transform.position = V2toV3(new Vector2(level.size.x, level.size.y / 2 - Cube.WORLD_HALF_SIZE));
		
		wall = createWall((int)level.size.x, (level.directions & Directions.Up) == Directions.Up);
		wall.transform.position = V2toV3(new Vector2(level.size.x / 2 - Cube.WORLD_HALF_SIZE, -1));
		wall.transform.rotation = Quaternion.Euler (0, 90, 0);
		
		wall = createWall((int)level.size.x, (level.directions & Directions.Down) == Directions.Down);
		wall.transform.position = V2toV3(new Vector2(level.size.x / 2 - Cube.WORLD_HALF_SIZE, level.size.y));
		wall.transform.rotation = Quaternion.Euler (0, -90, 0);

		// Corner
		wall = createBlock(
			new Vector2(-1, -1),
			new Vector3(1, height, 1),
			Vector2.one);
		wall.name = "Corner";

		wall = createBlock(
			new Vector2(level.size.x, -1),
			new Vector3(1, height, 1),
			Vector2.one);
		wall.name = "Corner";

		wall = createBlock(
			new Vector2(-1, level.size.y),
			new Vector3(1, height, 1),
			Vector2.one);
		wall.name = "Corner";
		
		wall = createBlock(
			new Vector2(level.size.x, level.size.y),
			new Vector3(1, height, 1),
			Vector2.one);
		wall.name = "Corner";

		// Floor
		wall = createBlock(
			new Vector2(level.size.x / 2 - Cube.WORLD_HALF_SIZE, level.size.y / 2 - Cube.WORLD_HALF_SIZE),
			new Vector3(level.size.x + 2, 1, level.size.y + 2),
			new Vector2(level.size.x + 2, level.size.y + 2));
		wall.renderer.material.color = Color.Lerp (wall.renderer.material.color, Color.black, Cube.WORLD_HALF_SIZE);
		wall.transform.position += Vector3.down * (height / 2 + Cube.WORLD_HALF_SIZE);
		wall.name = "Floor";
	}

	public void LoadLevel(Level level) {
		this.level = level;
		Deploy (Vector3.zero);
		createWalls();
		BoxCollider collider = gameObject.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		collider.size = new Vector3(level.size.x + 1, height + 1, level.size.y + 1);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			enterPlayerPosition = other.gameObject.transform.position;
			enterPlayerVelosity = other.gameObject.rigidbody.velocity;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player") {
			Vector3 direction = other.gameObject.transform.position - transform.position;
			direction.Normalize();
			Camera.main.GetComponent<Map>().MoveTo(Mathf.RoundToInt(direction.x), -Mathf.RoundToInt(direction.z));
		} else {
			Destroy(other.gameObject);
		}
	}

	public void Lock() {
		if (isDeactivated)
			return;
		foreach(GameObject obj in doors) {
			obj.GetComponent<Door>().Close();
		}
	}

	public void Unlock() {
		isDeactivated = true;
		foreach(GameObject obj in doors) {
			obj.GetComponent<Door>().Open();
		}
	}

	public Vector3 EnterPlayerPosition {
		get {
			return enterPlayerPosition;
		}
	}
	
	public Vector3 EnterPlayerVelosity {
		get {
			return enterPlayerVelosity;
		}
	}
}
