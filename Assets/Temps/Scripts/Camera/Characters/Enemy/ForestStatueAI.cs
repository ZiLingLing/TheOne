using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Roguelike;

namespace Roguelike
{
    public class ForestStatueAI : Tree
    {

        [SerializeField] private int m_id;
        public Transform m_target;

        public GameObject m_bullet;

        //public float m_minShootInterval = 0.2f;
        //public float m_maxShootInterval = 0.6f;
        public float m_min0ffsetAngle = -15f;
        public float m_maxOffsetAngle = 15f;

        private bool m_isDestoryed = false;

        public GameObject m_spawnVFX;

        private void Awake()
        {
            m_target = EventManager.TriggerEvent<Transform>("GetPlayerTransform");
        }

        private void OnEnable()
        {
            if (m_spawnVFX != null)
            {
                Instantiate(m_spawnVFX, this.transform.position, Quaternion.identity);
            }

            EventManager.AddEventListener<float, Object>("Wound", Wound);
            EventManager.AddEventListener<Object, int>("GetWeight", GetWeight);
            StartCoroutine(ShootBullets());
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 2f, this.transform.position.z);

            m_isDestoryed = false;
        }


        private void OnDisable()
        {
            StopCoroutine(ShootBullets());
            EventManager.RemoveEventListener<Object, int>("GetWeight", GetWeight);
            EventManager.RemoveEventListener<float, Object>("Wound", Wound);

            if (m_isDestoryed == false)
            {
                Destroy(this.gameObject);
                //GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
            }
        }

        private IEnumerator ShootBullets()
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            while (true)
            {

                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        //GameObject bullet = Instantiate(m_bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                        GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(this.transform.position.x, transform.position.y, this.transform.position.z),
                                Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject, 0.3f, 0.3f, 0.3f);
                        bullet.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        float angle = Random.Range(m_min0ffsetAngle, m_maxOffsetAngle);
                        Quaternion bulletRotation = Quaternion.Euler(0f, i * 30f + angle, 0f);
                        Vector3 bulletDirection = bulletRotation * transform.forward;
                        EventManager.TriggerEvent<Vector3, float, float, Object>("ShootBullet", bulletDirection, 1f, m_attack, bullet);
                    }

                    float interval = Random.Range(0.2f, 0.8f);
                    yield return new WaitForSeconds(interval);
                }
                yield return new WaitForSeconds(m_attackInverval);
            }
        }


        /// <summary>
        ///  ‹…À
        /// </summary>
        public override void Wound(float hit, Object woundCharacter)
        {
            if (woundCharacter == this.gameObject)
            {
                m_currentHp -= hit;
                if (m_currentHp <= 0)
                {
                    m_isDestoryed = true;
                    Dead();
                }
            }
        }

        /// <summary>
        /// À¿Õˆ
        /// </summary>
        public override void Dead()
        {
            //GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
            EventManager.TriggerEvent("OnEnemyDie");
            Destroy(this.gameObject);
        }

        /// <summary>
        /// ≥ØœÚ
        /// </summary>
        public override void FaceDirection()
        {
            Vector3 target = new Vector3(m_target.position.x, this.transform.position.y, m_target.position.z);
            this.transform.LookAt(target);
        }

    }

}

