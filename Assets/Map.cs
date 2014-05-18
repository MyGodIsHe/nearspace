using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {
	
	public GameObject playerPrefab;
	public GameObject cubePrefab;
	public GameObject jumpPrefab;
	public GameObject portalpPrefab;
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
		List<GameObject> portals = new List<GameObject>();
		Vector3 offset = level.Offset(startPoint);
		level.startPoint = startPoint;
		level.endPoint = V2toV3 (offset, level.end);

		foreach (LevelObject obj in level.cubes) {
			GameObject cube = Instantiate(
				cubePrefab,
				V2toV3(offset, obj.position),
				Quaternion.identity) as GameObject;
			if ((obj.cubeOptions & CubeOptions.Portal) == CubeOptions.Portal) {
				GameObject portal = Instantiate (portalpPrefab) as GameObject;
				portal.transform.parent = cube.transform;
				portal.GetComponent<Portal>().portals = portals;
				portal.transform.position = cube.transform.position + Vector3.up;
				portals.Add(portal);
			}
			if ((obj.cubeOptions & CubeOptions.EndPoint) == CubeOptions.EndPoint) {
				GameObject jump = Instantiate (jumpPrefab) as GameObject;
				jump.transform.parent = cube.transform;
				jump.transform.position = cube.transform.position + Vector3.up;
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
		Vector3 startPoint;
		if (levels.Count == 0)
			startPoint = -Vector3.up * layerDistance;
		else
			startPoint = levels[levels.Count - 1].endPoint;
		NextLevel (startPoint);
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

	public void JumpTo(string level) {
		Vector3 startPoint = player.transform.position + Vector3.up * layerDistance;
		int n = Convert.ToInt32 (level);
		try {
			LoadLevel (startPoint, n);
		} catch (System.NullReferenceException) {
			return;
		}
		Player playerComponent = player.GetComponent<Player>();
		playerComponent.Teleport(startPoint);
	}

	void GameOver() {
		levelNumber = 1;
		NextLevel (levels[levels.Count - 1].endPoint);
	}

	class Level {

		public List<LevelObject> cubes = new List<LevelObject>();
		public Vector2 start;
		public Vector2 end;

		public GameObject[] gameObjects;
		public Vector3 startPoint;
		public Vector3 endPoint;
		public GameObject[] portals;

		public Level(string name) {
			string[] lines = (Resources.Load(name, typeof(TextAsset)) as TextAsset).text.Split('\n');
			int y = 0;
			foreach (string line in lines) {
				int x = 0;
				foreach (char c in line) {
					CubeOptions cubeOptions = CubeOptions.None;
					switch (c) {
					case '#':
						cubeOptions = CubeOptions.Movable;
						break;
					case 'x':
						cubeOptions = CubeOptions.EndPoint;
						end = new Vector2(x, y);
						break;
					case 'o':
						cubeOptions = CubeOptions.Movable | CubeOptions.StartPoint;
						start = new Vector2(x, y);
						break;
					case 'p':
						cubeOptions = CubeOptions.Movable | CubeOptions.Portal;
						break;
					}
					if (cubeOptions != CubeOptions.None) {
						cubes.Add(new LevelObject(new Vector2(x, y), cubeOptions));
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
	
	struct LevelObject {
		public Vector2 position;
		public CubeOptions cubeOptions;

		public LevelObject(Vector2 position, CubeOptions cubeOptions) {
			this.position = position;
			this.cubeOptions = cubeOptions;
		}
		
		public LevelObject(Vector2 position) {
			this.position = position;
			this.cubeOptions = CubeOptions.Movable;
		}
	}

	[Flags]
	enum CubeOptions {
		None = 0x00,
		Movable = 0x01,
		StartPoint = 0x02,
		EndPoint = 0x04,
		Portal = 0x08
	}
}
