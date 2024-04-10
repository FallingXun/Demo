using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GPUSkinningData : ScriptableObject
{
    // ��������ͼ���
    public int m_Width;
    // ��������ͼ�߶�
    public int m_Height;
    // ��������
    public int m_BoneCount;
    // ����֡��
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
