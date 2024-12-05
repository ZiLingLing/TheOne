using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

namespace Roguelike
{
    public class Portal : MonoBehaviour
    {
        public GameObject m_onOpenVFX;

        private void OnEnable()
        {
            m_onOpenVFX.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.tag == "Player")
            {
                m_onOpenVFX.SetActive(true);

                SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
                AudioManager.PlayBackgroundMusic("Battle");
                UIManager.Show<View>("UIBattle");
                SceneManager.UnloadSceneAsync("WaitScene");
            }
        }
    }
}

