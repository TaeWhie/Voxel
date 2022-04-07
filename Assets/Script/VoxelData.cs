using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData//복셀에 들어갈 하나의 큐브를 설정하는 작업
{
	//덩어리의 크기를 지정해 준다.
	public static readonly int ChunkWidth = 16;
	public static readonly int ChunkHeight = 128;
	public static readonly int WorldSizeInChunks = 10;//월드의 최대 사이즈
	public static readonly int ViewDistanceInChunks = 10;//비춰줄 청크의 개수
	public static int WorldSizeInVoxels
	{

		get { return WorldSizeInChunks * ChunkWidth; }

	}
	public static readonly int TextureAtlasSizeInBlocks = 4;//가지고 있는 텍스쳐의 사이즈는 4x4짜리이기 때문에 값은 4로 지정한다.
	public static float NormalizedBlockTextureSize//한 블럭의 사이즈를 알려준다.
	{

		get { return 1f / (float)TextureAtlasSizeInBlocks; }//여기서는 텍스쳐를 1/4로 나누면 하나의 블럭 사이즈가 나온다.

	}

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

	public static readonly int[,] voxelTris = new int[6, 4] {//각각의 면을 그리는 순서
		//최적화 작업을 위하여 진행한 작업
		/*LT ㅡㅡㅡ RT		
		 * l	   l		첫번째 삼각형은 [LB - LT - RB], 두 번째 삼각형은 [RB - LT - RT]
		 * l	   l		하지만 실제 사각형은 정점 6개가 아니라 4개 만으로 정의 할 수 있기 때문에
		 * l	   l		배열도 4개로 바꾸어준다.
		 *LB ㅡㅡㅡ RB
		 */
		/*{0, 3, 1, 1, 3, 2},->*/{0, 3, 1, 2}, // Back Face
		/*{5, 6, 4, 4, 6, 7},->*/{5, 6, 4, 7}, // Front Face
		/*{3, 7, 2, 2, 7, 6},->*/{3, 7, 2, 6}, // Top Face
		/*{1, 5, 0, 0, 5, 4},->*/{1, 5, 0, 4}, // Bottom Face
		/*{4, 7, 0, 0, 7, 3},->*/{4, 7, 0, 3}, // Left Face
		/*{1, 2, 5, 5, 2, 6} ->*/{1, 2, 5, 6} // Right Face
		
	};

	public static readonly Vector2[] voxelUvs = new Vector2[4] {//texture의 uv에 따라 큐브 각각의 면에 값 할당

		//UV도 마찬가지로 6개의 점이 아닌 4개에 점에 할당해도 됨으로 크기를 줄인다.
		/*new Vector2 (0.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 1.0f)*/


		new Vector2 (0.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (1.0f, 1.0f)
	};

	public static readonly Vector3[] faceChecks = new Vector3[6] {//voxelTris를 참조하여,각 면마다 생기는 수직 벡터의 값이다.

		new Vector3(0.0f, 0.0f, -1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, -1.0f, 0.0f),
		new Vector3(-1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f)

	};

}