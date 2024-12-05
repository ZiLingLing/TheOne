using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    public class UIWin : View
    {
        private Button m_win;

        private bool m_isWaitSceneLoaded = false;

        private void Awake()
        {
            Init();

        }

        private void OnEnable()
        {
            m_isWaitSceneLoaded = false;
        }

        public override void Init()
        {
            m_win = this.transform.Find("WinImage").GetComponent<Button>();

            m_win.onClick.AddListener(Win);
        }


        /// <summary>
        /// 点击Continue按钮事件
        /// </summary>
        private void Win()
        {
            if (m_isWaitSceneLoaded == false)
            {
                m_isWaitSceneLoaded = true;
                UIManager.Show<UIMain>();
                UIManager.Close<UIWin>();
                UIManager.Close<UIBattle>();
                SceneManager.LoadSceneAsync("WaitScene", LoadSceneMode.Additive);
                //SceneManager.LoadSceneAsync("Start", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("BattleScene");
                AudioManager.PlayBackgroundMusic("WaitScene");
                //AudioManager.PlayBackgroundMusic("Entry");
            }

        }

    }
}
