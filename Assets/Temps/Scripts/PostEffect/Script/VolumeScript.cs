using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Roguelike
{
    public class VolumeScript : MonoBehaviour
    {
        public Volume m_volume;

        private void Awake()
        {
            m_volume = this.GetComponent<Volume>();
        }

        private void OnEnable()
        {
            EventManager.AddEventListener<float>("ChangeBrightness", ChangeBrightness);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<float>("ChangeBrightness", ChangeBrightness);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="brightnessValue"></param>
        private void ChangeBrightness(float brightnessValue)
        {
            // ��ȡVolumeProfile
            VolumeProfile volumeProfile = m_volume.profile;

            // ���VolumeProfile�Ƿ����
            if (volumeProfile == null)
            {
                return;
            }

            // ��VolumeProfile�в��Ҷ�Ӧ��VolumeComponent������ΪBrightnessSaturationAndContrast��
            if (volumeProfile.TryGet(out BrightnessSaturationAndContrast brightnessSaturationAndContrast))
            {
                brightnessSaturationAndContrast.m_brightness.value = brightnessValue;
            }
        }

        /// <summary>
        /// ����Y������Ч�ĸ߶�
        /// </summary>
        /// <param name="yValue"></param>
        private void ChangeFogHeight(float yValue)
        {

        }
    }
}
