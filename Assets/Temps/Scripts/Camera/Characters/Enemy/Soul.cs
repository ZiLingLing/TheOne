using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Roguelike
{
    public class Soul : Tree
    {
        private Node m_root;

        private Animator m_soulAnimator;
        private Blackboard m_blackboard;

        public GameObject m_bullet;

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        private Transform m_target;

        /// <summary>
        /// �����ͷż��
        /// </summary>
        public float m_castSpellInterval;

        private float m_castSpellTimer;
        private float m_attackTimer;

        //�Ƿ��ڹ���������ȴ��
        private bool m_attackTrigger = false;
        private bool m_castSpellTrigger = true;

        private Vector3 m_roomPosition;

        /// <summary>
        /// ���������֮��ʹ��ҧ�������������ӵ�
        /// </summary>
        private float m_distance = 1f;

        private bool m_isDestoryed = false;

        public GameObject m_spawnVFX;

        #region �������ں���
        private void Awake()
        {
            m_soulAnimator = this.GetComponent<Animator>();
            //m_navMeshAgent = this.GetComponent<NavMeshAgent>();
            m_blackboard = this.GetComponent<Blackboard>();
            m_target = EventManager.TriggerEvent<Transform>("GetPlayerTransform");

            Init();
        }

        private void OnEnable()
        {
            if (m_spawnVFX != null)
            {
                Instantiate(m_spawnVFX, this.transform.position, Quaternion.identity);
            }

            EventManager.AddEventListener<float, Object>("Wound", Wound);
            EventManager.AddEventListener<Object, int>("GetWeight", GetWeight);
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z);

            m_isDestoryed = false;
        }

        private void Update()
        {
            m_blackboard.AddData<bool>("AttackTrigger", m_attackTrigger);
            m_blackboard.AddData<bool>("CastSpellTrigger", m_castSpellTrigger);

            if (m_root != null)
            {
                m_root.Evaluate(this.transform, m_blackboard);
                //Debug.Log(m_root.Evaluate(this.transform, m_blackboard));
            }

            CoolDownTimer(m_castSpellInterval, ref m_castSpellTimer, ref m_castSpellTrigger);
            CoolDownTimer(m_attackInverval, ref m_attackTimer, ref m_attackTrigger);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<float, Object>("Wound", Wound);
            EventManager.RemoveEventListener<Object, int>("GetWeight", GetWeight);

            if (m_isDestoryed == false)
            {
                Destroy(this.gameObject);
                //GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
            }
        }
        #endregion

        #region ��Ϊ���ڵ����
        /// <summary>
        /// ����Soul����Ϊ��
        /// </summary>
        /// <returns></returns>
        protected override void OnSetup()
        {
            m_blackboard.AddData<bool>("AttackTrigger", m_attackTrigger);
            m_blackboard.AddData<bool>("CastSpellTrigger", m_castSpellTrigger);

            SelectNode selectNode = new SelectNode();
            AttackNode attackNode = new AttackNode();
            IdleNode idleNode = new IdleNode(FaceDirection);
            SkillNode skillNode = new SkillNode(CastSpell);
            
            selectNode.AddNode(skillNode, attackNode, idleNode);

            m_root = selectNode;
        }

        #endregion

        /// <summary>
        /// ��ʼ��Soul
        /// </summary>
        private void Init()
        {
            AddAnimatorClipEvent();
            GetRoomInfo();
            OnSetup();
            m_attackTimer = Random.Range(0.5f, 5f);
            m_castSpellTimer = m_castSpellInterval;
        }

        /// <summary>
        /// Soul�ܻ�
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="woundCharacter"></param>
        public override void Wound(float hit, Object woundCharacter)
        {
            if (woundCharacter == this.gameObject)
            {
                m_currentHp -= hit;
                if (m_currentHp <= 0)
                {
                    m_isDestoryed = true;
                    Dead();
                    return;
                }
                m_soulAnimator.Play("Take Damage");
            }
        }

        /// <summary>
        /// Soul����
        /// </summary>
        public override void Dead()
        {
            EventManager.TriggerEvent("OnEnemyDie");
            m_soulAnimator.Play("Die");
        }

        /// <summary>
        /// Soul����
        /// </summary>
        public override void FaceDirection()
        {
            Vector3 target = new Vector3(m_target.position.x, this.transform.position.y, m_target.position.z);
            this.transform.LookAt(target);
        }

        /// <summary>
        /// �����������¼�
        /// </summary>
        private void AddAnimatorClipEvent()
        {
            AnimationClip[] clips = m_soulAnimator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                //�ҵ���Ӧ�Ķ���������
                if (clip.name.Equals("Cast Spell"))
                {
                    //�������¼�
                    AnimationEvent m_castSpellEvent = new AnimationEvent();
                    //��Ӧ�¼�������Ӧ����������
                    m_castSpellEvent.functionName = "OnCastSpellEvent";
                    //�趨��Ӧ�¼�����Ӧ�����ϵĴ���ʱ���
                    m_castSpellEvent.time = clip.length * 0.55f;
                    clip.AddEvent(m_castSpellEvent);
                }
                else if(clip.name.Equals("Projectile Attack"))
                {
                    AnimationEvent m_attackEvent = new AnimationEvent();
                    m_attackEvent.functionName = "OnAttackEvent";
                    m_attackEvent.time = clip.length * 0.4f;
                    clip.AddEvent(m_attackEvent);
                }
                else if(clip.name.Equals("Bite Attack"))
                {
                    AnimationEvent m_attackEvent = new AnimationEvent();
                    m_attackEvent.functionName = "OnBiteAttackEvent";
                    m_attackEvent.time = clip.length * 0.3f;
                    clip.AddEvent(m_attackEvent);
                }
                else if (clip.name.Equals("Die"))
                {
                    AnimationEvent m_dieEvent = new AnimationEvent();
                    m_dieEvent.functionName = "OnDieEvent";
                    m_dieEvent.time = clip.length * 0.95f;
                    clip.AddEvent(m_dieEvent);
                }
            }
            //���°󶨶����������ж���������
            m_soulAnimator.Rebind();
        }

        /// <summary>
        /// Soul�ͷż���
        /// </summary>
        protected void CastSpell()
        {
            if (m_castSpellTrigger == false)
            {
                Vector3 target = new Vector3(Random.Range(m_roomPosition.x - 5f, m_roomPosition.x + 5f), this.transform.position.y,
                        Random.Range(m_roomPosition.z - 5f, m_roomPosition.z + 5f));
                this.transform.position = target;
                m_soulAnimator.Play("Cast Spell");
                FaceDirection();
                m_castSpellTrigger = true;

                //�ͷż��ܺ�һ��ʱ�����޷����й���
                m_attackTrigger = true;
                m_attackTimer = Random.Range(1f, m_attackInverval);
            }
        }

        /// <summary>
        /// �ͷż��ܶ��������¼�
        /// </summary>
        protected void OnCastSpellEvent()
        {
            //Vector3 target = new Vector3(Random.Range(m_roomPosition.x - 5f, m_roomPosition.x + 5f), this.transform.position.y,
            //        Random.Range(m_roomPosition.z - 5f, m_roomPosition.z + 5f));
            //this.transform.position = target;
            //Debug.Log("AIĿ��λ��"+target + "---" + this.transform.position);
            //for (int i = 0; i < 12; i++)
            //{
            //    Vector3 shootDirection = new
            //    GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(this.transform.position.x, transform.position.y + 1, this.transform.position.z),
            //        Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject, 0.3f, 0.3f, 0.3f);
            //    Quaternion bulletRotation = Quaternion.Euler(0f, i * 30f, 0f);
            //    Vector3 bulletDirection = bulletRotation * this.transform.forward;
            //    EventManager.TriggerEvent<Vector3, float, float, Object>("ShootBullet", bulletDirection, 1.5f, m_attack, bullet);
            //}
            for (int i = 0; i < 24; i++)
            {
                //Vector3 shootDirection=new 
                //GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(this.transform.position.x, transform.position.y + 1, this.transform.position.z),
                //    Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject, 0.3f, 0.3f, 0.3f);
                GameObject bullet = Instantiate(m_bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                bullet.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                Quaternion bulletRotation = Quaternion.Euler(0f, i * 15f, 0f);
                Vector3 bulletDirection = bulletRotation * this.transform.forward;
                EventManager.TriggerEvent<Vector3, float, float, Object>("ShootBullet", bulletDirection, 0.6f, m_attack, bullet);
            }
        }

        /// <summary>
        /// Soul���������������¼�
        /// </summary>
        protected void OnDieEvent()
        {
            //GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
            Destroy(this.gameObject);
        }


        /// <summary>
        /// Soul����
        /// </summary>
        public override void Attack()
        {
            if (m_attackTrigger == false)
            {
                FaceDirection();
                m_soulAnimator.Play("Projectile Attack");
                m_attackTrigger = true;

                //������0.5s���޷��ͷż���
                if (m_castSpellTrigger == false)
                {
                    m_castSpellTrigger = true;
                    m_castSpellTimer = 0.5f;
                }
                else
                {
                    if (m_castSpellTimer < 0.5f)
                    {
                        m_castSpellTimer = 0.5f;
                    }
                }
            }
        }

        /// <summary>
        /// Զ�̹������������¼�
        /// </summary>
        protected void OnAttackEvent()
        {
            //m_attackTrigger = true;
            GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), 
                Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject,0.3f, 0.3f, 0.3f);
            Debug.Log("�������" + m_target.position+"��ɫλ��"+m_target.position);
            Vector3 shootDirection = new Vector3(m_target.position.x - this.transform.position.x, 0f, m_target.position.z - this.transform.position.z);
            EventManager.TriggerEvent<Vector3, float, float, Object>("ShootBullet", shootDirection, 2f, m_attack, bullet);
        }

        /// <summary>
        /// Soulǰҧ����
        /// </summary>
        protected void BiteAttack()
        {
            float radius = this.GetComponent<CapsuleCollider>().radius;

            float step = 0.1f;
            float originX = this.transform.position.x - radius;

            while (originX < this.transform.position.x + radius)
            {
                Vector3 origin = new Vector3(originX, this.transform.position.y, this.transform.position.z);
                Vector3 direction = this.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction, out hitInfo, radius))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    switch (hitObject.tag)
                    {
                        case "Player":
                            Debug.Log("���߻��������壺" + hitObject.name);
                            EventManager.TriggerEvent<float, Object>("Wound", m_attack,hitInfo.transform.gameObject);
                            break;
                        default:
                            break;
                    }
                }
                originX += step;
            }
        }

        /// <summary>
        /// ����������ȴ
        /// </summary>
        /// <returns></returns>
        protected void CoolDownTimer(float interval,ref float timer, ref bool trigger)
        {
            //Debug.Log(trigger + "----" + timer);
            if (trigger == false)
            {
                return;
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    trigger = false;
                    timer = interval;
                }
            }
        }

        /// <summary>
        /// Soul�ƶ���Ŀ�ĵ�
        /// </summary>
        protected Vector3 GetDestination()
        {
            Vector3 destination = Vector3.zero;
            RaycastHit roomPoint;

            if(Physics.Raycast(this.transform.position,Vector3.down,out roomPoint))
            {
                //�ڲ���������ķ�Χ�ڡ���һ����Χ�ڽ����ƶ�
                float x = Random.Range(Mathf.Max(this.transform.position.x - 4.5f, roomPoint.transform.position.x - 7f),
                    Mathf.Min(this.transform.position.x + 4.5f, roomPoint.transform.position.x + 7f));
                float z = Random.Range(Mathf.Max(this.transform.position.z - 4.5f, roomPoint.transform.position.z - 7f),
                    Mathf.Min(this.transform.position.z + 4.5f, roomPoint.transform.position.z + 7f));
                destination = new Vector3(x, this.transform.position.y, z);
            }
            return destination;
        }

        /// <summary>
        /// ��ȡ���ڷ��������
        /// </summary>
        private void GetRoomInfo()
        {
            RaycastHit hitInfo;
            Vector3 origin = this.transform.position;
            if (Physics.Raycast(origin, Vector3.down, out hitInfo))
            {
                m_roomPosition = hitInfo.transform.position;
                Debug.Log("Soul���µذ������" + m_roomPosition);
            }
        }

        #region �ڲ�����Ϊ�ڵ���

        /// <summary>
        /// �ͷż��ܵ���Ϊ�ڵ�
        /// </summary>
        public class SkillNode : Node
        {
            public delegate void CastSpell();
            public CastSpell m_func;

            public SkillNode(params CastSpell[] skillsFunc)
            {
                foreach(var item in skillsFunc)
                {
                    m_func += item;
                }
            }

            protected override Status OnEvaluate(Transform agent, Blackboard blackboard)
            {
                //Debug.Log("�ż���");
                bool attackTrigger = blackboard.GetDate<bool>("CastSpellTrigger");
                if (attackTrigger == true)
                {
                    return Status.Failure;
                }
                if (m_func == null)
                {
                    return Status.Failure;
                }
                m_func?.Invoke();
                return Status.Success;
            }
        }

        /// <summary>
        /// ������Ϊ�ڵ�
        /// </summary>
        public class AttackNode : Node
        {
            private ICommand m_atk;
            public AttackNode()
            {
                m_atk = CommandFactory.CreateCommand("Attack");
            }

            protected override Status OnEvaluate(Transform agent, Blackboard blackboard)
            {
                //Debug.Log("�չ�");
                bool attackTrigger = blackboard.GetDate<bool>("AttackTrigger");
                if (attackTrigger == true)
                {
                    return Status.Failure;
                }
                Soul soul = agent.gameObject.GetComponent<Soul>();
                if (soul == null)
                {
                    return Status.Failure;
                }
                m_atk.Execute(soul);
                return Status.Success;
            }
        }

        /// <summary>
        /// ��������Ϊ�ڵ�
        /// </summary>
        public class IdleNode : Node
        {
            public delegate void CastSpell();
            public CastSpell m_func;

            public IdleNode(params CastSpell[] idleFunc)
            {
                foreach (var item in idleFunc)
                {
                    m_func += item;
                }
            }

            protected override Status OnEvaluate(Transform agent, Blackboard blackboard)
            {
                if (m_func == null)
                {
                    return Status.Failure;
                }
                m_func?.Invoke();
                //Debug.Log("վ��");
                return Status.Running;
            }
        }
        #endregion
    }
}

