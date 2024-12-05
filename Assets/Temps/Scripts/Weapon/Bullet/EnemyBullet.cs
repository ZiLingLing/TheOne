using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class EnemyBullet : MonoBehaviour
    {
        /// <summary>
        /// �ӵ���������ʱ��
        /// </summary>
        public float m_bulletLifeTime;

        /// <summary>
        /// �����ٶ�
        /// </summary>
        public float m_dashSpeed = 5f;

        /// <summary>
        /// ���з���
        /// </summary>
        public Vector3 m_dashDirection;

        /// <summary>
        /// ���е��ٶȱ���
        /// </summary>
        public float m_speedImpactFactor = 1;

        /// <summary>
        /// �ӵ�����ɵ��˺�
        /// </summary>
        public float m_damage = 1f;

        private bool m_isDestoryed = false;

        public GameObject m_destoryVFX;

        #region �������ں���

        private void OnEnable()
        {
            EventManager.AddEventListener<Vector3, float, float, Object>("ShootBullet", GetBulletProperty);
            StartCoroutine(TimeSpanCoroutine());
            m_isDestoryed = false;
        }

        private void Update()
        {
            Dash();
            RayDetect();
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<Vector3, float, float, Object>("ShootBullet", GetBulletProperty);
            StopCoroutine(TimeSpanCoroutine());
            if (m_isDestoryed == false)
            {
                BulletDestory();
            }
        }
        #endregion

        #region �ӵ���ص��߼�
        /// <summary>
        /// �ӵ�����
        /// </summary>
        private void Dash()
        {
            this.transform.position += m_dashDirection.normalized * m_dashSpeed * m_speedImpactFactor * Time.deltaTime;
        }

        /// <summary>
        /// ��ȡ�ӵ��ķ����ٶȺͷ��з���
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speedImpactFactor"></param>
        private void GetBulletProperty(Vector3 direction, float speedImpactFactor, float damage, Object info)
        {
            if ((info as GameObject).GetComponent<EnemyBullet>() == this)
            {
                m_speedImpactFactor = speedImpactFactor;
                m_dashDirection = direction;
                m_damage = damage;
            }

        }

        /// <summary>
        /// ��һ��������ӵ��ݻ�
        /// </summary>
        private void BulletDestory()
        {
            if (m_destoryVFX != null)
            {
                Instantiate(m_destoryVFX, this.transform.position, Quaternion.identity, GameObject.Find("Pool Objects/Particle Effects").transform);
            }
            GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
        }

        /// <summary>
        /// ���߼���ӵ��Ƿ�����
        /// </summary>
        private void RayDetect()
        {
            float rayLength = this.GetComponent<SphereCollider>().radius;
            int rayCount = 12;
            float angleStep = 15f;

            Vector3 origin = transform.position;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 direction = rotation * transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction, out hitInfo, rayLength))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    switch (hitObject.tag)
                    {
                        case "Player":
                            m_isDestoryed = true;
                            BulletDestory();
                            Debug.Log("���߻��������壺" + hitObject.name);
                            if (hitObject.GetComponent<PlayerBehaviour>().enabled == true)
                            {
                                EventManager.TriggerEvent<float, Object>("Wound", m_damage, hitObject);
                            }
                            break;
                        case "Wall":
                            m_isDestoryed = true;
                            BulletDestory(); break;
                        case "Weapon":
                            m_isDestoryed = true;
                            BulletDestory(); break;
                        default:
                            break;
                    }

                }
            }
        }
        #endregion

        #region Э��
        /// <summary>
        /// �����ӵ��������ڵ�Э��
        /// </summary>
        /// <returns></returns>
        private IEnumerator TimeSpanCoroutine()
        {
            yield return new WaitForSeconds(m_bulletLifeTime);
            this.BulletDestory();
        }

        #endregion
    }

}
