using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

    namespace Roguelike
    {
        public class AdditionPostProcessPass : ScriptableRenderPass
        {
            //��ǩ����������֡����������ʾ����������
            const string CommandBufferTag = "AdditionalPostProcessing Pass";

            // ���ں���Ĳ���
            public Material m_Material;

            // ���Բ������
            BrightnessSaturationAndContrast m_BrightnessSaturationContrast;

            // ��ɫ��Ⱦ��ʶ��
            RenderTargetIdentifier m_ColorAttachment;
            // ��ʱ����ȾĿ��
            RenderTargetHandle m_TemporaryColorTexture01;

            // ������Ⱦ����
            public void Setup(RenderTargetIdentifier _ColorAttachment, Material Material)
            {
                this.m_ColorAttachment = _ColorAttachment;

                m_Material = Material;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                // ��Volume����л�ȡ���ж�ջ
                var stack = VolumeManager.instance.stack;
                // �Ӷ�ջ�в��Ҷ�Ӧ�����Բ������
                m_BrightnessSaturationContrast = stack.GetComponent<BrightnessSaturationAndContrast>();

                // ������������л�ȡһ������ǩ������������ñ�ǩ�������ں���֡�������м���
                var cmd = CommandBufferPool.Get(CommandBufferTag);

                // ������Ⱦ����
                Render(cmd, ref renderingData);

                // ִ���������
                context.ExecuteCommandBuffer(cmd);
                // �ͷ������
                CommandBufferPool.Release(cmd);
                // �ͷ���ʱRT
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            }

            // ��Ⱦ
            void Render(CommandBuffer cmd, ref RenderingData renderingData)
            {
                // VolumeComponent�Ƿ������ҷ�Scene��ͼ�����
                if (m_BrightnessSaturationContrast.IsActive() && !renderingData.cameraData.isSceneViewCamera)
                {
                    // д�����
                    m_Material.SetFloat("_Brightness", m_BrightnessSaturationContrast.m_brightness.value);
                    m_Material.SetFloat("_Saturation", m_BrightnessSaturationContrast.m_saturation.value);
                    m_Material.SetFloat("_Contrast", m_BrightnessSaturationContrast.m_contrast.value);

                    // ��ȡĿ�������������Ϣ
                    RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                    // ������Ȼ�����
                    opaqueDesc.depthBufferBits = 0;
                    // ͨ��Ŀ���������Ⱦ��Ϣ������ʱ������
                    cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc);

                    // ͨ�����ʣ���������������ʱ������
                    cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier(), m_Material);
                    // �ٴ���ʱ����������������
                    cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment);
                }
            }
        }

    }
