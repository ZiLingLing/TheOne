using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Roguelike
{


    /// <summary>
    /// �涨������Ϊ�Ľӿ�
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// ִ�в���
        /// </summary>
        /// <param name="gamePlyaer"></param>
        public void Execute(BehavioursBase gamePlyaer);
        //public void Undo();
    }

    /// <summary>
    /// ��Ч������Ϊ
    /// </summary>
    public class EmptyCommand : ICommand
    {
        private BehavioursBase m_gamePlayer;
        public void Execute(BehavioursBase gamePlayer)
        {
            
        }
    }

    /// <summary>
    /// �ƶ���Ϊ
    /// </summary>
    public class MoveCommand : ICommand
    {
        private BehavioursBase m_actor;

        public void Execute(BehavioursBase actor)
        {
            actor.Move();

            Vector3 position = actor.gameObject.transform.position;
        }
    }

    /// <summary>
    /// ��ɫ��ת�������λ��
    /// </summary>
    public class RotateTowardMouseCommand : ICommand
    {
        private BehavioursBase m_actor;

        public void Execute(BehavioursBase actor)
        {
            actor.FaceDirection();

            Quaternion position = actor.gameObject.transform.rotation;
        }
    }

    /// <summary>
    /// ��ɫ�Ĺ�����Ϊ
    /// </summary>
    public class AttackCommand : ICommand
    {
        private BehavioursBase m_actor;

        public void Execute(BehavioursBase actor)
        {
            actor.Attack();

        }
    }

    /// <summary>
    /// ������Ϊ
    /// </summary>
    public class InterActiveCommand : ICommand
    {
        private BehavioursBase m_actor;

        public void Execute(BehavioursBase actor)
        {
            actor.InterActive();
        }
    }
}
