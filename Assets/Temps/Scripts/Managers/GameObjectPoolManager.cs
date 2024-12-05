using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// �����
    /// </summary>
    public class GameObjectPoolManager : MonoBehaviour
    {
        public static List<PoolObjectInfo> m_objectPools = new List<PoolObjectInfo>();

        private GameObject m_objectPoolEmptyHolder;
        private static GameObject s_gameObjectEmpty;
        private static GameObject s_particleSystemEmpty;

        public enum EPoolType
        {
            GameObject,
            ParticleSystem,
            None,
        }

        public static EPoolType s_poolingType;

        private void Awake()
        {
            SetupEmpties();
        }

        #region ��������
        /// <summary>
        /// ͨ������ش�������
        /// </summary>
        /// <param name="objectToSpawn"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="spawnRotation"></param>
        /// <returns></returns>
        public static GameObject SpawnObject(GameObject objectToSpawn,Vector3 spawnPosition,Quaternion spawnRotation,EPoolType poolType=EPoolType.None,float scaleX=1f,float scaleY=1f,float scaleZ=1f)
        {
            PoolObjectInfo pool = m_objectPools.Find(p => p.m_lookUpString == objectToSpawn.name);
            //PoolObjectInfo pool = null;
            //foreach(PoolObjectInfo p in m_objectPools)
            //{
            //    if(p.m_lookUpString==objectToSpawn.name)
            //    {
            //        pool = p;
            //        break;
            //    }
            //}

            //���������б���û�иö�����򿪱�һ���µĶ���ز���ӵ��б���
            if(pool==null)
            {
                pool = new PoolObjectInfo(objectToSpawn.name);
                m_objectPools.Add(pool);
            }

            //����������ʧ�����壬ѭ�����������е�ʧ����󣬽�ʧ����󸳸��´����Ķ���
            GameObject spawnAbleObj = null;
            foreach(GameObject obj in pool.m_InactiveObjects)
            {
                if(obj!=null)
                {
                    spawnAbleObj = obj;
                    break;
                }
            }

            //���û���ҵ�һ��ʧ��ĸó��е�GameObject��ʵ����һ���µĶ��󸳸�����
            //�����λ���Լ�����ֵ����ʧ������б���ɾ���ö���Ȼ�����¼��������
            if (spawnAbleObj == null)
            {
                //�ҵ����������Ӧ�ĸ�����
                GameObject parentObject = SetParentObject(poolType);

                spawnAbleObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

                if (parentObject != null)
                {
                    spawnAbleObj.transform.SetParent(parentObject.transform);
                }
            }
            else
            {
                pool.m_InactiveObjects.Remove(spawnAbleObj);
                spawnAbleObj.transform.position = spawnPosition;
                spawnAbleObj.transform.rotation = spawnRotation;
                spawnAbleObj.SetActive(true);
            }

            spawnAbleObj.transform.localScale = new Vector3(scaleX,scaleY,scaleZ);
            return spawnAbleObj;
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform)
        {
            PoolObjectInfo pool = m_objectPools.Find(p => p.m_lookUpString == objectToSpawn.name);

            if (pool == null)
            {
                pool = new PoolObjectInfo(objectToSpawn.name);
                m_objectPools.Add(pool);
            }

            GameObject spawnAbleObj = null;
            foreach (GameObject obj in pool.m_InactiveObjects)
            {
                if (obj != null)
                {
                    spawnAbleObj = obj;
                    break;
                }
            }

            if (spawnAbleObj == null)
            {
                spawnAbleObj = Instantiate(objectToSpawn, parentTransform);

                spawnAbleObj.transform.SetParent(parentTransform);
                spawnAbleObj.transform.position = parentTransform.position;
                spawnAbleObj.transform.rotation = parentTransform.rotation;
            }
            else
            {
                pool.m_InactiveObjects.Remove(spawnAbleObj);
                spawnAbleObj.SetActive(true);
            }

            return spawnAbleObj;
        }
        #endregion
        /// <summary>
        /// �����巵�ظó���
        /// </summary>
        /// <param name="obj"></param>
        public static void ReturnObjectToPool(GameObject obj)
        {
            string trueName = obj.name.Substring(0, obj.name.Length - 7);
            //Debug.Log(obj.name+"--"+ trueName);
            PoolObjectInfo pool = m_objectPools.Find(p=>trueName == p.m_lookUpString);

            if(pool!=null)
            {
                //if (pool.m_InactiveObjects.Count > 200)
                //{
                //    Destroy(obj);
                //}
                //else
                //{
                    obj.SetActive(false);
                    pool.m_InactiveObjects.Add(obj);
                //}
            }
            else
            {
                Debug.Log("�����ͷ�һ�����ڶ�����е�����");
                Destroy(obj);
            }
        }

        /// <summary>
        /// ��������ͬ���͵ĳ��Ӷ�Ӧ�Ŀ����壬�Լ����ǵĸ�����
        /// </summary>
        private void SetupEmpties()
        {
            m_objectPoolEmptyHolder = new GameObject("Pool Objects");

            s_gameObjectEmpty = new GameObject("GameObjects");
            s_gameObjectEmpty.transform.SetParent(m_objectPoolEmptyHolder.transform);

            s_particleSystemEmpty = new GameObject("Particle Effects");
            s_particleSystemEmpty.transform.SetParent(m_objectPoolEmptyHolder.transform);

        }

        /// <summary>
        /// �Ѳ�ͬ���ɵĲ�ͬ���͵�������ڶ�Ӧ�ĸ�������
        /// </summary>
        /// <param name="poolType"></param>
        /// <returns></returns>
        private static GameObject SetParentObject(EPoolType poolType)
        {
            switch (poolType)
            {
                case EPoolType.GameObject:
                    return s_gameObjectEmpty;
                case EPoolType.ParticleSystem:
                    return s_particleSystemEmpty;
                case EPoolType.None:
                    return null;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// �������Ϣ
    /// </summary>
    public class PoolObjectInfo
    {
        public string m_lookUpString;
        public List<GameObject> m_InactiveObjects = new List<GameObject>();

        public PoolObjectInfo(string lookUpString)
        {
            m_lookUpString = lookUpString;
        }
    }
}