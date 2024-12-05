using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class UIRoot : MonoBehaviour
    {
        public static Dictionary<string, View> m_views = new Dictionary<string, View>(); //����һ���ֵ�洢����UI����
        private void Awake()
        {
            var prefabs = Resources.LoadAll<GameObject>("UI");//����UI�ļ����µ�����UIԤ����
            foreach (GameObject view in prefabs)
            {
                if (!m_views.ContainsKey(view.name))
                {
                    GameObject prefab = Instantiate(view,this.transform);
                    prefab.name = prefab.name.Replace("(Clone)", "");//ȥ��ʵ���������(Clone)��׺����Ԥ��������ͳһ
                    m_views.Add(prefab.name, prefab.GetComponent<View>());//�����ֵ�
                }
            }

           
        }
    }
}
