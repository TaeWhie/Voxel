using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour//월드를 형성 하기 위한 정보를 위한 클래스이다.
{

    public Material material;
    public BlockType[] blocktypes;

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