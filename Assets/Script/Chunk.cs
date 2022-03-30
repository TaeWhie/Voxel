using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	int vertexIndex = 0;
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();

	bool[,,] voxelMap = new bool[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];//복셀 맵 배열의 크기

	void Start()
	{
		PopulateVoxelMap();//voxelmap을 true 설정
		CreateMeshData();//false면 그리고 true면 그리지 않는다.
		CreateMesh();//렌더링한다.
	}
	void PopulateVoxelMap()//Voxelmap의 기본 값을 true로 설정 하는 과정, 이 과정을 거치지 않으면 값은 false이다.
	{

		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{
					//if(x >= y && z >= y) //이런 조건을 추가하면 계단모양의 Chunk를 만들수 있다.
					voxelMap[x, y, z] = true;

				}
			}
		}

	}

	void CreateMeshData()//Chunkmap을 그리기 위해서 Voxel하나 하나 값을 집어넣는 과정
	{

		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{

					AddVoxelDataToChunk(new Vector3(x, y, z));

				}
			}
		}

	}

	bool CheckVoxel(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);
		// 맵 범위를 벗어나는 경우
		if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
			return false;

		return voxelMap[x, y, z];

	}

	void AddVoxelDataToChunk(Vector3 pos)
	{
		for (int p = 0; p < 6; p++)//VoxelData에 설정해 놓은 값을 통해 값을 집어넣음
		{	 //각 면을 그리는 과정이다. 정육면체임으로 6번을 순환한다.

			 // Face Check(면이 바라보는 방향으로 +1 이동하여 확인)를 했을 때 
			 // Solid가 아닌 경우에만 큐브의 면이 그려지도록 하기
			 // => 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록
			if (CheckVoxel(pos) && !CheckVoxel(pos + VoxelData.faceChecks[p]))
			{
				// 각 면(삼각형 2개) 그리기

				// 1. Vertex, UV 4개 추가
				for (int i = 0; i <= 3; i++)
				{
					vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, i]] + pos);
					uvs.Add(VoxelData.voxelUvs[i]);
				}

				// 2. Triangle의 버텍스 인덱스 6개 추가
				triangles.Add(vertexIndex);
				triangles.Add(vertexIndex + 1);
				triangles.Add(vertexIndex + 2);

				triangles.Add(vertexIndex + 2);
				triangles.Add(vertexIndex + 1);
				triangles.Add(vertexIndex + 3);

				vertexIndex += 4;
			}
		}
	}

	void CreateMesh()
	{

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;

	}
}