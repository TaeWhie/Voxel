using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    //Perlin Noise 관련해서는 Basic에 있다.
    //https://github.com/TaeWhie/Basic/issues/6
    public static float Get2DPerlin(Vector2 position, float offset, float scale)
    {

        return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.ChunkWidth * scale + offset, (position.y + 0.1f) / VoxelData.ChunkWidth * scale + offset);

    }

    public static bool Get3DPerlin(Vector3 position, float offset, float scale, float threshold)
    {
        //2D를 3D로 만들기 위해서는 각 면마다 노이즈 만들어서 평균을 구하면 된다.
        float x = (position.x + offset + 0.1f) * scale;
        float y = (position.y + offset + 0.1f) * scale;
        float z = (position.z + offset + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        if ((AB + BC + AC + BA + CB + CA) / 6f > threshold)
            return true;
        else
            return false;

    }

}