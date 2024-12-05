using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Roguelike
{
    public class DungeonWormAI : Tree
    {
        public Transform m_player;
        private NavMeshAgent m_agent;

        [SerializeField] private int m_id;

        private bool m_isLocked = false;

        private float m_currentSpeed;

        public float m_chaseDistance = 4f;

        public float m_retreatDistance = 4f;
        public float m_retreatSpeed = 8f;

        public float m_standTime = 3f;

        private bool m_isChasing = true;

        private bool m_isDestoryed = false;

        public GameObject m_spawnVFX;

        private void OnEnable()
        {
            if (m_spawnVFX != null)
            {
                Instantiate(m_spawnVFX, this.transform.position, Quaternion.identity);
            }

            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.7f, this.transform.position.z);
            m_player = EventManager.TriggerEvent<Transform>("GetPlayerTransform");
            m_isLocked = false;
            m_currentSpeed = m_speed;
            StartCoroutine(MoveCoroutine());

            m_isDestoryed = false;
        }

        private void FixedUpdate()
        {
            if (m_isLocked == true)
            {
                m_currentSpeed += 1f;
            }
        }

        private void Update()
        {
            if (m_isLocked == false)
            {
                this.transform.LookAt(new Vector3(m_player.position.x, this.transform.position.y, m_player.position.z));
            }
            else
            {
                Vector3 destination = new Vector3(m_player.position.x, this.transform.position.y, m_player.position.z);
                Vector3 direction = (destination - this.transform.position).normalized;
                this.transform.LookAt(destination * 20f);

                this.transform.position += direction * m_currentSpeed * Time.deltaTime;
                RayDetect();
            }
        }

        private void OnDisable()
        {
            StopCoroutine(MoveCoroutine());

            if (m_isDestoryed == false)
            {
                Destroy(this.gameObject);
                //GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
            }
            
        }

        private IEnumerator MoveCoroutine()
        {
            yield return new WaitForSeconds(Random.Range(1f, 4f));
            m_isLocked = true;
        }

        private void RayDetect()
        {
            float rayLength = 1.5f;
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
                            EventManager.TriggerEvent<float, Object>("Wound", m_attack, hitObject);
                            Dead();
                            Debug.Log("自爆虫撞击到物体：" + hitObject.name); break;
                        case "Wall":
                            Dead(); break;
                        default:
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// 受伤
        /// </summary>
        public override void Wound(float hit, Object woundCharacter)
        {
            if (woundCharacter == this.gameObject)
            {
                m_currentHp -= hit;
                if (m_currentHp <= 0)
                {
                    Dead();
                }
            }
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public override void Dead()
        {
            float rayLength = 2f;
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
                    if(hitObject.tag== "Player")
                    {
                        EventManager.TriggerEvent<float, Object>("Wound", m_attack, hitObject);
                        Debug.Log("自爆虫亡语攻击到物体：" + hitObject.name);
                    }
                }
            }
            Destroy(this.gameObject);
            EventManager.TriggerEvent("OnEnemyDie");
            //GameObjectPoolManager.ReturnObjectToPool(this.gameObject);

        }

    }
}

