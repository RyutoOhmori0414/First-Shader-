using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessPass : ScriptableRenderPass
{
    //CommandBufferの取得に使用する名前
    const string k_RenderCustomPostProcessingTag = "Render Custom PostProcessing Effects";
    
    //入出力
    private RenderTargetIdentifier _passSource;
    private RenderTargetHandle _passDestination;

    //Blitに使用するマテリアル
    private Material _grayScaleMaterial;

    //一時的なレンダーターゲット（パスの入出力が同一の場合、一度中間バッファを挟んでBlitする必要があるため）
    private RenderTargetHandle _temporaryColorTexture;

    public CustomPostProcessPass(RenderPassEvent renderPassEvent, Shader grayScaleShader)
    {
        //パスの実行タイミング
        this.renderPassEvent = renderPassEvent;

        if(grayScaleShader)
        {
            _grayScaleMaterial = new Material(grayScaleShader);
        }

        //一時バッファの設定
        _temporaryColorTexture.Init("_TemporaryColorTexture");
    }

    public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
    {
        this._passSource = source;
        this._passDestination = destination;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //レンダリング情報（画面サイズ）。一時バッファの作成に使用
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;

        var cmd = CommandBufferPool.Get(k_RenderCustomPostProcessingTag);

        Render(cmd, ref renderingData, opaqueDesc);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void Render(CommandBuffer cmd, ref RenderingData renderingData, RenderTextureDescriptor opaqueDesc)
    {
        cmd.GetTemporaryRT(_temporaryColorTexture.id, opaqueDesc, FilterMode.Bilinear);

        //エフェクトの適用
        DoEffectGrayScale(cmd, _passSource, _temporaryColorTexture, opaqueDesc);

        //出力先へのコピー
        if (_passDestination == RenderTargetHandle.CameraTarget)
        {
            Blit(cmd, _temporaryColorTexture.Identifier(), _passSource);
        }
    }
}
