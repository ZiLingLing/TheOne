using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Roguelike
{
    public class PlayerBehaviour : BehavioursBase
    {
        private ICommand m_move;
        private ICommand m_atk;
        private ICommand m_wound;
        private ICommand m_rotate;
        private ICommand m_empty;
        private ICommand m_interActive;

        private Input m_input;

        /// <summary>
        /// �Ƿ��ڹ��������
        /// </summary>
        private bool m_attackTrigger = false;

        /// <summary>
        /// �Ƿ��Ѿ����빥��������
        /// </summary>
        private float m_judgeAttack;

        /// <summary>
        /// ��ɫ�ƶ�����
        /// </summary>
        private Vector3 m_moveDirection;

        /// <summary>
        /// ��ɫ����
        /// </summary>
        private Vector3 m_faceDirection;
        private Vector3 localFaceDirection;

        private bool isPause = false;

        private CharacterController m_characterController;
        private Animator m_animator;
        private Collider m_collider;

        public GameObject m_bullet;

        public GameObject m_weapon;

        private bool m_switchWeaponFlag = false;
        private bool m_isGodMode = false;
        private bool m_isDestoryed = false;

        private Material m_bodyMaterial;
        private Material m_hatMaterial;

        #region �������ں���
        private void Awake()
        {
            //DontDestroyOnLoad(this.gameObject);
            m_input = new Input();
            m_input.KeyboardAndMouse.Movement.performed += Movement_performed;
            m_input.KeyboardAndMouse.Attack.performed += Attack_performed;
            m_input.KeyboardAndMouse.InterActive.performed += InterActive_performed;

            m_characterController = this.GetComponent<CharacterController>();
            m_collider = this.GetComponent<CapsuleCollider>();
            m_animator = this.GetComponent<Animator>();

            m_hatMaterial = this.transform.Find("Player/L").Find("pSphere6").GetComponent<SkinnedMeshRenderer>().material;
            m_bodyMaterial = this.transform.Find("Player/L").Find("AM32").GetComponent<SkinnedMeshRenderer>().material;

            //EventManager.AddEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);
            AddAnimatorClipEvent();
        }

        private void OnEnable()
        {
            m_input.KeyboardAndMouse.Enable();
            EventManager.AddEventListener<float, Object>("Wound", Wound);
            EventManager.AddEventListener("Pause", Pause);
            EventManager.AddEventListener("Continue", Continue);
            StartCoroutine(AttackIntervalCoroutine(m_attackInverval));

            m_isDestoryed = false;
        }

        private void Start()
        {
            m_move = CommandFactory.CreateCommand("Move");
            m_rotate = CommandFactory.CreateCommand("Aim");
            m_empty = CommandFactory.CreateCommand("Empty");
            m_atk = CommandFactory.CreateCommand("Attack");
            m_interActive = CommandFactory.CreateCommand("InterActive");

            //m_characterController = this.GetComponent<CharacterController>();
            //m_collider = this.GetComponent<CapsuleCollider>();
            //m_animator = this.GetComponent<Animator>();

            //AddAnimatorClipEvent();
        }

        private void Update()
        {
            if (isPause == false)
            {
                m_rotate.Execute(this);
                m_move.Execute(this);
                GetSingleChoiceCommand().Execute(this);
            }
        }

        private void OnDisable()
        {
            Debug.Log("�����л�");

            EventManager.RemoveEventListener<float, Object>("Wound", Wound);
            EventManager.RemoveEventListener("Pause", Pause);
            EventManager.RemoveEventListener("Continue", Continue);
            m_input.KeyboardAndMouse.Disable();
            StopCoroutine(AttackIntervalCoroutine(m_attackInverval));

            //if (m_isDestoryed == false)
            //{
            //    Destroy(this.gameObject);
            //}
        }

        private void OnDestroy()
        {
            //EventManager.RemoveEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);
            m_input.Disable();
        }
        #endregion

        #region ���ݽ�ɫ��Ϊ��ط���

        #region InputSystem���
        private void Attack_performed(InputAction.CallbackContext obj)
        {
            Debug.Log("���յ�������Ϣ");
            if (isPause == false)
            {
                m_judgeAttack = obj.ReadValue<float>();
                
            }
        }

        private void Movement_performed(InputAction.CallbackContext obj)
        {
            //m_atk.Execute(this);
            Debug.Log("���յ��ƶ���Ϣ"+ SceneManager.GetActiveScene().name+"��ȡ�������");
            m_moveDirection = obj.ReadValue<Vector3>();

        }

        private void InterActive_performed(InputAction.CallbackContext obj)
        {
            //InterActive();
            m_interActive.Execute(this);
        }
        #endregion

        private void Pause()
        {
            isPause = true;
        }

        private void Continue()
        {
            isPause = false;
        }

        /// <summary>
        /// ��ɫ�泯���λ��
        /// </summary>
        public override void FaceDirection()
        {
            // ��ȡ���λ��
            Vector3 mousePosition = Mouse.current.position.ReadValue();

            // ����Ļ����ת��Ϊ��������
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // �����ɫӦ�ó���ķ���
                //Vector3 lookDirection = hit.point - this.gameObject.transform.position;
                //lookDirection.y = 0f; // ȷ��ֻ��ˮƽ��������ת
                //Debug.Log(hit.transform.gameObject.name);
                m_faceDirection = (hit.point - this.gameObject.transform.position).normalized;
                m_faceDirection.y = 0f;
                //Debug.Log("��ɫ����"+m_faceDirection);
                // ����ɫ�������λ��
                this.gameObject.transform.rotation = Quaternion.LookRotation(m_faceDirection);
            }
        }

        /// <summary>
        /// ��ҲٿصĽ�ɫ�ƶ�
        /// </summary>
        public override void Move()
        {
            //float m_speed = 3f;
            //Vector3 moveDirection = new Vector3(m_faceDirection.x, 0f, m_faceDirection.z);
            Vector3 direction = this.transform.InverseTransformVector(m_moveDirection);
            m_animator.SetFloat("Vertical Speed", direction.z * m_speed, 0.1f, Time.deltaTime);
            m_animator.SetFloat("Horizontal Speed", direction.x * m_speed, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// ��ɫ����
        /// </summary>
        public override void Attack()
        {
            if(m_attackTrigger==false)
            {
                m_attackTrigger = true;
                //GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject,
                //    0.3f, 0.3f, 0.3f);
                GameObject bullet = Instantiate(m_bullet, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
                bullet.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                //GameObject bullet = Instantiate(m_bullet, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
                Debug.Log("�������" + m_faceDirection);
                EventManager.TriggerEvent<Vector3, float, float, Object>("ShootBullet", m_faceDirection, 4.5f, m_attack, bullet);

            }
        }

        /// <summary>
        /// ��ɫ����
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="woundCharacter"></param>
        public override void Wound(float hit, Object info)
        {
            //Debug.Log("��Ѫ:"+hit+"�Ƿ��Ǹ�����"+(info==this.gameObject));
            if (info == this.gameObject)
            {
                if (m_isGodMode == false)
                {
                    m_isGodMode = true;
                    m_currentHp -= hit;
                    EventManager.TriggerEvent<float>("UpdateCurrentHealth", hit);
                    Debug.Log("���ǵ�ǰѪ����" + m_currentHp);
                    if (m_currentHp <= 0)
                    {
                        this.enabled = false;
                        //this.tag = "Untagged";
                        Dead();
                        return;
                    }
                    StartCoroutine(GodModeCoroutine());
                    EventManager.TriggerEvent("WoundShakeCamera");
                }

            }

        }

        /// <summary>
        /// ��ɫ����
        /// </summary>
        public override void Dead()
        {
            if (m_isDestoryed == false)
            {
                m_isDestoryed = true;
                m_animator.Play("Die");
                UIManager.Show<View>("UIDie");
                AudioManager.PlayBackgroundMusic(null);
                this.enabled = false;
            }
            //Debug.Log("��������");

        }

        #region ���������¼�ʧ�ܣ������ʲôԭ��
        /// <summary>
        /// �����������¼�
        /// </summary>
        protected void AddAnimatorClipEvent()
        {
            AnimationClip[] clips = m_animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                //�ҵ���Ӧ�Ķ���������
                if (clip.name.Equals("Die"))
                {
                    Debug.Log("�ɹ��������������¼�");
                    AnimationEvent m_dieEvent = new AnimationEvent();
                    m_dieEvent.functionName = "OnDieEvent";
                    m_dieEvent.time = clip.length * 0.05f;
                    clip.AddEvent(m_dieEvent);
                }
            }
            //���°󶨶����������ж���������
            m_animator.Rebind();
        }

        /// <summary>
        /// �������������¼�
        /// </summary>
        protected void OnDieEvent()
        {
            Debug.Log("�����������¼��Ƿ�ִ��");
            Debug.Log("ִ�н��" + this.enabled);
            Destroy(this.gameObject);
        }
        #endregion

        /// <summary>
        /// ��ɫ������Ϊ
        /// </summary>
        public override void InterActive()
        {
            float rayLength = 3f;
            int rayCount = 12;
            float angleStep = 15f;

            Vector3 origin = new Vector3(this.transform.position.x, 1f, this.transform.position.z);

            for (int i = 0; i < rayCount; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 direction = rotation * transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction, out hitInfo, rayLength))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    //Debug.Log(hitObject.name);
                    switch (hitObject.tag)
                    {
                        case "Weapon":
                            if (m_weapon == null || hitObject.name != m_weapon.name)
                            {
                                if (m_weapon != null)
                                {
                                    EventManager.TriggerEvent<Transform>("SwitchState", m_weapon.transform);
                                }
                                m_weapon = hitObject;
                                EventManager.TriggerEvent<Transform>("SwitchState", m_weapon.transform);
                            }
                            break;
                        default:
                            break;
                    }
                    //EventManager.TriggerEvent<>
                }
            }
        }


        /// <summary>
        /// ����һ����Ҫ�����������б�Ĳ�����һ��һ֡�ڻ���Ĳ���
        /// </summary>
        /// <returns></returns>
        public ICommand GetSingleChoiceCommand()
        {
            ICommand command = m_empty;
            if(m_judgeAttack==1)
            {
                command = m_atk;
            }
            return command;
        }

        /// <summary>
        /// ��ȡplayer��Transform���
        /// </summary>
        /// <returns></returns>
        private Transform GetPlayerTransform()
        {
            return this.transform;
        }

        #endregion

        #region Э��
        /// <summary>
        /// �жϿ�����Э��
        /// </summary>
        /// <param name="fireInterval"></param>
        /// <returns></returns>
        private IEnumerator AttackIntervalCoroutine(float fireInterval)
        {
            while(true)
            {
                if (m_attackTrigger == true)
                {
                    yield return new WaitForSeconds(fireInterval);
                    m_attackTrigger = false;
                }
                yield return null;
            }
        }

        /// <summary>
        /// �ж��޵�ʱ���Э��
        /// </summary>
        /// <returns></returns>
        private IEnumerator GodModeCoroutine()
        {
            StartCoroutine(BlinkCoroutine());
            yield return new WaitForSeconds(2f);
            m_isGodMode = false;
            yield return null;
        }

        /// <summary>
        /// ��˸Ч����Э��
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlinkCoroutine()
        {
            Color hatColor = m_hatMaterial.color;
            Color bodyColor = m_bodyMaterial.color;
            float hatStartAlpha = hatColor.a;
            float bodyStartAlpha = bodyColor.a;
            float hatCurrentAlpha = hatStartAlpha;
            float bodyCurrentAlpha = hatStartAlpha;
            bool addAlpha = false;
            while (m_isGodMode == true)
            {
                if (addAlpha == false)
                {
                    hatCurrentAlpha -= 5;
                    bodyCurrentAlpha -= 5;
                    addAlpha = true;
                }
                else
                {
                    hatCurrentAlpha += 5;
                    bodyCurrentAlpha += 5;
                    addAlpha = false;
                }
                //hatColor = m_hatMaterial.color;
                //bodyColor = m_bodyMaterial.color;
                hatColor.a = hatCurrentAlpha;
                bodyColor.a = bodyCurrentAlpha;
                m_hatMaterial.color = hatColor;
                m_bodyMaterial.color = bodyColor;
                Debug.Log(hatColor.a);

                yield return new WaitForSeconds(0.05f);
            }

            hatColor.a = hatStartAlpha;
            bodyColor.a = bodyStartAlpha;
            m_hatMaterial.color = hatColor;
            m_bodyMaterial.color = bodyColor;
            yield return null;
        }
        #endregion
    }
}
