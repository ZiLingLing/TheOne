using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class Shield : MonoBehaviour
    {
        /// <summary>
        /// Я��װ����Ŀ��
        /// </summary>
        public Transform m_target = null;

        /// <summary>
        /// �Ƿ�װ������Ǹ�װ����״̬���Ƿ��ڵ��ϻ���Χ�ƽ�ɫ
        /// </summary>
        private bool m_isEquipped = false;

        private Transform m_transform;

        private bool m_isDestoryed = false;

        /// <summary>
        /// ���ƾ���Ŀ��ľ���
        /// </summary>
        public float m_targetDistance = 1.5f;
        private void OnEnable()
        {
            EventManager.AddEventListener<Transform>("SwitchState", SwitchState);
            m_isDestoryed = false;
        }

        private void Start()
        {
            m_transform = this.GetComponent<Transform>();
        }

        private void Update()
        {
            if (m_isEquipped == false)
            {
                ShieldUnepuipped();
            }
            else
            {
                //float currentDistance = Vector3.Distance(m_transform.position, m_target.position);
                float xDistance = Mathf.Abs(m_transform.position.x - m_target.position.x);
                float zDistance = Mathf.Abs(m_transform.position.z - m_target.position.z);
                float currentDistance = xDistance * xDistance + zDistance * zDistance;
                m_transform.RotateAround(m_target.position, Vector3.up, 180f * Time.deltaTime);
                if (currentDistance != m_targetDistance * m_targetDistance)
                {
                    //m_transform.position = (m_transform.position - m_target.position).normalized * m_targetDistance + m_target.position;
                    m_transform.position = (new Vector3(m_transform.position.x, m_target.position.y + 0.7f, m_transform.position.z) -
                        m_target.position).normalized * m_targetDistance + m_target.position;
                    //m_transform.position = new Vector3(m_transform.position.x, m_target.position.y, m_transform.position.z);
                }
            }
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<Transform>("SwitchState", SwitchState);
            Destroy(this.gameObject);
        }

        public void ShieldEquipped()
        {
            m_transform.position = Vector3.zero;
            m_isEquipped = true;
        }

        /// <summary>
        /// װ��δװ��ʱ
        /// </summary>
        public void ShieldUnepuipped()
        {
            m_transform.Rotate(Vector3.up, 30f * Time.deltaTime);
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="transform"></param>
        public void SwitchState(Transform transform)
        {
            Debug.Log(transform == m_transform);
            if (transform == m_transform)
            {
                if (m_isEquipped == true)
                {
                    m_target = null;
                    m_isEquipped = false;
                }
                else
                {
                    m_target = EventManager.TriggerEvent<Transform>("GetPlayerTransform");

                    
                    m_isEquipped = true;
                }
            }
        }


    }
}

