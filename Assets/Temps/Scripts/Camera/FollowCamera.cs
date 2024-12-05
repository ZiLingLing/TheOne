using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;

namespace Roguelike
{
    public class FollowCamera : MonoBehaviour
    {
        public CinemachineVirtualCamera m_cinemachineVirtualCamera;
        private CinemachineBasicMultiChannelPerlin m_noiseProfile;
        #region �������ں���
        private void Awake()
        {
            m_cinemachineVirtualCamera = this.GetComponent<CinemachineVirtualCamera>();
            m_noiseProfile = m_cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            Debug.Log("CinemachineVirtualCamera: " + m_cinemachineVirtualCamera);
        }

        private void OnEnable()
        {
            Debug.Log("׼������������������ط���");
            EventManager.AddEventListener<Transform>("SetCameraLookAt",SetCameraLookAt);
            EventManager.AddEventListener<Transform>("SetCameraFollow",SetCameraFollow);
            EventManager.AddEventListener<Vector3>("InitCamera", InitVirtualCamera);
            EventManager.AddEventListener("WoundShakeCamera", WoundShakeCamera);
            Debug.Log("�����������ط�����������");
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<Transform>("SetCameraLookAt", SetCameraLookAt);
            EventManager.RemoveEventListener<Transform>("SetCameraFollow", SetCameraFollow);
            EventManager.RemoveEventListener<Vector3>("InitCamera", InitVirtualCamera);
            EventManager.RemoveEventListener("WoundShakeCamera", WoundShakeCamera);
        }
        #endregion

        /// <summary>
        /// �����������������LookAt����
        /// </summary>
        /// <param name="lookAt"></param>
        private void SetCameraLookAt(Transform lookAt)
        {
            //Debug.Log("����lookat"+lookAt==null+lookAt.gameObject.name);
            m_cinemachineVirtualCamera.LookAt = lookAt;
        }

        /// <summary>
        /// �����������������Follow����
        /// </summary>
        /// <param name="follow"></param>
        private void SetCameraFollow(Transform follow)
        {
            Debug.Log(("����follow"+follow)==null);
            m_cinemachineVirtualCamera.Follow = follow;
        }

        /// <summary>
        /// ���˾�ͷ����
        /// </summary>
        private void WoundShakeCamera()
        {
            float shakeTime = 0.5f;

            m_noiseProfile.m_PivotOffset = new Vector3(5f, 0, 5f);
            m_noiseProfile.m_AmplitudeGain = 3f;
            m_noiseProfile.m_FrequencyGain = 0.15f;
            StartCoroutine(ShakeCoroutine(shakeTime));

        }

        /// <summary>
        /// ��ʼ����������ĸ������
        /// </summary>
        private void InitVirtualCamera(Vector3 position)
        {
            Vector3 initPosition = new Vector3(position.x, position.y, position.z);
            this.transform.position = initPosition;
            this.transform.rotation = Quaternion.Euler(90f, 0, 0);

            //����transposer
            CinemachineTransposer transposer = m_cinemachineVirtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            //transposer = m_cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            transposer.m_FollowOffset = new Vector3(0, 15.5f, 0);
            transposer.m_XDamping = transposer.m_YDamping = transposer.m_ZDamping = 1.5f;

        }

        /// <summary>
        /// ��ʱ��ļ�ʱЭ��
        /// </summary>
        /// <param name="shakeTime"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private IEnumerator ShakeCoroutine(float shakeTime)
        {
            yield return new WaitForSeconds(shakeTime);
            m_noiseProfile.m_AmplitudeGain = 0;
            m_noiseProfile.m_FrequencyGain = 0;
        }

    }


}
