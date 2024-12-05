using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class PortalInEndRoom : MonoBehaviour
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

                AudioManager.PlayBackgroundMusic(null);
                UIManager.Show<View>("UIWin");
            }
        }
    }

}
