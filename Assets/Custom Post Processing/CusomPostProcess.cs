using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CusomPostProcess : ScriptableRendererFeature
{
    [System.Serializable]
    public class CusitomPostProcessSettings
    {
        // パスの実行タイミング
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
        // 使用するシェーダー
        public Shader GrayScaleShader;
    }

    public CusitomPostProcessSettings Settings = new CusitomPostProcessSettings();

    private CustomPostProcessPass pass;

    // ScriptableRendererFeatureはScriptableObjectとしてRendererData内部に格納される
    // ScriptableObjectのシリアライズのタイミングで呼ばれる
    public override void Create()
    {
        this.name = "Custom PostProcess";
        pass = new CustomPostProcessPass(Settings.Event, Settings.GrayScaleShader);
    }

    // パスの差し込み。URPのSetupで呼ばれる
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // 入出力を指定してパスを初期化
        pass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
        // パスの差し込み
        renderer.EnqueuePass(pass);
    }
}
