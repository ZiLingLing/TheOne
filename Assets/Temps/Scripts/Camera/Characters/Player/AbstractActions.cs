using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// �淶����������Ϊ
    /// </summary>
    public interface IAction
    {

    }

    /// <summary>
    /// ��Ӧ������ĳ���
    /// </summary>
    public interface IAbstractActions
    {
        public abstract void Move();

        public abstract void Attack();

        public abstract void Wound(float hit,Object woundCharacter);

        public abstract void Dead();

        public abstract void FaceDirection();
    }
}

