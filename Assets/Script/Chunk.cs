using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk//직접적으로 엔진을 돌리지 않기 때문에 MonoBehaviour삭제
{
	public ChunkCoord coord;//청크가 들어가야할 좌표

	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;
	GameObject chunkObject;

	int vertexIndex = 0;
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();

	byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];//복셀 맵 배열의 크기

	private World world;

	public Chunk(ChunkCoord _coord, World _world)//좌표와 월드를 받아 그리는 함수
	{
		coord = _coord;
		chunkObject = new GameObject();
		chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);

		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshFilter = chunkObject.AddComponent<MeshFilter>();
		world = _world;

		chunkObject.transform.SetParent(world.transform);
		meshRenderer.material = world.material;

		chunkObject.name = coord.x + ", " + coord.z;

		PopulateVoxelMap();
		CreateMeshData();
		CreateMesh();

	}

	void PopulateVoxelMap()//어떤 위치에 어떤 모양의 복셀이 들어가는지 설정하는 함수
	{

	for (int y = 0; y < VoxelData.ChunkHeight; y++) {
			for (int x = 0; x < VoxelData.ChunkWidth; x++) {
				for (int z = 0; z < VoxelData.ChunkWidth; z++) {

                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
    
				}
			}
		}

	}
	public bool isActive
	{

		get { return chunkObject.activeSelf; }
		set { chunkObject.SetActive(value); }

	}

	Vector3 position
	{

		get { return chunkObject.transform.position; }

	}

	bool IsVoxelInChunk(int x, int y, int z)
	{

		if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
			return false;
		else return true;

	}

	void CreateMeshData()//Chunkmap을 그리기 위해서 Voxel하나 하나 값을 집어넣는 과정
	{

		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{

					if (world.blocktypes[voxelMap[x, y, z]].isSolid)
						AddVoxelDataToChunk(new Vector3(x, y, z));

				}
			}
		}

	}
	public byte GetVoxelFromMap(Vector3 pos)
	{

		pos -= position;

		return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

	}
	bool CheckVoxel(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);
		// 맵 범위를 벗어나는 경우
		if (!IsVoxelInChunk(x, y, z))
			return world.blocktypes[world.GetVoxel(pos + position)].isSolid;

		return /*voxelMap[x, y, z]; ->*/world.blocktypes[voxelMap[x, y, z]].isSolid;

	}

	void AddVoxelDataToChunk(Vector3 pos)
	{
		for (int p = 0; p < 6; p++)//VoxelData에 설정해 놓은 값을 통해 값을 집어넣음
		{
			//각 면을 그리는 과정이다. 정육면체임으로 6번을 순환한다.

			// Face Check(면이 바라보는 방향으로 +1 이동하여 확인)를 했을 때 
			// Solid가 아닌 경우에만 큐브의 면이 그려지도록 하기
			// => 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록
			if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
			{
				// 각 면(삼각형 2개) 그리기


				byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
				// 1. Vertex, UV 4개 추가
				for (int i = 0; i <= 3; i++)
				{
					vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, i]] + pos);
				}
				AddTexture(world.blocktypes[blockID].GetTextureID(p));

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
	void AddTexture(int textureID)//각 면에 텍스쳐를 추가하는 함수
	{
		/*  (0.1)ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ(1.1)
				ㅣ  0 ㅣ  1 ㅣ 2 ㅣ  3 ㅣ
				ㅏㅡㅡ  ㅡㅡ  ㅡㅡ ㅡㅡㅓ		텍스쳐는 다음과 같이 TextureID를 갖는다.
				ㅣ  4 ㅣ  5 ㅣ 6 ㅣ  7 ㅣ		그리고 그것에 맞추어 UV를 지정해 주어야하는데
				ㅏㅡㅡ  ㅡㅡ  ㅡㅡ ㅡㅡㅓ		예시)
				ㅣ  8 ㅣ  9 ㅣ10 ㅣ 11 ㅣ		TextureID=0		(0,1)(0,0.75)(0.25,0)(0.25,0.75)
				ㅏㅡㅡ  ㅡㅡ  ㅡㅡ ㅡㅡㅓ		순서는 폴리곤을 그리는 순서이며 버텍스가 들어가는 순서이다.
				ㅣ 12 ㅣ 13 ㅣ14 ㅣ 15 ㅣ		
		   (0.0)ㅣㅡㅡ  ㅡㅡ  ㅡㅡ ㅡㅡㅣ(0.1)
		*/

		//여기서 x,y는 텍스쳐의 좌측 위의 점을 기준으로 한다.
		float y = textureID / VoxelData.TextureAtlasSizeInBlocks;//세로줄은 0-4-8-12 로 늘고 있기 때문에 4씩 곱해지면 된다.
		float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);//가로줄은 순서대로 더하면 되는데, 다음 줄 처리를 위해 y값을 나눠 몇번째 줄인지 알수 있다.

		x *= VoxelData.NormalizedBlockTextureSize;
		y *= VoxelData.NormalizedBlockTextureSize;

		y = 1f - y - VoxelData.NormalizedBlockTextureSize;//uv좌표는 뒤집어져있기 때문에 1에서 빼준다.

		//각각의 버텍스에 집어넣는다.
		uvs.Add(new Vector2(x, y));
		uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
		uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
		uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));


	}

}
public class ChunkCoord
{

	public int x;
	public int z;

	public ChunkCoord(int _x, int _z)
	{

		x = _x;
		z = _z;

	}

	public bool Equals(ChunkCoord other)
	{

		if (other == null)
			return false;
		else if (other.x == x && other.z == z)
			return true;
		else
			return false;

	}
}
