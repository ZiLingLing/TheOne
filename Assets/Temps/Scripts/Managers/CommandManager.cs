using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// ȫ�ִ���һ������������е�CommandManager
    /// </summary>
    public class CommandManager
    {
        private static CommandManager m_instance;

        private Deque<ICommand> m_commandBuffer = new Deque<ICommand>();

        public static CommandManager Instance
        {
            get
            {
                if(m_instance==null)
                {
                    m_instance = new CommandManager();
                }
                return m_instance;
            }
        }

        /// <summary>
        /// ��������������
        /// </summary>
        /// <param name="command"></param>
        public void AddCommands(ICommand command)
        {
            m_commandBuffer.BackEnqueue(command);
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void UndoCommands()
        {
            while(m_commandBuffer.Count!=0)
            {
                m_commandBuffer.BackDequeue();
            }
        }
    }

    /// <summary>
    /// ˫�����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Deque<T>
    {
        private LinkedList<T> m_linkList;

        /// <summary>
        /// ������Ԫ�ص�����
        /// </summary>
        private int m_count;

        /// <summary>
        /// ��ȡ������Ԫ�ص�����
        /// </summary>
        public int Count
        {
            get { m_count = m_linkList.Count; return m_count; }
        }

        public Deque()
        {
            m_linkList = new LinkedList<T>();
        }

        /// <summary>
        /// �Ӷ������
        /// </summary>
        /// <param name="t"></param>
        public void FrontEnqueue(T t)
        {
            m_linkList.AddFirst(t);
        }

        /// <summary>
        /// �Ӷ��׳���
        /// </summary>
        /// <returns></returns>
        public void FrontDequeue()
        {
            if(m_linkList.Count!=0)
            {
                m_linkList.RemoveFirst();
            }
        }

        /// <summary>
        /// �Ӷ�β���
        /// </summary>
        /// <param name="t"></param>
        public void BackEnqueue(T t)
        {
            m_linkList.AddLast(t);
        }

        /// <summary>
        /// �Ӷ�β����
        /// </summary>
        /// <returns></returns>
        public void BackDequeue()
        {
            if (m_linkList.Count != 0)
            {
                m_linkList.RemoveLast();
            }
        }

        /// <summary>
        /// ��ն���
        /// </summary>
        public void Clear()
        {
            m_linkList.Clear();
        }
    }
}


