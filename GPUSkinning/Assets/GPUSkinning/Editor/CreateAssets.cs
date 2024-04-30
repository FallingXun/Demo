using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;

public class CreateAssets : Editor
{
    [MenuItem("Assets/GPUSkinning/Create")]
    [MenuItem("GameObject/GPUSkinning/Create")]
    public static void CreateGPUSkinning()
    {
        var mesh = CreateMesh();
        var v = CreateAni();
        var mat = CreateMat(v.Item2);
        var pref = CreatePref(mesh, mat, v.Item2);
    }

    public static Mesh CreateMesh()
    {
        var go = Selection.activeGameObject;
        var skr = go.GetComponentInChildren<SkinnedMeshRenderer>(true);
        if (skr == null)
        {
            return null;
        }
        var mesh = new Mesh();
        mesh.vertices = skr.sharedMesh.vertices;
        mesh.normals = skr.sharedMesh.normals;
        //mesh.tangents = skr.sharedMesh.tangents;
        List<Vector4> uv2s = new List<Vector4>(skr.sharedMesh.vertexCount);
        List<Vector4> uv3s = new List<Vector4>(skr.sharedMesh.vertexCount);
        var boneIndex = new int[4];
        for (int i = 0; i < skr.sharedMesh.vertexCount; i++)
        {
            var boneWeight = skr.sharedMesh.boneWeights[i];
            boneIndex[0] = boneWeight.boneIndex0;
            boneIndex[1] = boneWeight.weight1 > 0 ? boneWeight.boneIndex1 : boneIndex[0];
            boneIndex[2] = boneWeight.weight2 > 0 ? boneWeight.boneIndex2 : boneIndex[1];
            boneIndex[3] = boneWeight.weight3 > 0 ? boneWeight.boneIndex3 : boneIndex[2];
            var uv2 = new Vector4(boneIndex[0], boneWeight.weight0, boneIndex[1], boneWeight.weight1);
            var uv3 = new Vector4(boneIndex[2], boneWeight.weight2, boneIndex[3], boneWeight.weight3);
            uv2s.Add(uv2);
            uv3s.Add(uv3);
        }
        mesh.SetUVs(0, skr.sharedMesh.uv);
        mesh.SetUVs(1, uv2s);
        mesh.SetUVs(2, uv3s);
        mesh.triangles = skr.sharedMesh.triangles;
        mesh.name = go.name + "_mesh";

        var path = string.Format("Assets/GPUSkinning/{0}/Mesh/{1}_mesh.asset", go.name, go.name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        var dir = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), path));
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        AssetDatabase.CreateAsset(mesh, path);
        return mesh;
    }

    public static (Texture, GPUSkinningData) CreateAni()
    {
        var go = Selection.activeGameObject;
        var skr = go.GetComponentInChildren<SkinnedMeshRenderer>(true);
        if (skr == null)
        {
            return (null, null);
        }
        var bindPoses = skr.sharedMesh.bindposes;
        var bones = skr.bones;
        var clips = AnimationUtility.GetAnimationClips(go);
        var fps = 15;
        var curIndex = 0;
        var clipDatas = new GPUSkinningClipData[clips.Length];
        var matrices = new List<Matrix4x4>();
        for (int i = 0; i < clips.Length; i++)
        {
            var clip = clips[i];
            var name = clip.name;
            var frame = (int)(clip.length * fps);
            for (int j = 0; j < frame; j++)
            {
                clip.SampleAnimation(go, 1.0f / fps * j);
                for (int k = 0; k < bones.Length; k++)
                {
                    var matrix = go.transform.worldToLocalMatrix * bones[k].localToWorldMatrix * bindPoses[k];
                    matrices.Add(matrix);
                }
            }
            GPUSkinningClipData clipData = new GPUSkinningClipData();
            clipData.m_Name = name;
            clipData.m_Start = curIndex;
            clipData.m_Count = frame;
            clipData.m_Loop = clip.isLooping ? 1 : 0;
            clipDatas[i] = clipData;

            curIndex += frame;
        }
        int width = 4;
        int height = 4;
        bool sign = true;
        while (width * height < matrices.Count * 4)
        {
            if (sign)
            {
                width *= 2;
            }
            else
            {
                height *= 2;
            }
            sign = !sign;
        }
        var tex = new Texture2D(width, height, TextureFormat.RGBAHalf, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < matrices.Count; i++)
        {
            var x = i % width;
            var y = i / width * 4;
            for (int j = 0; j < 4; j++)
            {
                var col = matrices[i].GetRow(j);
                tex.SetPixel(x, y + j, col);
            }
        }
        tex.Apply();

        var path = string.Format("Assets/GPUSkinning/{0}/Animation/{1}_animation.asset", go.name, go.name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        var dir = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), path));
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        AssetDatabase.CreateAsset(tex, path);


        path = string.Format("Assets/GPUSkinning/{0}/Data/{1}_data.asset", go.name, go.name);
        var data = new GPUSkinningData();
        data.m_ClipData = clipDatas;
        data.m_Width = width;
        data.m_Height = height;
        data.m_BoneCount = bones.Length;
        data.m_SampleFPS = fps;
        data.m_AnimationTex = tex;
        dir = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), path));
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        AssetDatabase.CreateAsset(data, path);

        return (tex, data);

    }


    public static Material CreateMat(GPUSkinningData data)
    {
        var go = Selection.activeGameObject;
        var skr = go.GetComponentInChildren<SkinnedMeshRenderer>(true);
        if (skr == null)
        {
            return null;
        }

        var path = string.Format("Assets/GPUSkinning/{0}/Material/{1}_material.mat", go.name, go.name);
        Material mat = new Material(Shader.Find("Unlit/GPUSkinning"));
        mat.SetInt("_Width", data.m_Width);
        mat.SetInt("_Height", data.m_Height);
        mat.SetInt("_BoneCount", data.m_BoneCount);
        mat.SetTexture("_MainTex", skr.sharedMaterial.mainTexture);
        mat.enableInstancing = true;
        var dir = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), path));
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    public static GameObject CreatePref(Mesh mesh, Material mat, GPUSkinningData data)
    {
        var go = Selection.activeGameObject;
        var skr = go.GetComponentInChildren<SkinnedMeshRenderer>(true);
        if (skr == null)
        {
            return null;
        }

        GameObject pref = new GameObject(go.name);
        var gpu = pref.AddComponent<GPUSkinning>();
        gpu.m_GPUSkinningData = data;
        GameObject child = new GameObject(go.name);
        child.transform.parent = pref.transform;
        var mf = child.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        var mr = child.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        var path = string.Format("Assets/GPUSkinning/{0}/Prefab/{1}_GPU.prefab", go.name, go.name);
        var dir = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), path));
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        PrefabUtility.SaveAsPrefabAsset(pref, path);
        return pref;
    }


    [MenuItem("Assets/GPUSkinning/Instantiate")]
    public static void CreateInstance()
    {
        var go = Selection.activeGameObject;
        if (go == null)
        {
            return;
        }
        var root = new GameObject("Root_" + go.name);

        int[] groups = new int[] { 10, 50, 100, 500, 1000 };
        for (int i = 0; i < groups.Length; i++)
        {
            int row = Mathf.CeilToInt(Mathf.Sqrt(groups[i] / ((3f / 5) * (2f / 5))) * (3f / 5));
            int column = Mathf.CeilToInt(Mathf.Sqrt(groups[i] / ((3f / 5) * (2f / 5))) * (2f / 5));
            float width = 20f;
            float height = 15f;
            float offset_x = width / row;
            float offset_y = height / column;
            var group = new GameObject("Group" + groups[i]);
            group.transform.SetParent(root.transform);
            for (int j = 0; j < groups[i]; j++)
            {
                var child = GameObject.Instantiate(go, group.transform);
                child.transform.localPosition = new Vector3(offset_x * row / 2 - j % row * offset_x, 0, offset_y * column / 2 - j / row * offset_y);
            }
            group.SetActive(false);
        }
        root.transform.localPosition = new Vector3(0, -8, 5);
        root.transform.localEulerAngles = new Vector3(0, 180, 0);

    }
}
