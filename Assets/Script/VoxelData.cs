using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData//복셀에 들어갈 하나의 큐브를 설정하는 작업
{


	public static readonly Vector3[] voxelVerts = new Vector3[8] {//정육면체의 각각 정점의 위치

		new Vector3(0.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 1.0f),

	};

	public static readonly int[,] voxelTris = new int[6, 6] {//각각의 면을 그리는 순서

		{0, 3, 1, 1, 3, 2}, // Back Face
		{5, 6, 4, 4, 6, 7}, // Front Face
		{3, 7, 2, 2, 7, 6}, // Top Face
		{1, 5, 0, 0, 5, 4}, // Bottom Face
		{4, 7, 0, 0, 7, 3}, // Left Face
		{1, 2, 5, 5, 2, 6} // Right Face

	};

	public static readonly Vector2[] voxelUvs = new Vector2[6] {//texture의 uv에 따라 큐브 각각의 면에 값 할당

		new Vector2 (0.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 1.0f)

	};


}