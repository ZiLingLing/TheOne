using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Roguelike
{

    public class ScenesManager : MonoBehaviour
    {
        private static ScenesManager m_instance;//˽�е���

        private Action m_onSceneLoaded = null;//����������ɻص�

        private string m_strNextSceneName = null;//��Ҫ���صĳ�����
        private string m_strCurSceneName = null;//��ǰ������������û�г�������Ĭ�Ϸ��� Login
        private string m_strPreSceneName = null;//��һ��������

        private bool m_bLoading = false;//�Ƿ����ڼ�����

        private bool m_bDestroyAuto = true;//�Զ�ɾ�� loading ����

        private const string m_strLoadSceneName = "LoadingScene";//���س�������

        private GameObject m_objLoadProgress = null;//���ؽ�����ʾ����

        //��ȡ��ǰ������
        public static string s_strLoadedSceneName => m_instance.m_strCurSceneName;

        public static void CreateInstance(GameObject go)
        {
            if (null != m_instance)
            {
                return;
            }
            m_instance = go.AddComponent<ScenesManager>();
            DontDestroyOnLoad(m_instance);
            m_instance.m_strCurSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        public static void LoadPreScene()
        {
            if (string.IsNullOrEmpty(m_instance.m_strPreSceneName))
            {
                return;
            }
            LoadScene(m_instance.m_strPreSceneName);
        }

        public static void LoadScene(string strLevelName)
        {
            m_instance.LoadLevel(strLevelName, null);
        }

        public static void LoadScene(string strLevelName, Action onSecenLoaded)
        {
            m_instance.LoadLevel(strLevelName, onSecenLoaded);
        }

        private void LoadLevel(string strLevelName, Action onSecenLoaded, bool isDestroyAuto = true)
        {
            if (m_bLoading || m_strCurSceneName == strLevelName)
            {
                return;
            }

            m_bLoading = true;  // ����
                                // ��ʼ����    
            m_onSceneLoaded = onSecenLoaded;
            m_strNextSceneName = strLevelName;
            m_strPreSceneName = m_strCurSceneName;
            m_strCurSceneName = m_strLoadSceneName;
            m_bDestroyAuto = isDestroyAuto;

            //���첽���� Loading ����
            StartCoroutine(StartLoadSceneOnEditor(m_strLoadSceneName, OnLoadingSceneLoaded, null));
        }

        /// <summary>
        /// ���ɳ���������ɻص�
        /// </summary>
        private void OnLoadingSceneLoaded()
        {
            // ���ɳ���������ɺ������һ������
            StartCoroutine(StartLoadSceneOnEditor(m_strNextSceneName, OnNextSceneLoaded, OnNextSceneProgress));
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="strLevelName"></param>
        /// <param name="OnSecenLoaded"></param>
        /// <param name="OnSceneProgress"></param>
        /// <returns></returns>
        private IEnumerator StartLoadSceneOnEditor(string strLevelName, Action OnSecenLoaded, Action<float> OnSceneProgress)
        {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(strLevelName);
            if (null == async)
            {
                yield break;
            }

            //*���ؽ���
            while (!async.isDone)
            {
                float fProgressValue;
                if (async.progress < 0.9f)
                {
                    fProgressValue = async.progress;
                }
                else
                {
                    fProgressValue = 1.0f;
                }
                OnSceneProgress?.Invoke(fProgressValue);
                yield return null;
            }
            OnSecenLoaded?.Invoke();
        }

        /// <summary>
        /// ������һ������ɻص�
        /// </summary>
        private void OnNextSceneLoaded()
        {
            m_bLoading = false;
            OnNextSceneProgress(1);
            m_strCurSceneName = m_strNextSceneName;
            m_strNextSceneName = null;
            m_onSceneLoaded?.Invoke();
        }

        /// <summary>
        /// �������ؽ��ȱ仯
        /// </summary>
        /// <param name="fProgress"></param>
        private void OnNextSceneProgress(float fProgress)
        {
            if (null == m_objLoadProgress)
            {
                m_objLoadProgress = GameObject.Find("TextLoadProgress");
            }
            Text textLoadProgress = m_objLoadProgress.GetComponent<Text>();
            if (null == textLoadProgress)
            {
                return;
            }
            textLoadProgress.text = (fProgress * 100).ToString() + "%";
        }
    }
}