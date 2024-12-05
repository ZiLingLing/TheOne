using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    public class UIPause : View
    {
        private Button m_settingButton;
        private Button m_continueButton;
        private Button m_exitButton;

        private Input m_input;
        public Animator m_pauseAnimator;

        private bool m_isFreeze = false;

        private bool m_startSceneLoaded = false;

        private Transform m_player;

        private void Awake()
        {
            m_pauseAnimator = this.GetComponent<Animator>();

            m_input = new Input();
            m_input.KeyboardAndMouse.Pause.started += Pause_started;
            
            Init();

        }

        private void OnEnable()
        {
            m_input.Enable();
            m_pauseAnimator.Play("UIPauseShow");
            EventManager.AddEventListener("UIPauseUnlock", Unlock);

            m_player = EventManager.TriggerEvent<Transform>("GetPlayerTransform");
            Debug.Log("UIPause�л�ȡ��ɫTransform�Ƿ�Ϊ�գ�" + m_player);
            if (m_player != null)
            {
                m_player.GetComponent<PlayerBehaviour>().enabled = false;
            }
        }

        private void OnDisable()
        {
            m_input.Disable();
            EventManager.RemoveEventListener("UIPauseUnlock", Unlock);
        }

        private void Pause_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (m_input.KeyboardAndMouse.Pause.IsPressed() && m_isFreeze == false)
            {
                Continue();
            }
        }

        public override void Init()
        {
            m_settingButton = this.transform.Find("Setting").GetComponent<Button>();
            m_continueButton = this.transform.Find("Continue").GetComponent<Button>();
            m_exitButton = this.transform.Find("Exit").GetComponent<Button>();

            m_continueButton.onClick.AddListener(Continue);
            m_settingButton.onClick.AddListener(Setting);
            m_exitButton.onClick.AddListener(Exit);

            AddAnimatorClipEvent();
            m_isFreeze = false;
        }

        /// <summary>
        /// ������ʾ�����ϵ��¼�
        /// </summary>
        private void OnPanelShowEvent()
        {
            m_isFreeze = false;
            Time.timeScale = 0;
        }

        /// <summary>
        /// ���ڹرն����ϵ��¼�
        /// </summary>
        private void OnPanelCloseEvent()
        {
            this.Hide();
            m_isFreeze = true;



            EventManager.TriggerEvent("Continue");
            EventManager.TriggerEvent("UIBattleUnlock");
        }

        /// <summary>
        /// ���Continue��ť�¼�
        /// </summary>
        private void Continue()
        {
            if (m_player != null)
            {
                m_player.GetComponent<PlayerBehaviour>().enabled = true;
            }

            Time.timeScale = 1;
            m_pauseAnimator.Play("UIPauseClose");
            Debug.Log("Continue");
        }

        /// <summary>
        /// ���Setting��ť�¼�
        /// </summary>
        private void Setting()
        {
            m_isFreeze = true;
            UIManager.Show<View>("UISetting");
        }

        /// <summary>
        /// ���Exit��ť�¼�
        /// </summary>
        private void Exit()
        {
            if (m_startSceneLoaded == false)
            {

                m_startSceneLoaded = true;

                SceneManager.LoadSceneAsync("Start", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("BattleScene");

                AudioManager.PlayBackgroundMusic("Entry");

                UIManager.Show<UIMain>();
                UIManager.Close<UIBattle>();

                Time.timeScale = 1;
                UIManager.Close<UIPause>();

                //StartCoroutine(SwitchScene());

            }

        }

        private void AddAnimatorClipEvent()
        {
            AnimationClip[] clips = m_pauseAnimator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                //�ҵ���Ӧ�Ķ���������
                if (clip.name.Equals("UIPauseShow"))
                {
                    //�������¼�
                    AnimationEvent m_castSpellEvent = new AnimationEvent();
                    //��Ӧ�¼�������Ӧ����������
                    m_castSpellEvent.functionName = "OnPanelShowEvent";
                    //�趨��Ӧ�¼�����Ӧ�����ϵĴ���ʱ���
                    m_castSpellEvent.time = clip.length;
                    clip.AddEvent(m_castSpellEvent);
                }
                else if(clip.name.Equals("UIPauseClose"))
                {
                    AnimationEvent m_castSpellEvent = new AnimationEvent();
                    m_castSpellEvent.functionName = "OnPanelCloseEvent";
                    m_castSpellEvent.time = clip.length * 0.9f;
                    clip.AddEvent(m_castSpellEvent);
                }
            }
            //���°󶨶����������ж���������
            m_pauseAnimator.Rebind();
        }

        /// <summary>
        /// �ر�Setting����ʹ��Pause������
        /// </summary>
        private void Unlock()
        {
            if (m_player != null)
            {
                m_player.GetComponent<PlayerBehaviour>().enabled = true;
            }

            m_isFreeze = false;
        }

        private IEnumerator SwitchScene()
        {
            yield return new WaitForSeconds(0.5f);



        }
    }
}

