using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// ������UIԤ����ű��ϵĳ�����
    /// </summary>
    public abstract class View : MonoBehaviour
    {
        /// <summary>
        /// //����һ�����󷽷���ÿ��UI����̳иýű�������ʵ��Init����
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// ���õ�ǰ��������
        /// </summary>
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// ���õ�ǰ������ʾ
        /// </summary>
        public virtual void Show()
        {
             this.gameObject.SetActive(true);
        }
    }
}
