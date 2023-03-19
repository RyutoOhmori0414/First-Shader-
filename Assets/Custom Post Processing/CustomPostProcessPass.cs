using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessPass : ScriptableRenderPass
{
    //CommandBuffer�̎擾�Ɏg�p���閼�O
    const string k_RenderCustomPostProcessingTag = "Render Custom PostProcessing Effects";
    
    //���o��
    private RenderTargetIdentifier _passSource;
    private RenderTargetHandle _passDestination;

    //Blit�Ɏg�p����}�e���A��
    private Material _grayScaleMaterial;

    //�ꎞ�I�ȃ����_�[�^�[�Q�b�g�i�p�X�̓��o�͂�����̏ꍇ�A��x���ԃo�b�t�@�������Blit����K�v�����邽�߁j
    private RenderTargetHandle _temporaryColorTexture;

    public CustomPostProcessPass(RenderPassEvent renderPassEvent, Shader grayScaleShader)
    {
        //�p�X�̎��s�^�C�~���O
        this.renderPassEvent = renderPassEvent;

        if(grayScaleShader)
        {
            _grayScaleMaterial = new Material(grayScaleShader);
        }

        //�ꎞ�o�b�t�@�̐ݒ�
        _temporaryColorTexture.Init("_TemporaryColorTexture");
    }

    public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
    {
        this._passSource = source;
        this._passDestination = destination;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //�����_�����O���i��ʃT�C�Y�j�B�ꎞ�o�b�t�@�̍쐬�Ɏg�p
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

        //�G�t�F�N�g�̓K�p
        DoEffectGrayScale(cmd, _passSource, _temporaryColorTexture, opaqueDesc);

        //�o�͐�ւ̃R�s�[
        if (_passDestination == RenderTargetHandle.CameraTarget)
        {
            Blit(cmd, _temporaryColorTexture.Identifier(), _passSource);
        }
    }
}
