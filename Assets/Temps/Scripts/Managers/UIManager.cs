using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class UIManager
    {
        public static Dictionary<string, View> s_viewsDic = new Dictionary<string, View>();//��������UIԤ����
        public UIManager()
        {
            s_viewsDic = UIRoot.m_views;//��ֵ
            Initilization();
        }

        /// <summary>
        /// ��ʼ����ֵ
        /// </summary>
        private static void Initilization()
        {
            foreach (var view in s_viewsDic)
            {
                Debug.Log(view.Value == null );
                view.Value.Init();
                view.Value.Hide();
            }
            //Show<UIMain>();
        }

        /// <summary>
        /// ��ȡ��ǰUI��ͼ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetView<T>() where T : View
        {
            foreach (var itemView in s_viewsDic)
            {
                if (itemView.Value is T tView)
                {
                    return tView;
                }
            }
            return null;
        }

        /// <summary>
        /// ��ʾ����UI��ͼ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Show<T>() where T : View
        {
            foreach (var itemView in s_viewsDic)
            {
                if (itemView.Value is T)
                {
                    itemView.Value.Show();
                }
            }
        }

        /// <summary>
        /// ��ʾĳһ��UI��ͼ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public static void Show<T>(string name) where T : View
        {
            if (s_viewsDic.ContainsKey(name))
            {
                View itemView = s_viewsDic[name];
                itemView.Show();
            }
        }
        /// <summary>
        /// �ر�UI��ͼ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Close<T>() where T : View
        {
            foreach (var itemView in s_viewsDic)
            {
                if (itemView.Value is T)
                {
                    itemView.Value.Hide();
                }
            }
        }
    }
}

