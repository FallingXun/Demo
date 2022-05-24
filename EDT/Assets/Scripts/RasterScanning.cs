using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 光栅扫描算法（8SSEDT，即 8-point Signed Sequential Euclidean Distance Transform）
/// </summary>
public class RasterScanning
{
    public static int m_Width;
    public static int m_Height;
    public static Grid m_GridMax;
    public static Grid m_GridMin;

    public static float[,] CreateSDF(bool[,] color)
    {
        m_Width = color.GetLength(0);
        m_Height = color.GetLength(1);
        m_GridMax = new Grid(m_Width, m_Height);
        m_GridMin = new Grid(0, 0);

        var positiveMaps = new bool[m_Width, m_Height];
        var negativeMaps = new bool[m_Width, m_Height];
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
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

        // 计算白色节点到黑色节点
        var g1 = Compute(positiveMaps);

        // 计算黑色节点到白色节点
        var g2 = Compute(negativeMaps);

        float[,] distance = new float[m_Width, m_Height];
        float max = float.MinValue;
        float min = float.MaxValue;
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                float d = GetDistance(g1[i, j], g2[i, j]);
                distance[i, j] = d;
                max = Math.Max(d, max);
                min = Math.Min(d, min);
            }
        }
        // 计算最大值到最小值的差值
        float clamp = max - min;
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
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

    public static float GetDistance(Grid g1, Grid g2)
    {
        double d1 = Math.Sqrt(g1.GetDistance());
        double d2 = Math.Sqrt(g2.GetDistance());
        // 白色到黑色为正距离，黑色到白色为负距离
        float d = (float)(d1 - d2);
        return d;
    }

    public static Grid[,] Init(bool[,] maps)
    {
        m_Width = maps.GetLength(0);
        m_Height = maps.GetLength(1);
        m_GridMax = new Grid(m_Width, m_Height);
        m_GridMin = new Grid(0, 0);

        var gridMaps = new Grid[m_Width, m_Height];
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                if (maps[i, j] == true)
                {
                    // 目标点为无穷远值
                    gridMaps[i, j] = m_GridMax;
                }
                else
                {
                    // 非目标点为0
                    gridMaps[i, j] = m_GridMin;
                }
            }
        }
        return gridMaps;
    }

    public static Grid[,] Compute(bool[,] maps)
    {
        var gridMaps = Init(maps);

        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                // 1.从上往下，从左往右，计算每个节点到左、上、左上、右上节点的距离，并将当前节点更新为距离最小的节点

                gridMaps[i, j] = Compare(gridMaps, i, j, -1, 0);    //邻居-左
                gridMaps[i, j] = Compare(gridMaps, i, j, 0, -1);    //邻居-上
                gridMaps[i, j] = Compare(gridMaps, i, j, -1, -1);   //邻居-左上
                gridMaps[i, j] = Compare(gridMaps, i, j, 1, -1);    //邻居-右上
            }

            for (int i = m_Width - 1; i >= 0; i--)
            {
                // 2.从上往下，从右往左，计算每个节点到右节点的距离，并将当前节点更新为距离最小的节点

                gridMaps[i, j] = Compare(gridMaps, i, j, 1, 0);     //邻居-右
            }
        }

        for (int j = m_Height - 1; j >= 0; j--)
        {
            for (int i = m_Width - 1; i >= 0; i--)
            {
                // 3.从下往上，从右往左，计算每个节点到右、下、左下、右下节点的距离，并将当前节点更新为距离最小的节点

                gridMaps[i, j] = Compare(gridMaps, i, j, 1, 0);    //邻居-右
                gridMaps[i, j] = Compare(gridMaps, i, j, 0, 1);    //邻居-下
                gridMaps[i, j] = Compare(gridMaps, i, j, -1, 1);    //邻居-左下
                gridMaps[i, j] = Compare(gridMaps, i, j, 1, 1);    //邻居-右下

            }

            for (int i = 0; i < m_Width; i++)
            {
                // 2.从下往上，从左往右，计算每个节点到左节点的距离，并将当前节点更新为距离最小的节点
                gridMaps[i, j] = Compare(gridMaps, i, j, -1, 0);    //邻居-左
            }
        }
        return gridMaps;
    }

    public static Grid Compare(Grid[,] gridMaps, int i, int j, int offset_i, int offset_j)
    {
        Grid result;
        Grid grid = GetGrid(gridMaps, i, j);

        int compare_i = i + offset_i;
        int compare_j = j + offset_j;
        if (compare_i < 0 || compare_i >= m_Width || compare_j < 0 || compare_j >= m_Height)
        {
            // 超出边界的节点不进行比较
            result = grid;
        }
        else
        {
            Grid grid_compare = GetGrid(gridMaps, i + offset_i, j + offset_j);
            // 比较的目标节点，加上偏移信息，即从源节点移动到目标节点的信息
            grid_compare.Offset(offset_i, offset_j);

            int d = grid.GetDistance();
            int d_compare = grid_compare.GetDistance();

            // 如果目标节点的距离小，则返回目标节点
            if (d_compare < d)
            {
                result = grid_compare;
            }
            else
            {
                result = grid;
            }
        }
        return result;
    }


    public static Grid GetGrid(Grid[,] gridMaps, int i, int j)
    {
        if (i < 0 || i >= m_Width || j < 0 || j >= m_Height)
        {
            // 超出边界的设置为无穷远（可以不用遍历超出边界的点）
            return m_GridMax;
        }
        return gridMaps[i, j];
    }


    public struct Grid
    {
        public int dx;
        public int dy;

        public Grid(int dx, int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }

        public void Offset(int x, int y)
        {
            dx += x;
            dy += y;
        }

        public int GetDistance()
        {
            return dx * dx + dy * dy;
        }
    }
}


