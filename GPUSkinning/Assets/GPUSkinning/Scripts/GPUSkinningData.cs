using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GPUSkinningData : ScriptableObject
{
    // 骨骼动画图宽度
    public int m_Width;
    // 骨骼动画图高度
    public int m_Height;
    // 骨骼数量
    public int m_BoneCount;
    // 采样帧率
    public int m_SampleFPS;
    public Texture m_AnimationTex;

    public GPUSkinningClipData[] m_ClipData;

}

[Serializable]
public class GPUSkinningClipData
{
    public string m_Name;
    public int m_Start;
    public int m_Count;
    public int m_Loop;
}
