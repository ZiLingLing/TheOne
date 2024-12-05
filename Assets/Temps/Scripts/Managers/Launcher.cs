using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    /// <summary>
    /// Entry�����ڵ�Launcher
    /// </summary>
    public class Launcher : MonoBehaviour
    {
        private GameObject m_player;
        private GameObject m_prefab;

        void Awake()
        {

            //ʵ����GameManager
            //���س�ʼ������UI
            //���ɽ�ɫ
            DontDestroyOnLoad(this.gameObject);
            //ʵ��������Root

            GameObject audioRoot = Instantiate(Resources.Load<GameObject>("Roots/AudioRoot"));
            audioRoot.name = audioRoot.name.Replace("(Clone)", "");
            new AudioManager();

            GameObject objPool = Instantiate(Resources.Load<GameObject>("Roots/ObjectPoolRoot"));
            objPool.name = objPool.name.Replace("(Clone)", "");


            ////���ؽ�ɫ
            //m_prefab = Resources.Load<GameObject>("Character/Player");
            //m_player = Instantiate(m_prefab, Vector3.zero, Quaternion.identity);

            GameObject uiRoot = Instantiate(Resources.Load<GameObject>("Roots/UIRoot"));
            uiRoot.name = uiRoot.name.Replace("(Clone)", "");
            new UIManager();

            //��ʼ����ļ���
            AudioManager.PlayBackgroundMusic("Entry");
            SceneManager.LoadSceneAsync("Start", LoadSceneMode.Additive);


            UIManager.Show<UIMain>();

            //m_player.SetActive(false);
            //GameObject audioRoot=Instantiate

            //var roots = Resources.LoadAll<GameObject>("Roots");//����UI�ļ����µ�����UIԤ����
            //foreach (GameObject root in roots)
            //{
            //    GameObject prefab = Instantiate(Resources.Load<GameObject>("Roots/"+root.name));
            //    prefab.name = prefab.name.Replace("(Clone)", "");
            //}


        }


    }
}

