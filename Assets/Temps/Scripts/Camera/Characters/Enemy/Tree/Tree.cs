using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public enum Status
    {
        Failure = 0,
        Success,
        Running,
    }

    [RequireComponent(typeof(Blackboard))]
    public class Tree : BehavioursBase
    {
        private Node m_root;
        private Blackboard m_blackBoard;

        public int m_weight;
        //private void Awake()
        //{
        //    OnSetup();
        //}

        //private void Update()
        //{
        //    if (m_root != null)
        //    {
        //        m_root.Evaluate(this.gameObject.transform, m_blackBoard);
        //    }
        //}

        protected virtual void OnSetup()
        {

        }

        protected virtual int GetWeight(Object obj)
        {
            int weight = 0;
            if (obj == this.gameObject)
            {
                weight = m_weight;
            }
            return m_weight;
        }

        //public virtual void Move()
        //{
            
        //}

        //public virtual void Attack()
        //{
            
        //}

        //public virtual void Wound(float hit, Object woundCharacter)
        //{
            
        //}

        //public virtual void Dead()
        //{
            
        //}

        //public virtual void FaceDirection()
        //{
            
        //}
    }
    public abstract class Node
    {
        private Node m_parent;
        protected List<Node> m_children = new List<Node>();
        private Status m_status;

        public Status Status
        {
            get { return m_status; }
            protected set { m_status = value; }
        }

        public Status Evaluate(Transform agent, Blackboard blackboard)
        {
            Status statu = OnEvaluate(agent, blackboard);
            return statu;
        }

        protected abstract Status OnEvaluate(Transform agent, Blackboard blackboard);

        public void AddNode(params Node[] nodes)
        {
            foreach(var node in nodes)
            {
                node.m_parent = this;
                m_children.Add(node);
            }
        }
    }

    /// <summary>
    /// 顺序节点
    /// </summary>
    public class SequenceNode : Node
    {
        protected override Status OnEvaluate(Transform agent, Blackboard blackboard)
        {
            bool isRunning = false;
            bool success = false;
            foreach(var child in m_children)
            {
                Status curStatue = child.Evaluate(agent, blackboard);
                if (curStatue == Status.Failure)
                {
                    return Status.Failure;
                }
                else if (curStatue == Status.Running)
                {
                    isRunning = true;
                }
                else if (curStatue == Status.Success)
                {
                    success = true;
                }
            }

            return isRunning ? Status.Running : success ? Status.Success : Status.Failure;
        }
    }

    /// <summary>
    /// 选择节点
    /// </summary>
    public class SelectNode : Node
    {
        protected override Status OnEvaluate(Transform agent, Blackboard blackboard)
        {
            bool isRunning = false;
            bool success = false;
            foreach (var child in m_children)
            {
                Status curStatue = child.Evaluate(agent, blackboard);
                if (curStatue == Status.Failure)
                {
                    continue;
                }
                else if (curStatue == Status.Running)
                {
                    isRunning = true;
                }
                else if (curStatue == Status.Success)
                {
                    return Status.Success;
                }
            }

            return isRunning ? Status.Running : success ? Status.Success : Status.Failure;
        }
    }

}

