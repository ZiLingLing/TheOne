using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    public class UIMain : View
    {
        private Button m_startButtom;
        private Button m_quitButton;

        private Input m_input;

        private bool m_waitSceneLoaded = false;

        private GameObject m_player;

        private void Awake()
        {
            m_input = new Input();

            Init();
        }

        private void OnEnable()
        {
            m_input.Enable();
            m_waitSceneLoaded = false;
            //m_player = EventManager.TriggerEvent<Transform>("GetPlayerTransform").gameObject;
    }

        private void OnDisable()
        {
            m_input.Disable();
        }

        public override void Init()
        {

                m_startButtom = this.transform.Find("Start").GetComponent<Button>();
                m_quitButton = this.transform.Find("Exit").GetComponent<Button>();

                m_startButtom.onClick.AddListener(StartGame);
                m_quitButton.onClick.AddListener(Quit);

        }

        /// <summary>
        /// Start��ť
        /// </summary>
        private void StartGame()
        {
            if (m_waitSceneLoaded == false)
            {
                m_waitSceneLoaded = true;

                AudioManager.PlayBackgroundMusic("WaitScene");
                //SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene("WaitScene", LoadSceneMode.Additive);
                //m_player.transform.position = new Vector3(219.47f, 32f, 220.64f);
                SceneManager.UnloadSceneAsync("Start");
                UIManager.Close<UIMain>();
                //Debug.Log("��ɫ��ǰ��λ��" + m_player.transform.position);
            }
        }

        //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    if (scene.name == "WaitScene")
        //    {
        //        m_player.transform.position = new Vector3(219.47f, 32f, 220.64f);
        //        Debug.Log("��ɫ��ǰ��λ��" + m_player.transform.position);
        //        SceneManager.sceneLoaded -= OnSceneLoaded; // ȡ���¼�����
        //    }
        //}

        /// <summary>
        /// Exit��ť
        /// </summary>
        private void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
