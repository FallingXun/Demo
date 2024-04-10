using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUSkinning : MonoBehaviour
{
    public GPUSkinningData m_GPUSkinningData;
    private float m_StartTime = 0f;
    private float m_StopTime = 0f;
    private int m_Count = 0;
    private int m_Loop = 0;
    private int m_Start = 0;

    public int m_Index = 0;
    public float m_PlaySpeed = 1f;
    public bool m_AutoPlay = true;

    private MaterialPropertyBlock m_Mpb;
    private MeshRenderer m_MeshRender;


    private void Awake()
    {
        Play();
    }

    private void OnEnable()
    {
        
    }


    [ContextMenu("Play")]
    private void Play()
    {
        if(m_MeshRender == null)
        {
            m_MeshRender = GetComponentInChildren<MeshRenderer>(true);
        }
        if (m_GPUSkinningData != null)
        {
            m_StartTime = Time.timeSinceLevelLoad;
            m_Start = m_GPUSkinningData.m_ClipData[m_Index].m_Start;
            m_Count = m_GPUSkinningData.m_ClipData[m_Index].m_Count;
            m_Loop = m_GPUSkinningData.m_ClipData[m_Index].m_Loop;
            m_MeshRender.sharedMaterial.SetTexture(GPUSkinningShaderID.ShaderID_AnimationTex, m_GPUSkinningData.m_AnimationTex);
            m_MeshRender.sharedMaterial.SetInt(GPUSkinningShaderID.ShaderID_Width, m_GPUSkinningData.m_Width);
            m_MeshRender.sharedMaterial.SetInt(GPUSkinningShaderID.ShaderID_Height, m_GPUSkinningData.m_Height);
            m_MeshRender.sharedMaterial.SetInt(GPUSkinningShaderID.ShaderID_BoneCount, m_GPUSkinningData.m_BoneCount);
            m_MeshRender.sharedMaterial.SetInt(GPUSkinningShaderID.ShaderID_SampleFPS, m_GPUSkinningData.m_SampleFPS);
        }
        if (m_Mpb == null)
        {
            m_Mpb = new MaterialPropertyBlock();
        }
        m_Mpb.SetInt(GPUSkinningShaderID.ShaderID_Start, m_Start);
        m_Mpb.SetInt(GPUSkinningShaderID.ShaderID_Count, m_Count);
        m_Mpb.SetInt(GPUSkinningShaderID.ShaderID_Loop, m_Loop);
        m_Mpb.SetFloat(GPUSkinningShaderID.ShaderID_StartTime, m_StartTime);
        m_Mpb.SetFloat(GPUSkinningShaderID.ShaderID_StopTime, m_StopTime);
        m_Mpb.SetFloat(GPUSkinningShaderID.ShaderID_PlaySpeed, m_PlaySpeed);
        m_MeshRender.SetPropertyBlock(m_Mpb);
    }


    [ContextMenu("UpdateSpeed")]
    private void UpdateSpeed()
    {
        if (m_MeshRender == null)
        {
            m_MeshRender = GetComponentInChildren<MeshRenderer>(true);
        }
        if (m_Mpb == null)
        {
            m_Mpb = new MaterialPropertyBlock();
        }
        if (m_PlaySpeed <= 0)
        {
            m_PlaySpeed = 0;
            m_StopTime = Time.timeSinceLevelLoad;
        }
        else
        {
            if (m_StopTime > 0)
            {
                m_StartTime += Time.timeSinceLevelLoad - m_StopTime;
            }
            m_StopTime = 0;
        }
        m_Mpb.SetFloat(GPUSkinningShaderID.ShaderID_StartTime, m_StartTime);
        m_Mpb.SetFloat(GPUSkinningShaderID.ShaderID_StopTime, m_StopTime);
        m_Mpb.SetFloat(GPUSkinningShaderID.ShaderID_PlaySpeed, m_PlaySpeed);
        m_MeshRender.SetPropertyBlock(m_Mpb);
    }
}
