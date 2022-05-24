using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 独立扫描算法（Saito and Toriwaki, 1994）
/// </summary>
public class IndependentScanning
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
        int[] temp_width = new int[width];
        // 1. 对每一行，计算目标点到非目标点的距离，即得到每个点(i,j)的水平距离
        for (int j = 0; j < height; j++)
        {
            // 是否有非目标点
            bool hasTarget = false;
            for (int i = 0; i < width; i++)
            {
                if (maps[i, j] == true)
                {
                    if (i == 0)
                    {
                        temp_width[i] = int.MaxValue;
                    }
                    else
                    {
                        if (temp_width[i - 1] == int.MaxValue)
                        {
                            temp_width[i] = int.MaxValue;
                        }
                        else
                        {
                            temp_width[i] = temp_width[i - 1] + 1;
                        }
                    }
                }
                else
                {
                    hasTarget = true;
                    temp_width[i] = 0;
                }
            }
            for (int i = width - 1; i >= 0; i--)
            {
                if (hasTarget == false)
                {
                    temp_width[i] = width;
                }
                else
                {
                    if (maps[i, j] == true)
                    {
                        if (i < width - 1)
                        {
                            temp_width[i] = Math.Min(temp_width[i + 1] + 1, temp_width[i]);
                        }
                    }
                    else
                    {
                        temp_width[i] = 0;
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                distance[i, j] = temp_width[i] * temp_width[i];
            }
        }
        // 2. 对每一列，计算每个点(i,j)与每一行(i,y)的竖直距离，即得到当前点(i, j)的竖直距离数组
        // 3. 将当前点(i, j)的竖直距离数组和当前列的对应点(i, y)的水平距离相加，其中的最小值即为当前点(i,j)的距离
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int min = int.MaxValue;
                for (int y = 0; y < height; y++)
                {
                    var dis = distance[i, y] + (j - y) * (j - y);
                    min = Math.Min(dis, min);
                }
                temp_width[i] = min;
            }

            for (int i = 0; i < width; i++)
            {
                distance[i, j] = temp_width[i];
            }
        }
        return distance;
    }
}
