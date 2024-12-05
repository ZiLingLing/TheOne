using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    public class UIDie : View
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
            m_win = this.transform.Find("DieImage").GetComponent<Button>();

            m_win.onClick.AddListener(Die);
        }


        /// <summary>
        /// 点击Continue按钮事件
        /// </summary>
        private void Die()
        {
            if (m_isWaitSceneLoaded == false)
            {
                m_isWaitSceneLoaded = true;
                UIManager.Close<UIDie>();
                UIManager.Close<UIBattle>();
                SceneManager.LoadSceneAsync("WaitScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("BattleScene");
                AudioManager.PlayBackgroundMusic("WaitScene");
            }

        }
    }

}
