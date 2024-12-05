using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    /// <summary>
    /// Entry场景内的Launcher
    /// </summary>
    public class Launcher : MonoBehaviour
    {
        private GameObject m_player;
        private GameObject m_prefab;

        void Awake()
        {

            //实例化GameManager
            //加载初始场景的UI
            //生成角色
            DontDestroyOnLoad(this.gameObject);
            //实例化所有Root

            GameObject audioRoot = Instantiate(Resources.Load<GameObject>("Roots/AudioRoot"));
            audioRoot.name = audioRoot.name.Replace("(Clone)", "");
            new AudioManager();

            GameObject objPool = Instantiate(Resources.Load<GameObject>("Roots/ObjectPoolRoot"));
            objPool.name = objPool.name.Replace("(Clone)", "");


            ////加载角色
            //m_prefab = Resources.Load<GameObject>("Character/Player");
            //m_player = Instantiate(m_prefab, Vector3.zero, Quaternion.identity);

            GameObject uiRoot = Instantiate(Resources.Load<GameObject>("Roots/UIRoot"));
            uiRoot.name = uiRoot.name.Replace("(Clone)", "");
            new UIManager();

            //开始界面的加载
            AudioManager.PlayBackgroundMusic("Entry");
            SceneManager.LoadSceneAsync("Start", LoadSceneMode.Additive);


            UIManager.Show<UIMain>();

            //m_player.SetActive(false);
            //GameObject audioRoot=Instantiate

            //var roots = Resources.LoadAll<GameObject>("Roots");//加载UI文件夹下的所有UI预制体
            //foreach (GameObject root in roots)
            //{
            //    GameObject prefab = Instantiate(Resources.Load<GameObject>("Roots/"+root.name));
            //    prefab.name = prefab.name.Replace("(Clone)", "");
            //}


        }


    }
}

