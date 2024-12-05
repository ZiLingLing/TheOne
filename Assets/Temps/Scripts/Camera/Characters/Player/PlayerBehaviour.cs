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
        /// 是否处于攻击间隔中
        /// </summary>
        private bool m_attackTrigger = false;

        /// <summary>
        /// 是否已经输入攻击的命令
        /// </summary>
        private float m_judgeAttack;

        /// <summary>
        /// 角色移动方向
        /// </summary>
        private Vector3 m_moveDirection;

        /// <summary>
        /// 角色朝向
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

        #region 生命周期函数
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
            Debug.Log("场景切换");

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

        #region 操纵角色行为相关方法

        #region InputSystem相关
        private void Attack_performed(InputAction.CallbackContext obj)
        {
            Debug.Log("接收到攻击信息");
            if (isPause == false)
            {
                m_judgeAttack = obj.ReadValue<float>();
                
            }
        }

        private void Movement_performed(InputAction.CallbackContext obj)
        {
            //m_atk.Execute(this);
            Debug.Log("接收到移动信息"+ SceneManager.GetActiveScene().name+"获取玩家输入");
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
        /// 角色面朝鼠标位置
        /// </summary>
        public override void FaceDirection()
        {
            // 获取鼠标位置
            Vector3 mousePosition = Mouse.current.position.ReadValue();

            // 将屏幕坐标转换为世界坐标
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 计算角色应该朝向的方向
                //Vector3 lookDirection = hit.point - this.gameObject.transform.position;
                //lookDirection.y = 0f; // 确保只在水平方向上旋转
                //Debug.Log(hit.transform.gameObject.name);
                m_faceDirection = (hit.point - this.gameObject.transform.position).normalized;
                m_faceDirection.y = 0f;
                //Debug.Log("角色朝向"+m_faceDirection);
                // 将角色朝向鼠标位置
                this.gameObject.transform.rotation = Quaternion.LookRotation(m_faceDirection);
            }
        }

        /// <summary>
        /// 玩家操控的角色移动
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
        /// 角色攻击
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
                Debug.Log("射击方向" + m_faceDirection);
                EventManager.TriggerEvent<Vector3, float, float, Object>("ShootBullet", m_faceDirection, 4.5f, m_attack, bullet);

            }
        }

        /// <summary>
        /// 角色受伤
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="woundCharacter"></param>
        public override void Wound(float hit, Object info)
        {
            //Debug.Log("扣血:"+hit+"是否是该物体"+(info==this.gameObject));
            if (info == this.gameObject)
            {
                if (m_isGodMode == false)
                {
                    m_isGodMode = true;
                    m_currentHp -= hit;
                    EventManager.TriggerEvent<float>("UpdateCurrentHealth", hit);
                    Debug.Log("主角当前血量：" + m_currentHp);
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
        /// 角色死亡
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
            //Debug.Log("死亡动画");

        }

        #region 给动画绑定事件失败，不清楚什么原因
        /// <summary>
        /// 给动画加入事件
        /// </summary>
        protected void AddAnimatorClipEvent()
        {
            AnimationClip[] clips = m_animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                //找到对应的动画的名称
                if (clip.name.Equals("Die"))
                {
                    Debug.Log("成功插入死亡动画事件");
                    AnimationEvent m_dieEvent = new AnimationEvent();
                    m_dieEvent.functionName = "OnDieEvent";
                    m_dieEvent.time = clip.length * 0.05f;
                    clip.AddEvent(m_dieEvent);
                }
            }
            //重新绑定动画器的所有动画的属性
            m_animator.Rebind();
        }

        /// <summary>
        /// 死亡动画插入事件
        /// </summary>
        protected void OnDieEvent()
        {
            Debug.Log("死亡动画的事件是否执行");
            Debug.Log("执行结果" + this.enabled);
            Destroy(this.gameObject);
        }
        #endregion

        /// <summary>
        /// 角色交互行为
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
        /// 返回一个将要被加入输入列表的操作，一个一帧内互斥的操作
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
        /// 获取player的Transform组件
        /// </summary>
        /// <returns></returns>
        private Transform GetPlayerTransform()
        {
            return this.transform;
        }

        #endregion

        #region 协程
        /// <summary>
        /// 判断开火间隔协程
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
        /// 判断无敌时间的协程
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
        /// 闪烁效果的协程
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
