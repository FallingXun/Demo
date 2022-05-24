using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 暴力算法
/// </summary>
public class BruteForce
{
    public static float[,] CreateSDF(bool[,] color)
    {
        int width = color.GetLength(0);
        int height = color.GetLength(1);

        var positiveMaps = new bool[width, height];
        var negativeMaps = new bool[width, height];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                // 白色节点为true，黑色节点为false
                if (color[i, j] == true)
                {
                    positiveMaps[i, j] = true;  // 正向map，计算白色节点（目标点）到黑色节点（非目标点）的距离
                    negativeMaps[i, j] = false; // 反向map，计算黑色节点（目标点）到白色节点（非目标点）的距离
                }
                else
                {
                    positiveMaps[i, j] = false;
                    negativeMaps[i, j] = true;
                }
            }
        }

        var d1 = Compute(positiveMaps);
        var d2 = Compute(negativeMaps);

        float[,] distance = new float[width, height];
        float max = float.MinValue;
        float min = float.MaxValue;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                float d = (float)(Math.Sqrt(d1[i, j]) - Math.Sqrt(d2[i, j]));
                distance[i, j] = d;
                max = Math.Max(d, max);
                min = Math.Min(d, min);
            }
        }
        // 计算最大值到最小值的差值
        float clamp = max - min;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (clamp <= 0)
                {
                    distance[i, j] = 0;
                }
                else
                {
                    // 将距离映射为[0,1]
                    distance[i, j] = (distance[i, j] - min) / clamp;
                }
            }
        }
        return distance;
    }

    public static int[,] Compute(bool[,] maps)
    {
        int width = maps.GetLength(0);
        int height = maps.GetLength(1);

        int[,] distance = new int[width, height];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                // 1. 遍历每一个白色节点
                if (maps[i, j] == true)
                {
                    int min = int.MaxValue;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (maps[x, y] == true)
                            {
                                continue;
                            }
                            // 2. 遍历每一个黑色节点，计算白色节点到黑色节点的距离，记录最小值
                            int dis = (x - i) * (x - i) + (y - j) * (y - j);
                            if (dis < min)
                            {
                                min = dis;
                            }
                        }
                    }
                    distance[i, j] = min;
                }
            }
        }
        return distance;
    }
}
