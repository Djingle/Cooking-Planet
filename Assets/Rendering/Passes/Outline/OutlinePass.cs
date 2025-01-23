using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static OutlineFeature;

internal class OutlinePass : ScriptableRenderPass {
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Outline");
    Material m_Material;
    RTHandle m_CameraColorTarget;

    private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
	private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
	private static readonly int DepthThresholdProperty = Shader.PropertyToID("_DepthThreshold");
	private static readonly int NormalThresholdProperty = Shader.PropertyToID("_NormalThreshold");
	private static readonly int LuminanceThresholdProperty = Shader.PropertyToID("_LuminanceThreshold");

	public OutlinePass(Material material) {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        m_Material = material;
    }

    public void SetUp(ref OutlineSettings settings) {
		m_Material.SetFloat(OutlineThicknessProperty, settings.outlineThickness);
		m_Material.SetColor(OutlineColorProperty, settings.outlineColor);
		m_Material.SetFloat(DepthThresholdProperty, settings.depthThreshold);
		m_Material.SetFloat(NormalThresholdProperty, settings.normalThreshold);
		m_Material.SetFloat(LuminanceThresholdProperty, settings.luminanceThreshold);
	}

    public void SetTarget(RTHandle target) {
        m_CameraColorTarget = target;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
        ConfigureTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, m_ProfilingSampler)) {
            CoreUtils.SetRenderTarget(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);
        } 
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
}