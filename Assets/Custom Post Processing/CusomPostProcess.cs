using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CusomPostProcess : ScriptableRendererFeature
{
    [System.Serializable]
    public class CusitomPostProcessSettings
    {
        // �p�X�̎��s�^�C�~���O
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
        // �g�p����V�F�[�_�[
        public Shader GrayScaleShader;
    }

    public CusitomPostProcessSettings Settings = new CusitomPostProcessSettings();

    private CustomPostProcessPass pass;

    // ScriptableRendererFeature��ScriptableObject�Ƃ���RendererData�����Ɋi�[�����
    // ScriptableObject�̃V���A���C�Y�̃^�C�~���O�ŌĂ΂��
    public override void Create()
    {
        this.name = "Custom PostProcess";
        pass = new CustomPostProcessPass(Settings.Event, Settings.GrayScaleShader);
    }

    // �p�X�̍������݁BURP��Setup�ŌĂ΂��
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // ���o�͂��w�肵�ăp�X��������
        pass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
        // �p�X�̍�������
        renderer.EnqueuePass(pass);
    }
}
