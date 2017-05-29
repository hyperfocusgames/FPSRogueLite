using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class RuntimeNavMeshBuilder : MonoBehaviour
{
	NavMeshData nmd;
	NavMeshDataInstance navMeshInstance;

	public void OnEnable()
	{
		generateNavMesh();
	}

	void OnDisable()
	{
		clear();
	}

	public void regen()
	{
		OnDisable();
		OnEnable();
	}

	protected void clear()
	{
		navMeshInstance.Remove();
	}

	protected void init()
	{
		clear();
		nmd = new NavMeshData();
		navMeshInstance = NavMesh.AddNavMeshData(nmd);
	}

	public void generateNavMesh()
	{
		init();

		List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
		foreach(MeshFilter mf in GetComponentsInChildren<MeshFilter>())
		{
			NavMeshBuildSource s = new NavMeshBuildSource();
			s.transform= mf.transform.localToWorldMatrix;
			s.shape = NavMeshBuildSourceShape.Mesh;
			s.sourceObject = mf.mesh;
			s.area = 0;
			
			sources.Add(s);
		}
		NavMeshBuilder.UpdateNavMeshData(nmd, NavMesh.GetSettingsByIndex(0), sources, gameObject.GetComponent<BoxCollider>().bounds);
	}
}

[CustomEditor(typeof(RuntimeNavMeshBuilder))]
public class RuntimeNavMeshBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if(GUILayout.Button("Generate"))
            ((RuntimeNavMeshBuilder)target).generateNavMesh();
	}
}