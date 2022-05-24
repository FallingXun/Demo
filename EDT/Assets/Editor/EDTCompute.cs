using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class EDTCompute : Editor
{
    [MenuItem("EDT/BruteForce")]
    public static void Compute_BruteForce()
    {
        var objs = Selection.objects;
        if(objs == null)
        {
            return;
        }
        foreach (var obj in objs)
        {
            var positive = GetPositiveColor(obj);
            var negative = GetNegativeColor(obj);
            if (positive == null || negative == null)
            {
                return;
            }
            int width = positive.GetLength(0);
            int height = positive.GetLength(1);
            double time1 = 0;
            double time2 = 0;
            double time3 = 0;

            time1 = GetTime();
            BruteForce.Compute(positive);
            time2 = GetTime();
            BruteForce.Compute(negative);
            time3 = GetTime();
            Debug.Log(string.Format("BruteForce : width = {0}, height = {1}, time = {2}", width, height, time2 - time1));
            Debug.Log(string.Format("BruteForce : width = {0}, height = {1}, time = {2}", width, height, time3 - time2));
        }
    }

    [MenuItem("EDT/RasterScanning")]
    public static void Compute_RasterScanning()
    {
        var objs = Selection.objects;
        if (objs == null)
        {
            return;
        }
        foreach (var obj in objs)
        {
            var positive = GetPositiveColor(obj);
            var negative = GetNegativeColor(obj);
            if (positive == null || negative == null)
            {
                return;
            }
            int width = positive.GetLength(0);
            int height = positive.GetLength(1);
            double time1 = 0;
            double time2 = 0;
            double time3 = 0;

            time1 = GetTime();
            RasterScanning.Compute(positive);
            time2 = GetTime();
            RasterScanning.Compute(negative);
            time3 = GetTime();
            Debug.Log(string.Format("RasterScanning : width = {0}, height = {1}, time = {2}", width, height, time2 - time1));
            Debug.Log(string.Format("RasterScanning : width = {0}, height = {1}, time = {2}", width, height, time3 - time2));
        }
    }

    [MenuItem("EDT/IndependentScanning")]
    public static void Compute_IndependentScanning()
    {
        var objs = Selection.objects;
        if (objs == null)
        {
            return;
        }
        foreach (var obj in objs)
        {
            var positive = GetPositiveColor(obj);
            var negative = GetNegativeColor(obj);
            if (positive == null || negative == null)
            {
                return;
            }
            int width = positive.GetLength(0);
            int height = positive.GetLength(1);
            double time1 = 0;
            double time2 = 0;
            double time3 = 0;

            time1 = GetTime();
            IndependentScanning.Compute(positive);
            time2 = GetTime();
            IndependentScanning.Compute(negative);
            time3 = GetTime();
            Debug.Log(string.Format("IndependentScanning : width = {0}, height = {1}, time = {2}", width, height, time2 - time1));
            Debug.Log(string.Format("IndependentScanning : width = {0}, height = {1}, time = {2}", width, height, time3 - time2));
        }
    }

    public static double GetTime()
    {
        return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
    }

 

    public static bool[,] GetPositiveColor(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return null;
        }
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
        if (tex == null)
        {
            return null;
        }
        Color target = Color.white;
        int width = tex.width;
        int height = tex.height;
        bool[,] color = new bool[width, height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var col = tex.GetPixel(i, j);
                if (col == target)
                {
                    color[i, j] = true;
                }
                else
                {
                    color[i, j] = false;
                }
            }
        }

        return color;
    }

    public static bool[,] GetNegativeColor(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return null;
        }
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
        if (tex == null)
        {
            return null;
        }
        Color target = Color.white;
        int width = tex.width;
        int height = tex.height;
        bool[,] color = new bool[width, height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var col = tex.GetPixel(i, j);
                if (col == target)
                {
                    color[i, j] = false;
                }
                else
                {
                    color[i, j] = true;
                }
            }
        }

        return color;
    }
}


