using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorNode : MonoBehaviour {

	[Range(0, 1)] public float propagationChance = 1;
	public WeightedRoomNodeElement[] roomNodePrefabPool;
	
	public bool hasGenerated;

	void OnDrawGizmos() {
		Color oldColor = Gizmos.color;
		Matrix4x4 oldMat = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(0, 0.5f, 0), new Vector3(1, 1, 0.1f));
		Gizmos.DrawLine(new Vector3(0, 0.5f, 0), new Vector3(0, 0.5f, 0.5f));
		Gizmos.color = oldColor;
		Gizmos.matrix = oldMat;
	}

	public void Generate() {
		if (!hasGenerated) {
			hasGenerated = true;
			LevelGenerator generator = LevelGenerator.instance;
			System.Random rng = generator.rng;
			if (propagationChance > rng.NextDouble()) {
				Propagate();
			} 
		}
	}

	public void Propagate() {
		System.Random rng = LevelGenerator.instance.rng;
		RoomNode roomPrefab = rng.ChooseWeighted(roomNodePrefabPool);
		RoomNode roomNode = roomPrefab.CreateInstance();
		DoorNode doorNode = rng.Choose(roomNode.doorNodes);
		doorNode.hasGenerated = true;
		roomNode.transform.parent = transform;
		float angle = Vector3.Angle(-transform.forward, doorNode.transform.forward);
		if (angle != 0) {
			angle *= -Mathf.Sign(Vector3.Cross(-transform.forward, doorNode.transform.forward).y);
			if (angle == 0) {
				angle = 180;
			}
		}
		roomNode.transform.Rotate(0, angle, 0);
		roomNode.transform.position = transform.position + (roomNode.transform.position - doorNode.transform.position);
	}

}

[System.Serializable]
public class WeightedRoomNodeElement : WeightedElement<RoomNode> {}