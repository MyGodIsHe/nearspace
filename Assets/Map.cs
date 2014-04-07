using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {
	
	public GameObject playerPrefab;
	public GameObject cubePrefab;
	public GameObject jumpPrefab;
	public GameObject endLinePrefab;
	
	private GameObject endLine;
	private GameObject player;
	private List<Level> levels = new List<Level>();
	private int levelNumber = 1;

	readonly float layerDistance = 15;

	void Start () {
		player = Instantiate(
			playerPrefab,
			Vector3.zero,
			Quaternion.identity) as GameObject;
		NextLevel ();
		GetComponent<FollowTop> ().SetTarget(player);
	}

	static Vector3 V2toV3 (Vector3 offset, Vector2 pos) {
		return new Vector3 (pos.x - offset.x, offset.y, - (pos.y - offset.z));
	}
	
	void Deploy(Level level, Vector3 startPoint) {
		List<GameObject> objects = new List<GameObject>();
		Vector3 offset = level.Offset(startPoint);
		level.startPoint = startPoint;
		level.endPoint = V2toV3 (offset, level.end);

		foreach (Vector2 pos in level.cubes) {
			GameObject cube = Instantiate(
				cubePrefab,
				V2toV3(offset, pos),
				Quaternion.identity) as GameObject;
			if (pos == level.end) {
				GameObject jump = Instantiate (jumpPrefab) as GameObject;
				jump.transform.parent = cube.transform;
				jump.transform.position = cube.transform.position + Vector3.up;
				cube.renderer.material.color = Color.black;
				cube.AddComponent<EndCube>();
			}
			objects.Add(cube);
		}

		level.gameObjects = objects.ToArray();
	}

	void createEndLine(GameObject[] objects) {
		if (endLine)
			Destroy (endLine);

		endLine = Instantiate(endLinePrefab) as GameObject;
		Vector3 p = objects[0].transform.position;
		float minX = p.x, minZ = p.z, maxX = p.x, maxZ = p.z;
		foreach (GameObject obj in objects) {
			p = obj.transform.position;
			minX = Mathf.Min(minX, p.x);
			minZ = Mathf.Min(minZ, p.z);
			maxX = Mathf.Max(maxX, p.x);
			maxZ = Mathf.Max(maxZ, p.z);
		}
		endLine.transform.position = new Vector3(minX + (maxX - minX) / 2,
		                                         layerDistance * (levels.Count - 1),
		                                         minZ + (maxZ - minZ) / 2);
		endLine.transform.localScale = new Vector3 ((maxX - minX) / 2 + 50,
		                                            layerDistance * 3,
		                                            minZ + (maxZ - minZ) / 2 + 50);
	}

	Vector3 getStartPoint() {
		return levels.Count > 0 ? levels [levels.Count - 1].endPoint + Vector3.up * layerDistance : Vector3.zero;
	}

	void LoadLevel(Vector3 startPoint, int number) {
		Level level = new Level (number.ToString ());
		Deploy (level, startPoint);
		levels.Add (level);
		createEndLine (level.gameObjects);
	}
	
	public void NextLevel () {
		NextLevel (-Vector3.up * layerDistance);
	}

	public void NextLevel (Vector3 startPoint) {
		startPoint += Vector3.up * layerDistance;
		try {
			LoadLevel (startPoint, levelNumber++);
		} catch (System.NullReferenceException) {
			GameOver();
		}
	}

	public void RestartLevel () {
		Level level = levels [levels.Count - 1];
		foreach (GameObject obj in level.gameObjects)
			Destroy (obj);
		Deploy (level, level.startPoint);
		player.transform.position = level.startPoint;
	}

	void GameOver() {
		levelNumber = 1;
		NextLevel (levels[levels.Count - 1].endPoint);
	}

	class Level {

		public List<Vector2> cubes = new List<Vector2>();
		public Vector2 start;
		public Vector2 end;

		public GameObject[] gameObjects;
		public Vector3 startPoint;
		public Vector3 endPoint;

		public Level(string name) {
			string[] lines = (Resources.Load(name, typeof(TextAsset)) as TextAsset).text.Split('\n');
			int y = 0;
			foreach (string line in lines) {
				int x = 0;
				foreach (char c in line) {
					switch (c) {
					case '#':
						cubes.Add(new Vector2(x, y));
						break;
					case 'x':
						cubes.Add(new Vector2(x, y));
						end = new Vector2(x, y);
						break;
					case 'o':
						cubes.Add(new Vector2(x, y));
						start = new Vector2(x, y);
						break;
					}
					x++;
				}
				y++;
			}
		}

		public Vector3 Offset(Vector3 startPoint) {
			Vector3 offset = startPoint;
			offset.x = start.x - offset.x;
			offset.z += start.y;
			return offset;
		}
	}
}
