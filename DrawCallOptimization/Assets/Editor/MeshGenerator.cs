using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class MeshGenerator : Editor
{

    [MenuItem("GameObject/CreateStaticBatch")]
    public static void CreateStaticBatch()
    {
        var root = new GameObject("Root");

        var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/TestMaterial.mat");

        var meshList = CreateStaticBatchMesh();
        for (int i = 0; i < meshList.Count; i++)
        {
            var m = meshList[i];
            GameObject go = new GameObject(((i + 1) * 2).ToString());
            go.isStatic = true;
            go.transform.parent = root.transform;
            go.transform.localPosition = new Vector3(-(i + 1) / 2f, 0, i);
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;

            mf.sharedMesh = m;
        }
    }

    private static List<Mesh> CreateStaticBatchMesh()
    {
        var meshList = new List<Mesh>();
        var mesh = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh;
        var colors1 = new Color32[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            colors1[i] = new Color32(255, 255, 255, 255);
        }
        var tangents1 = new Vector4[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            tangents1[i] = new Vector4(0, 0, 0, 1);
        }
        for (int i = 1; i <= 10; i++)
        {
            var m = new Mesh();
            var vertics = new List<Vector3>(i * 4);
            var normals = new List<Vector3>(i * 4);
            var triangles = new List<int>(i * 4 * mesh.vertexCount);
            var uvs = new List<Vector2>(i * 4);
            var uvs4 = new List<Vector4>(i * 4);
            var tangents = new List<Vector4>(i * 4);
            var colors32 = new List<Color32>(i * 4);
            var colors = new List<Color>(i * 4);
            var boneWeight = new List<BoneWeight>(i * 4);
            for (int j = 0; j < i; j++)
            {
                normals.AddRange(mesh.normals);
                for (int k = 0; k < mesh.vertexCount; k++)
                {
                    vertics.Add(mesh.vertices[k] + new Vector3(j, 0, 0));
                    uvs.Add(mesh.uv[k]);
                    uvs4.Add(new Vector4(mesh.uv[k].x, mesh.uv[k].y, mesh.uv[k].x, mesh.uv[k].y));
                    colors32.Add(colors1[k]);
                    colors.Add(colors1[k]);
                    tangents.Add(tangents1[k]);
                }
                for (int k = 0; k < mesh.triangles.Length; k++)
                {
                    triangles.Add(mesh.triangles[k] + j * mesh.vertexCount);
                }
            }

            m.SetVertices(vertics);
            m.SetNormals(normals);
            m.SetTangents(tangents);
            //m.colors32 = colors32.ToArray();
            m.colors = colors.ToArray();
            m.SetTriangles(triangles, 0);
            m.SetUVs(0, uvs);
            m.SetUVs(1, uvs);
            m.SetUVs(2, uvs);
            m.SetUVs(3, uvs);
            m.SetUVs(4, uvs);
            m.SetUVs(5, uvs);
            m.SetUVs(6, uvs);
            m.SetUVs(7, uvs);
            //m.SetUVs(0, uvs4);
            //m.SetUVs(1, uvs4);
            //m.SetUVs(2, uvs4);
            //m.SetUVs(3, uvs4);
            //m.SetUVs(4, uvs4);
            //m.SetUVs(5, uvs4);
            //m.SetUVs(6, uvs4);
            //m.SetUVs(7, uvs4);

            m.RecalculateBounds();

            var assetPath = string.Format("Assets/Mesh/StaticBatch/mesh_{0}.asset", (i * 4));
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
            var dirPath = Path.GetDirectoryName(fullPath);
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
            AssetDatabase.CreateAsset(m, assetPath);

            meshList.Add(m);
        }
        return meshList;
    }
}
