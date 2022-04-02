using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour//월드를 형성 하기 위한 정보를 위한 클래스이다.
{
    public Transform player;//플레이어 위치
    public Vector3 spawn;//스폰 위치

    public Material material;
    public BlockType[] blocktypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerLastChunkCoord;

    private void Start()
    {
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);
    }
    private void Update()
    {

        if (!GetChunkCoordFromVector3(player.transform.position).Equals(playerLastChunkCoord))
            CheckViewDistance();

    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);

    }

    private void GenerateWorld()
    {

        for (int x = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks / 2; x < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks / 2; x++)
        {
            for (int z = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks / 2; z < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks / 2; z++)
            {

                CreateChunk(new ChunkCoord(x, z));

            }
        }

        spawn = new Vector3(VoxelData.WorldSizeInBlocks / 2, VoxelData.ChunkHeight + 2, VoxelData.WorldSizeInBlocks / 2);
        player.position = spawn;

    }

    private void CheckViewDistance()
    {

        int chunkX = Mathf.FloorToInt(player.position.x / VoxelData.ChunkWidth);
        int chunkZ = Mathf.FloorToInt(player.position.z / VoxelData.ChunkWidth);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = chunkX - VoxelData.ViewDistanceInChunks / 2; x < chunkX + VoxelData.ViewDistanceInChunks / 2; x++)
        {
            for (int z = chunkZ - VoxelData.ViewDistanceInChunks / 2; z < chunkZ + VoxelData.ViewDistanceInChunks / 2; z++)
            {

                // 청크를 붙혀서 쓰다 보면 지정한 월드 크기보다 커질 수 있는데, 만약 지정한 월드보다 커졌다면 그리지 않는다.
                if (IsChunkInWorld(x, z))
                {

                    ChunkCoord thisChunk = new ChunkCoord(x, z);

                    if (chunks[x, z] == null)//만약 생성되어 있지않다면 생성한다.
                        CreateChunk(thisChunk);
                    else if (!chunks[x, z].isActive)//엑티브만 꺼져있다면 켜주고, 엑티브되었으니 리스트에 추가해준다.
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(thisChunk);
                    }
                    //이미 액티브가 되어있던 청크들중에, 시야 범위에 들어오는 청크들은 제외 시켜준다.
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    {

                        //if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        if (previouslyActiveChunks[i].x == x && previouslyActiveChunks[i].z == z)
                            previouslyActiveChunks.RemoveAt(i);

                    }

                }
            }
        }
        //최종적으로 previouslyActiveChunks에는 시야범위에서 벗어난 블럭들만 모이게 됨으로 액티브를 다 끈다.
        foreach (ChunkCoord coord in previouslyActiveChunks)
            chunks[coord.x, coord.z].isActive = false;

    }

    bool IsChunkInWorld(int x, int z)
    {

        if (x > 0 && x < VoxelData.WorldSizeInChunks - 1 && z > 0 && z < VoxelData.WorldSizeInChunks - 1)
            return true;
        else
            return false;

    }

    private void CreateChunk(ChunkCoord coord)
    {
        //x와z만 고려하기 때문에 y인 높이는 그려지지 않는다.
        chunks[coord.x, coord.z] = new Chunk(new ChunkCoord(coord.x, coord.z), this);
        activeChunks.Add(new ChunkCoord(coord.x, coord.z));


    }

    public byte GetVoxel(Vector3 pos)
    {

        if (pos.x < 0 || pos.x > VoxelData.WorldSizeInBlocks - 1 || pos.y < 0 || pos.y > VoxelData.ChunkHeight - 1 || pos.z < 0 || pos.z > VoxelData.WorldSizeInBlocks - 1)
            return 0;
        if (pos.y < 1)
            return 1;
        else if (pos.y == VoxelData.ChunkHeight - 1)
            return 3;
        else
            return 2;

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

[System.Serializable]
public class BlockType
{

    public string blockName;
    public bool isSolid;//solid를 false로 두면 내면까지 모두 그릴 수 있다.

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Back, Front, Top, Bottom, Left, Right

    public int GetTextureID(int faceIndex)//인덱스에 기입된 값을 통해 복셀 각각의 면에 다르게 텍스쳐를 넣을 수 있다.
    {

        switch (faceIndex)
        {

            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;


        }

    }

}