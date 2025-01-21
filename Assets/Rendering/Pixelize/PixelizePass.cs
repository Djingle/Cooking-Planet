using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizePass : ScriptableRenderPass
{
    private PixelizeFeature.CustomPassSettings m_settings;

    private RenderTargetIdentifier m_colorBuffer, m_pixelBuffer;
    private RTHandle m_CameraColorTarget, m_Camera;
    private int m_pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    //private RenderTargetIdentifier pointBuffer;
    //private int pointBufferID = Shader.PropertyToID("_PointBuffer");

    private Material m_PixelizeMaterial;
    private int m_pixelScreenHeight, m_pixelScreenWidth;

    public PixelizePass(PixelizeFeature.CustomPassSettings settings)
    {
        this.m_settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        if (m_PixelizeMaterial == null) m_PixelizeMaterial = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        m_colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        // m_CameraColorTarget = 
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        //cmd.GetTemporaryRT(pointBufferID, descriptor.width, descriptor.height, 0, FilterMode.Point);
        //pointBuffer = new RenderTargetIdentifier(pointBufferID);

        m_pixelScreenHeight = m_settings.screenHeight;
        m_pixelScreenWidth = (int)(m_pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

        m_PixelizeMaterial.SetVector("_BlockCount", new Vector2(m_pixelScreenWidth, m_pixelScreenHeight));
        m_PixelizeMaterial.SetVector("_BlockSize", new Vector2(1.0f / m_pixelScreenWidth, 1.0f / m_pixelScreenHeight));
        m_PixelizeMaterial.SetVector("_HalfBlockSize", new Vector2(0.5f / m_pixelScreenWidth, 0.5f / m_pixelScreenHeight));

        descriptor.height = m_pixelScreenHeight;
        descriptor.width = m_pixelScreenWidth;

        cmd.GetTemporaryRT(m_pixelBufferID, descriptor, FilterMode.Point);
        m_pixelBuffer = new RenderTargetIdentifier(m_pixelBufferID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Pixelize Pass"))) {
            // No-shader variant
            //Blit(cmd, colorBuffer, pointBuffer);
            //Blit(cmd, pointBuffer, pixelBuffer);
            //Blit(cmd, pixelBuffer, colorBuffer);

            // Old blits
            //Blit(cmd, m_colorBuffer, m_pixelBuffer, m_PixelizeMaterial, 0);
            //Blit(cmd, m_pixelBuffer, m_colorBuffer); 
            // Blitter.BlitCameraTexture(cmd, )


        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(m_pixelBufferID);
        //cmd.ReleaseTemporaryRT(pointBufferID);
    }

}