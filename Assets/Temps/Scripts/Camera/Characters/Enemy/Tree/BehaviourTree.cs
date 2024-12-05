using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Roguelike
{
    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour
    {
        protected BehaviourTreeBaseNode m_root = null;

        //protected DataBase m_database = new DataBase();//Ϊÿ��������һ���ڰ�ڵ�

        private void Start()
        {
            m_root = SetUpTree();
        }

        private void Update()
        {
            if (m_root != null)
            {
                m_root.Execute();
            }
        }

        public abstract BehaviourTreeBaseNode SetUpTree();

    }

    /// <summary>
    /// ��Ϊ���ڵ��״̬
    /// </summary>
    public enum ENodeState
    {
        Success,
        Failure,
        Running,
    }

    /// <summary>
    /// ��Ϊ�ڵ����
    /// </summary>
    public class BehaviourTreeBaseNode
    {
        /// <summary>
        /// �ڵ�״̬
        /// </summary>
        protected ENodeState m_state;

        /// <summary>
        /// ��Ϊ���ӽڵ��б�
        /// </summary>
        protected List<BehaviourTreeBaseNode> m_childList = new List<BehaviourTreeBaseNode>();

        /// <summary>
        /// ��ǰ�ڵ�ĸ��ڵ�
        /// </summary>
        public BehaviourTreeBaseNode m_parent;

        /// <summary>
        /// ִ�е�ǰ�߼����ӽڵ����
        /// </summary>
        protected int m_nodeIndex;

        /// <summary>
        /// �洢�ڵ���Զ�������,��������и��ڵ��еĸ�����
        /// </summary>
        private Dictionary<string, object> m_dataContext = new Dictionary<string, object>();

        public BehaviourTreeBaseNode()
        {
            m_parent = null;
        }

        public BehaviourTreeBaseNode(List<BehaviourTreeBaseNode> childList)
        {
            foreach(BehaviourTreeBaseNode child in childList)
            {
                AddChild(child);
            }
        }

        /// <summary>
        /// ����ӽڵ�
        /// </summary>
        /// <param name="nodes"></param>
        public virtual void AddChild(BehaviourTreeBaseNode node)
        {
            node.m_parent = this;
            m_childList.Add(node);
        }

        /// <summary>
        /// �Ƴ��ӽڵ�
        /// </summary>
        /// <param name="index"></param>
        //public virtual void RemoveChild(int index)
        //{
        //    m_childList.Remove(m_childList[index]);
        //}

        /// <summary>
        /// ִ�нڵ��߼�
        /// </summary>
        /// <returns></returns>
        public virtual ENodeState Execute()
        {
            return ENodeState.Failure;
        }

        /// <summary>
        /// ���ö�Ӧ�����Ƶ��Զ�������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetData(string key,object value)
        {
            m_dataContext[key] = value;
        }

        /// <summary>
        /// ͨ�����ƻ�ȡ��Ӧ���Զ�������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            object value = null;
            if(m_dataContext.TryGetValue(key,out value) == true)
            {
                return value;
            }

            BehaviourTreeBaseNode node = m_parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                {
                    return value;
                }
            }
            return value;
        }

        /// <summary>
        /// ��ո�����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ClearData(string key)
        {
            if (m_dataContext.ContainsKey(key))
            {
                m_dataContext.Remove(key);
                return true;
            }

            BehaviourTreeBaseNode node = m_parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared == true)
                {
                    return true;
                }
                node = node.m_parent;
            }
            return false;
        }
    }

    #region ��Ϊ�ڵ�
    // ��Ϊ�ڵ�ͨ����Ϊ����Ҷ�ӽ�㣬������ɾ����һ����Ϊ֮����ݼ�������÷��ط���ֵ��
    // ��Ϊ�ڵ������ִ��һ�εõ����(����ʧ�ܻ�ɹ�)��Ҳ���Էֲ�ִ�кܶ�Ρ�

    /// <summary>
    /// ������Ϊ�ڵ�
    /// </summary>
    public class AttackAction : BehaviourTreeBaseNode
    {
        //private EnemyManager _enemyManager;

        private Transform _transform;

        private float _attackTime = 1f;
        private float _attackCounter = 0f;

        public AttackAction(Transform transform)
        {
            _transform = transform;
        }

        public override ENodeState Execute()
        {
            _attackCounter += Time.deltaTime;

            if (_attackCounter >= _attackTime)
            {
                Transform target = (Transform)GetData("target");
                //var enemyManager = target.GetComponent<EnemyManager>();

                bool enemyDead = true;//enemyManager.TakeHit();
                if (enemyDead)
                {
                    ClearData("target");
                }
                else
                {
                    _attackCounter = 0f;
                }
            }

            m_state = ENodeState.Running;
            return m_state;
        }
    }

    //public class BehaviourTreeActionNode : BehaviourTreeBaseNode
    //{
    //    public Func<ENodeState> m_action;

    //    private ENodeState m_nodeState;

    //    public ENodeState NodeState
    //    {
    //        get
    //        {
    //            return m_nodeState;
    //        }
    //    }

    //    public BehaviourTreeActionNode(Func<ENodeState> action)
    //    {
    //        this.m_action = action;
    //    }

    //    public override ENodeState Execute()
    //    {
    //        if (m_action == null)
    //        {
    //            m_nodeState = ENodeState.Failure;
    //            return m_nodeState;
    //        }

    //        switch (m_action.Invoke())
    //        {
    //            case ENodeState.Failure:
    //                m_nodeState = ENodeState.Failure;
    //                return ENodeState.Failure;
    //            case ENodeState.Running:
    //                m_nodeState = ENodeState.Running;
    //                return ENodeState.Running;
    //        }
    //        m_nodeState = ENodeState.Success;
    //        return m_nodeState;
    //    }
    //}
    #endregion
    /// <summary>
    /// �����ڵ㣺
    /// ���������㷵��True�����򷵻�Fasle��
    /// </summary>
    public class BehaviourTreeConditionNode : BehaviourTreeBaseNode
    {

        public Func<bool> action;

        public BehaviourTreeConditionNode(Func<bool> action)
        {
            this.action = action;
        }

        public override ENodeState Execute()
        {
            if (action == null)
            {
                return ENodeState.Failure;
            }

            return action.Invoke() ? ENodeState.Success : ENodeState.Failure;
        }

    }

    #region ѡ��ڵ�
    // ��ִ�б����ͽڵ�ʱ��������ͷ��β����ִ���Լ����ӽڵ㣬�������һ���ӽڵ�ִ�к󷵻�True����ֹͣ���������ڵ�����Լ����ϲ㸸�ڵ�Ҳ�᷵��True��
    // ���������ӽڵ㶼������Fasle����ô���ڵ�Ҳ�����Լ��ĸ��ڵ㷵��Fasle��
    // ���������е��ӽڵ㣬һ�����ӽڵ���������ִ�е���Ϊ������������ֹ����

    public class Selector : BehaviourTreeBaseNode
    {
        public Selector() : base() { }
        public Selector(List<BehaviourTreeBaseNode> m_childList) : base(m_childList) { }

        public override ENodeState Execute()
        {
            foreach (BehaviourTreeBaseNode child in m_childList)
            {
                switch (child.Execute())
                {
                    case ENodeState.Failure:
                        continue;

                    case ENodeState.Success:
                        m_state = ENodeState.Success;
                        return m_state;

                    case ENodeState.Running:
                        m_state = ENodeState.Running;
                        return m_state;

                    default:
                        continue;
                }
            }

            m_state = ENodeState.Failure;
            return m_state;
        }
    }

    ///// <summary>
    ///// ѡ��ڵ�(ÿһִ֡��һ���ӽڵ�)
    ///// </summary>
    //public class BehaviourTreeSelectNodeOneFrameOneChild : BehaviourTreeBaseNode
    //{
    //    public BehaviourTreeSelectNodeOneFrameOneChild() : base() { }

    //    public override ENodeState Execute()
    //    {
    //        BehaviourTreeBaseNode childNode;
    //        if (m_childList.Count != 0)
    //        {
    //            childNode = m_childList[m_nodeIndex];
    //            switch (childNode.Execute())//����ִ���ӽڵ�Ľ��
    //            {
    //                case ENodeState.Success:
    //                    m_nodeIndex = 0;
    //                    return ENodeState.Success;
    //                case ENodeState.Failure:
    //                    m_nodeIndex++;
    //                    if (m_nodeIndex == m_childList.Count)
    //                    {
    //                        m_nodeIndex = 0;
    //                        return ENodeState.Failure;
    //                    }
    //                    break;
    //                case ENodeState.Running:
    //                    return ENodeState.Running;
    //            }
    //        }
    //        return ENodeState.Failure;
    //    }
    //}

    ///// <summary>
    ///// ѡ��ڵ�(һִ֡��ȫ���ӽڵ�)
    ///// </summary>
    //public class BehaviourTreeSelectNodeOneFrameAllChild : BehaviourTreeBaseNode
    //{
    //    public BehaviourTreeSelectNodeOneFrameAllChild() : base() { }
    //    public override ENodeState Execute()
    //    {
    //        foreach (BehaviourTreeBaseNode childNode in m_childList)
    //        {
    //            ENodeState result = childNode.Execute();
    //            if (result != ENodeState.Failure)
    //            {
    //                return result;
    //            }
    //        }
    //        return ENodeState.Failure;
    //    }
    //}
    #endregion

    #region ˳��ڵ�
    // ��ִ�б����ͽڵ�ʱ��������ͷ��β����ִ���Լ����ӽڵ㣬�������һ���ӽڵ�ִ�к󷵻�Fasle,�ͻ�����ֹͣ������ͬʱ���ڵ�����Լ��ĸ��ڵ�Ҳ�᷵��Fasle��
    // �෴�����ӽڵ㶼������True ����ô���ڵ�Ҳ�����Լ��ĸ��ڵ㷵��Ture��
    // ��һ�������ӽڵ㷵��ʧ�ܵ�״̬��������ֹͣ������

    public class Sequence : BehaviourTreeBaseNode
    {
        public Sequence() : base() { }
        public Sequence(List<BehaviourTreeBaseNode> m_childList) : base(m_childList) { }

        public override ENodeState Execute()
        {
            bool anyChildIsRunning = false;

            foreach (BehaviourTreeBaseNode node in m_childList)
            {
                switch (node.Execute())
                {
                    case ENodeState.Failure:
                        m_state = ENodeState.Failure;
                        return m_state;

                    case ENodeState.Success:
                        continue;

                    case ENodeState.Running:
                        anyChildIsRunning = true;
                        continue;

                    default:
                        m_state = ENodeState.Success;
                        return m_state;
                }
            }

            m_state = anyChildIsRunning ? ENodeState.Running : ENodeState.Success;
            return m_state;
        }
    }

    ///// <summary>
    ///// ˳��ڵ�(ÿһִ֡��һ���ӽڵ�)
    ///// </summary>
    //public class BehaviourTreeSequenceNodeOneFrameOneChild : BehaviourTreeBaseNode
    //{
    //    public BehaviourTreeSequenceNodeOneFrameOneChild() : base() { }
    //    public override ENodeState Execute()
    //    {
    //        BehaviourTreeBaseNode childNode;
    //        if (m_childList.Count != 0)
    //        {
    //            childNode = m_childList[m_nodeIndex];
    //            switch (childNode.Execute())
    //            {
    //                case ENodeState.Success:
    //                    m_nodeIndex++;
    //                    if (m_nodeIndex == m_childList.Count)
    //                    {
    //                        m_nodeIndex = 0;
    //                        return ENodeState.Success;
    //                    }
    //                    break;
    //                case ENodeState.Failure:
    //                    m_nodeIndex = 0;
    //                    return ENodeState.Failure;
    //                case ENodeState.Running:
    //                    return ENodeState.Running;
    //                default:
    //                    break;
    //            }
    //        }
    //        return ENodeState.Failure;
    //    }
    //}

    ///// <summary>
    ///// ˳��ڵ�(ÿһִ֡��ȫ���ӽڵ�)
    ///// </summary>
    //public class BehaviourTreeSequenceNodeOneFrameAllChild : BehaviourTreeBaseNode
    //{
    //    public BehaviourTreeSequenceNodeOneFrameAllChild() : base() { }
    //    public override ENodeState Execute()
    //    {
    //        foreach(BehaviourTreeBaseNode childNode in m_childList)
    //        {
    //            ENodeState result = childNode.Execute();
    //            if(result != ENodeState.Success)
    //            {
    //                return result;
    //            }
    //        }
    //        return ENodeState.Success;
    //    }
    //}
    #endregion

    /// <summary>
    /// ����װ�νڵ㣺
    /// ��������÷��ظ��ϼ����������ӽڵ�ΪTrueʱ�����ظ��Լ��ĸ��ڵ�ΪFasle����֮ͬ��
    /// </summary>
    public class BehaviourTreeDecoratorNot : BehaviourTreeBaseNode
    {
        public Func<bool> action;

        public BehaviourTreeDecoratorNot(Func<bool> action)
        {
            this.action = action;
        }

        public override ENodeState Execute()
        {
            if (action == null)
            {
                return ENodeState.Failure;
            }

            return action.Invoke() ? ENodeState.Failure : ENodeState.Success;
        }
    }

    /// <summary>
    /// ѭ���ڵ�
    /// </summary>
    public class Loop : BehaviourTreeBaseNode
    {
        BehaviourTreeBaseNode child;

        public Loop() : base() { }
        public Loop(BehaviourTreeBaseNode child)
        {
            this.child = child;
        }

        public override ENodeState Execute()
        {
            ENodeState state = child.Execute();
            if(state==ENodeState.Success || state == ENodeState.Failure)
            {
                child.Execute();
            }


            m_state = ENodeState.Failure;
            return m_state;
        }
    }

    /// <summary>
    /// �ڰ��㣺
    /// ���ڶ�ȡ��д�빫�����ݣ��Ա���Ϊ���ڵ�֮�乲����Ϣ������洢�ʹ���״̬��Ϣ�����ߴ洢���˵�λ�ã������Ļ���������
    /// </summary>
    public class DataBase
    {
        public Dictionary<string, object> m_dataContext = new Dictionary<string, object>();

        /// <summary>
        /// д�빫������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Setdata(string key, object value)
        {
            if (m_dataContext.ContainsKey(key))
            {
                m_dataContext[key] = value;
            }
            else
            {
                m_dataContext.Add(key, value);
            }
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            if (m_dataContext.ContainsKey(key))
            {
                return m_dataContext[key];
            }
            Debug.LogWarning("û�д����ݣ��޷���ȡ");
            return null;
        }

        /// <summary>
        /// ��չ�������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ClearData(string key)
        {
            if (m_dataContext.ContainsKey(key))
            {
                m_dataContext.Remove(key);

                return true;
            }
            Debug.LogWarning("û�д����ݣ��޷����");
            return false;
        }
    }

}
