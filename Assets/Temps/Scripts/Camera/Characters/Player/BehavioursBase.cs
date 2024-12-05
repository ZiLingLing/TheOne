using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// ��ɫ��Ϊ�Ļ���
    /// </summary>
    public class BehavioursBase : MonoBehaviour,IAbstractActions
    {
        /// <summary>
        /// ��ɫ����
        /// </summary>
        public float m_speed;

        /// <summary>
        /// ��ɫ��ǰѪ��
        /// </summary>
        public float m_currentHp;

        /// <summary>
        /// ��ɫ�������
        /// </summary>
        public float m_attackInverval;

        /// <summary>
        /// ��ɫ������
        /// </summary>
        public float m_attack;

        /// <summary>
        /// ��ɫѪ������
        /// </summary>
        public float m_hpUpperLimit;

        /// <summary>
        /// ����
        /// </summary>
        public virtual void Attack()
        {

        }

        /// <summary>
        /// �ƶ�
        /// </summary>
        public virtual void Move()
        {
            
        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void Wound(float hit,Object woundCharacter)
        {
            m_currentHp -= hit;
            if(m_currentHp==0)
            {
                Dead();
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void Dead()
        {
            Destroy(this);
        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void FaceDirection()
        {
            
        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void InterActive()
        {

        }
    }

}
