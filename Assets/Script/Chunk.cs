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

	byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];//복셀 맵 배열의 크기

	private World world;
	void Start()
	{
		world = GameObject.Find("World").GetComponent<World>();//씬에 존재하는 월드 정보를 불러온다.

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
					//voxelMap[x, y, z] = true;

					//높이에 따라 다른 다른 블록을 쓴다. 숫자는 World의 Blocktype의 배열 변호이다.
					if (y < 1)
						voxelMap[x, y, z] = 0;
					else if (y == VoxelData.ChunkHeight - 1)
						voxelMap[x, y, z] = 1;
					else
						voxelMap[x, y, z] = 2;
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
		return /*voxelMap[x, y, z]; ->*/world.blocktypes[voxelMap[x, y, z]].isSolid;

	}

	void AddVoxelDataToChunk(Vector3 pos)
	{
		for (int p = 0; p < 6; p++)//VoxelData에 설정해 놓은 값을 통해 값을 집어넣음
		{    //각 면을 그리는 과정이다. 정육면체임으로 6번을 순환한다.

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
					//uvs.Add(VoxelData.voxelUvs[i]);
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
		/*(0.1)ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ(1.0)
			ㅣ  0 ㅣ  1 ㅣ	 2 ㅣ  3 ㅣ
		    ㅏㅡㅡ  ㅡㅡ  ㅡㅡ  ㅡㅡ ㅓ		텍스쳐는 다음과 같이 TextureID를 갖는다.
			ㅣ  4 ㅣ  5 ㅣ	 6 ㅣ  7 ㅣ		그리고 그것에 맞추어 UV를 지정해 주어야하는데
			ㅏㅡㅡ  ㅡㅡ  ㅡㅡ  ㅡㅡ ㅓ		예시)
			ㅣ  8 ㅣ  9 ㅣ	10 ㅣ 11 ㅣ		TextureID=0		(0,1)(0,0.75)(0.25,0)(0.25,0.75)
			ㅏㅡㅡ  ㅡㅡ  ㅡㅡ  ㅡㅡ ㅓ		순서는 폴리곤을 그리는 순서이며 버텍스가 들어가는 순서이다.
			ㅣ 12 ㅣ 13 ㅣ	14 ㅣ 15 ㅣ		
		 (0.0)ㅣㅡㅡ  ㅡㅡ  ㅡㅡ  ㅡㅡㅣ(1.1)
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