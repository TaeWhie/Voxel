using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "MinecraftTutorial/Biome Attribute")]//Unity에서 bioattributes의 정보를 추가 할수 있게 된다.
public class BiomeAttributes : ScriptableObject
{

    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;

    public Lode[] lodes;

}

[System.Serializable]
public class Lode
{

    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;


}