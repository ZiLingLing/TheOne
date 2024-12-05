using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    /// <summary>
    /// ��Ϸ�����������ڿ�����Ϸ����
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameObject m_player;
        public GameObject prefab;

        /// <summary>
        /// BOSS�б�
        /// </summary>
        public List<GameObject> m_boss;

        /// <summary>
        /// �����б�
        /// </summary>
        public List<GameObject> m_enemys;
        [SerializeField]public Dictionary<GameObject, int> m_enemy;

        /// <summary>
        /// ������Դ�б�
        /// </summary>
        public List<GameObject> m_reward;

        /// <summary>
        /// ��¼���з�����б�
        /// </summary>
        private List<GameObject> m_roomList;

        /// <summary>
        /// ��¼�����ŵ��б�
        /// </summary>
        private List<GameObject> m_doorList;

        /// <summary>
        /// ��¼�Ƿ��һ��̤���Ӧ����
        /// </summary>
        private Dictionary<GameObject, bool> m_roomInfo;

        /// <summary>
        /// ��ǰ���β���
        /// </summary>
        private int m_level = 0;

        /// <summary>
        /// ��ͼ�е��˴������
        /// </summary>
        private int m_enemyNum = 0;

        private List<GameObject> m_currentEnemy;

        //���ɵ��ˡ�Boss�������ĸ���Ȩ��
        private float rewardWeight;
        private float enemyWeight;
        private float bossWeight;

        private void Awake()
        {
            LoadConfigData();
            prefab = Resources.Load<GameObject>("Character/Player");
        }

        private void OnEnable()
        {
            m_enemyNum = 0;
            EventManager.AddEventListener("OnEnemyDie", OnEnemyDie);
            //������ͼ

        }

        private void Start()
        {
            //m_player = EventManager.TriggerEvent<Transform>("GetPlayerTransform").gameObject;
            //Debug.Log("GameManager���Ƿ���ȷ��ȡ��player��Transform" + (m_player == null));

            GenerateMap();

            //���ؽ�ɫ
            Vector3 entryPosition = EventManager.TriggerEvent<Vector3>("GetEntryPoint");
            if (m_player == null)
            {
                Vector3 playerPosition = new Vector3(entryPosition.x, entryPosition.y + 1.5f, entryPosition.z);
                m_player = Instantiate(prefab, playerPosition, Quaternion.identity);
            }
            else
            {
                m_player.transform.position = new Vector3(entryPosition.x, entryPosition.y + 1.5f, entryPosition.z);
            }

            // ��ȡBattleScene����������
            int battleSceneIndex = SceneManager.GetSceneByName("BattleScene").buildIndex;
            // ����ʵ������������ص�BattleScene������
            SceneManager.MoveGameObjectToScene(m_player, SceneManager.GetSceneByBuildIndex(battleSceneIndex));


            //���ý�ɫλ��
            //m_player.transform.position = new Vector3(entryPosition.x, entryPosition.y + 2f, entryPosition.z);
            //m_player.SetActive(true);

            //���������
            //GameObject virtualCameraObj = new GameObject("CinemachineVirtualCamera");
            Debug.Log("��ʼ���������" + m_player.transform);
            EventManager.TriggerEvent<Transform>("SetCameraLookAt", VirtualCameraLookAt(m_player.transform));
            EventManager.TriggerEvent<Transform>("SetCameraFollow", VirtualCameraFollow(m_player.transform));
            EventManager.TriggerEvent<Vector3>("InitCamera", entryPosition);
            Debug.Log("������������");

            //�����ص��¼�
            EventManager.AddEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);

            m_doorList = EventManager.TriggerEvent<List<GameObject>>("GetDoorList");

            //ͨ���ֵ��Ƿ����Ƿ�����
            m_roomList = EventManager.TriggerEvent<List<GameObject>>("GetRoomList");
            m_roomInfo = new Dictionary<GameObject, bool>();
            foreach (var item in m_roomList)
            {
                m_roomInfo.Add(item, false);
                Debug.Log(item);
            }


            EventManager.TriggerEvent<Transform>("SetCameraLookAt", VirtualCameraLookAt(m_player.transform));
            EventManager.TriggerEvent<Transform>("SetCameraFollow", VirtualCameraFollow(m_player.transform));
            EventManager.TriggerEvent<Vector3>("InitCamera", entryPosition);
        }

        private void Update()
        {
            EventManager.TriggerEvent<Transform>("SetCameraFollow", VirtualCameraFollow(m_player.transform));
            OnEntryRoom();
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);
            EventManager.RemoveEventListener("OnEnemyDie", OnEnemyDie);
            Destroy(m_player);

            m_roomList.Clear();
            m_roomInfo.Clear();

            Destroy(m_player);
        }

        /// <summary>
        /// ����������Ϣ
        /// </summary>
        private void LoadConfigData()
        {

        }

        /// <summary>
        /// �����������������LookAt����
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Transform VirtualCameraFollow(Transform lookAt)
        {
            Transform roomTransform = null;
            RaycastHit hitInfo;
            Vector3 origin = new Vector3(lookAt.transform.position.x, lookAt.transform.position.y, lookAt.transform.position.z);
            if (Physics.Raycast(origin, Vector3.down, out hitInfo))
            {
                roomTransform = hitInfo.transform;
                //Debug.Log("��⵽����" + hitInfo.transform.gameObject.name);
            }
            return roomTransform;
        }

        /// <summary>
        /// �����������������Follow����
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Transform VirtualCameraLookAt(Transform follow)
        {
            Transform characterTransform = follow.transform;
            return characterTransform;
        }

        /// <summary>
        /// ̤��ĳ������ʱ����Ƿ��ǵ�һ�ν��룬�����������Ӧ����
        /// </summary>
        public void OnEntryRoom()
        {
            if (m_player != null)
            {
                string roomTag = null;
                RaycastHit hitInfo;
                Vector3 origin = m_player.transform.position;
                if (Physics.Raycast(origin, Vector3.down, out hitInfo))
                {
                    Debug.Log(hitInfo.transform.gameObject.name);
                    if (m_roomInfo.ContainsKey(hitInfo.transform.parent.parent.gameObject) == true && m_roomInfo[hitInfo.transform.parent.parent.gameObject] == false)
                    {
                        roomTag = hitInfo.transform.parent.parent.tag;
                        Debug.Log(hitInfo.transform.position + "---" + hitInfo.transform.name);
                        m_roomInfo[hitInfo.transform.parent.parent.gameObject] = true;
                        Debug.Log(roomTag);
                        switch (roomTag)
                        {
                            //case "StartRoom":
                            //    break;
                            case "FightRoom":
                                OnEntryFightRoom(hitInfo.transform.position);
                                break;
                            case "RewardRoom":
                                OnEntryRewardRoom(hitInfo.transform.position);
                                break;
                            case "EndRoom":
                                //OnEntryEndRoom();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// ��һ��̤��ս����
        /// </summary>
        private void OnEntryFightRoom(Vector3 spawnPosition)
        {
            int weightSum = Random.Range(4, 7);

            while (weightSum > 0)
            {
                Debug.Log("��ǰ����ʣ���Ȩ�أ�" + weightSum);
                GameObject obj = m_enemys[Random.Range(0, m_enemys.Count)];
                Vector3 spawnPos = new Vector3(Random.Range(spawnPosition.x - 5.2f, spawnPosition.x + 5.2f), 0f, Random.Range(spawnPosition.z - 5.2f, spawnPosition.z + 5.2f));

                //GameObjectPoolManager.SpawnObject(obj, spawnPos, Quaternion.identity);
                obj = Instantiate(obj, spawnPos, Quaternion.identity);
                int weight = EventManager.TriggerEvent<Object, int>("GetWeight", obj);
                weightSum -= weight;

                // ��ȡBattleScene����������
                int battleSceneIndex = SceneManager.GetSceneByName("BattleScene").buildIndex;
                // ����ʵ������������ص�BattleScene������
                SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByBuildIndex(battleSceneIndex));
                //m_currentEnemy.Add(obj);
                m_enemyNum++;
            }
            foreach(var door in m_doorList)
            {
                door.SetActive(true);
            }

        }

        /// <summary>
        /// ��һ��̤�뽱����
        /// </summary>
        private void OnEntryRewardRoom(Vector3 spawnPosition)
        {
            GameObject weapon = m_reward[Random.Range(0, m_reward.Count)];
            Debug.Log(weapon.name=="Shield");
            if (weapon.name == "Shield")
            {
                //GameObjectPoolManager.SpawnObject(weapon, , Quaternion.identity, GameObjectPoolManager.EPoolType.None, );
                weapon = Instantiate(weapon, new Vector3(spawnPosition.x, 1.6f, spawnPosition.z),new Quaternion());
                weapon.transform.SetLocalScaleX(0.15f);
                weapon.transform.SetLocalScaleY(0.15f);
                weapon.transform.SetLocalScaleZ(0.15f);
            }
            else
            {
                weapon = Instantiate(weapon, new Vector3(spawnPosition.x, 1.5f, spawnPosition.z), new Quaternion());
            }
        }

        /// <summary>
        /// ��һ��̤��Boss��
        /// </summary>
        private void OnEntryEndRoom()
        {
            //if (m_level % 3 == 0)
            //{
            //    GameObjectPoolManager.SpawnObject(m_boss[Random.Range(0, m_boss.Count)], Vector3.zero,Quaternion.identity,GameObjectPoolManager.EPoolType.None,3f,3f,3f);
            //}
        }

        /// <summary>
        /// ��ȡplayer��Transform���
        /// </summary>
        /// <returns></returns>
        private Transform GetPlayerTransform()
        {
            if (m_player != null)
            {
                return m_player.transform;
            }
            Debug.LogWarning("���˻�ȡ��ɫ��TransformΪ��");
            return null;
        }

        /// <summary>
        /// ������ͼ
        /// </summary>
        private void GenerateMap()
        {
            m_level++;
            EventManager.TriggerEvent("GenerateMap");
        }

        /// <summary>
        /// ��������ʱ������������һ
        /// </summary>
        private void OnEnemyDie()
        {
            m_enemyNum--;
            if (m_enemyNum == 0)
            {
                foreach(var door in m_doorList)
                {
                    door.SetActive(false);
                }
            }
        }
    }
}

