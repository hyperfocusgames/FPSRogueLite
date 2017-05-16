using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : SingletonBehaviour<LevelGenerator> {

	public bool generateOnStart;
	public RoomNode startNodePrefab;

	public System.Random rng { get; private set; }
	public RoomNode startNode { get; private set; }

	void Start() {
		if (generateOnStart) {
			GenerateLevel();
		}
	}

	public void GenerateLevel() {
		rng = new System.Random();
		startNode = startNodePrefab.CreateInstance();
	}

	void Update() {
		if (Input.anyKeyDown) {
			GenerateRooms();
		}
	}

	public void GenerateRooms() {
		foreach (RoomNode roomNode in FindObjectsOfType<RoomNode>()) {
			roomNode.Generate();
		}
	}

}
