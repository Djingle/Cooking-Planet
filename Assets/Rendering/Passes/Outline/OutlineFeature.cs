using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class OutlineFeature : ScriptableRendererFeature {

    public Shader m_Shader;
    Material m_Material;
    OutlinePass m_RenderPass = null;
	[SerializeField] private OutlineSettings settings;

	[Serializable]
    public class OutlineSettings
    {
        public RTHandle target = null;

        [Range(0, 15)] public float outlineThickness = 3;
        public Color outlineColor = Color.black;

        [Range(0, .01f)] public float depthThreshold = .005f;
        [Range(0, 1.1f)] public float normalThreshold = .25f;
        [Range(0, 5)] public float luminanceThreshold = 2f;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                    ref RenderingData renderingData) {
        if (renderingData.cameraData.cameraType == CameraType.Game) {

			m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
			m_RenderPass.SetUp(ref settings);
            renderer.EnqueuePass(m_RenderPass);
        }
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer,
                                        in RenderingData renderingData) {
        if (renderingData.cameraData.cameraType == CameraType.Game) { 
            // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
            // ensures that the opaque texture is available to the Render Pass.
            //m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_RenderPass.SetTarget(renderer.cameraColorTargetHandle);
        }
    }


    public override void Create() { 
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        if (m_Shader == null)
            Debug.LogWarning("Material couldn't be created");
        m_RenderPass = new OutlinePass(m_Material);
    }

    protected override void Dispose(bool disposing) {
        CoreUtils.Destroy(m_Material);
    }
}