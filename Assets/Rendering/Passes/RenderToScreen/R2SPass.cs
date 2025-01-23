using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class R2SPass : ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("R2S Pass");
    Material m_Material;
    RTHandle m_CameraColorTarget, m_DownScaleRT;
    RenderTexture m_DownScaleTexture;

    public R2SPass(Material material, RenderTexture downScaleTexture)
    {
        m_Material = material;
        m_DownScaleTexture = downScaleTexture;
        // Debug.Log("instanciating pass, dst name : " + downScaleTexture.name);
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetTarget(RTHandle colorHandle)
    {
        m_CameraColorTarget = colorHandle; 
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //var dsDesc = m_DownScaleTexture.descriptor;
        if (m_DownScaleRT == null) {
            //m_DownScaleRT = RTHandles.Alloc(Vector2.one, dsDesc);
            m_DownScaleRT = RTHandles.Alloc(m_DownScaleTexture);
            // Debug.Log("allocating rthandle, scaled size : " + m_DownScaleRT.GetScaledSize() + ", reference size : " + m_DownScaleRT.referenceSize);
        }
        ConfigureTarget(m_CameraColorTarget);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler)) {
            Blitter.BlitCameraTexture(cmd, m_DownScaleRT, m_CameraColorTarget);
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
}