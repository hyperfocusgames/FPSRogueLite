using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomNode : MonoBehaviour {

	public RoomNode prefab { get; private set; }
	public bool isPrefab { get { return prefab == null; } }

	public List<DoorNode> doorNodes { get; private set; }

	void Awake() {
		doorNodes = new List<DoorNode>();
		foreach (DoorNode node in GetComponentsInChildren<DoorNode>()) {
			doorNodes.Add(node);
		}
	}

	public RoomNode CreateInstance() {
		RoomNode room = Instantiate(this);
		room.name = name;
		room.prefab = this;
		return room;
	}

	public void Generate() {
		foreach (DoorNode doorNode in doorNodes) {
			doorNode.Generate();
		}
	}

}
