using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roguelike
{
    /// <summary>
    /// 游戏管理器，用于控制游戏流程
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameObject m_player;
        public GameObject prefab;

        /// <summary>
        /// BOSS列表
        /// </summary>
        public List<GameObject> m_boss;

        /// <summary>
        /// 敌人列表
        /// </summary>
        public List<GameObject> m_enemys;
        [SerializeField]public Dictionary<GameObject, int> m_enemy;

        /// <summary>
        /// 奖励资源列表
        /// </summary>
        public List<GameObject> m_reward;

        /// <summary>
        /// 记录所有房间的列表
        /// </summary>
        private List<GameObject> m_roomList;

        /// <summary>
        /// 记录所有门的列表
        /// </summary>
        private List<GameObject> m_doorList;

        /// <summary>
        /// 记录是否第一次踏入对应房间
        /// </summary>
        private Dictionary<GameObject, bool> m_roomInfo;

        /// <summary>
        /// 当前地牢层数
        /// </summary>
        private int m_level = 0;

        /// <summary>
        /// 地图中敌人存活数量
        /// </summary>
        private int m_enemyNum = 0;

        private List<GameObject> m_currentEnemy;

        //生成敌人、Boss、奖励的各项权重
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
            //创建地图

        }

        private void Start()
        {
            //m_player = EventManager.TriggerEvent<Transform>("GetPlayerTransform").gameObject;
            //Debug.Log("GameManager中是否正确获取了player的Transform" + (m_player == null));

            GenerateMap();

            //加载角色
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

            // 获取BattleScene场景的索引
            int battleSceneIndex = SceneManager.GetSceneByName("BattleScene").buildIndex;
            // 将新实例化的物体加载到BattleScene场景中
            SceneManager.MoveGameObjectToScene(m_player, SceneManager.GetSceneByBuildIndex(battleSceneIndex));


            //设置角色位置
            //m_player.transform.position = new Vector3(entryPosition.x, entryPosition.y + 2f, entryPosition.z);
            //m_player.SetActive(true);

            //设置摄像机
            //GameObject virtualCameraObj = new GameObject("CinemachineVirtualCamera");
            Debug.Log("开始设置摄像机" + m_player.transform);
            EventManager.TriggerEvent<Transform>("SetCameraLookAt", VirtualCameraLookAt(m_player.transform));
            EventManager.TriggerEvent<Transform>("SetCameraFollow", VirtualCameraFollow(m_player.transform));
            EventManager.TriggerEvent<Vector3>("InitCamera", entryPosition);
            Debug.Log("摄像机设置完毕");

            //添加相关的事件
            EventManager.AddEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);

            m_doorList = EventManager.TriggerEvent<List<GameObject>>("GetDoorList");

            //通过字典标记房间是否进入过
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
        /// 加载配置信息
        /// </summary>
        private void LoadConfigData()
        {

        }

        /// <summary>
        /// 用于设置虚拟相机的LookAt属性
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
                //Debug.Log("检测到房间" + hitInfo.transform.gameObject.name);
            }
            return roomTransform;
        }

        /// <summary>
        /// 用于设置虚拟相机的Follow属性
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Transform VirtualCameraLookAt(Transform follow)
        {
            Transform characterTransform = follow.transform;
            return characterTransform;
        }

        /// <summary>
        /// 踏入某个房间时检测是否是第一次进入，若是则进行相应操作
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
        /// 第一次踏入战斗房
        /// </summary>
        private void OnEntryFightRoom(Vector3 spawnPosition)
        {
            int weightSum = Random.Range(4, 7);

            while (weightSum > 0)
            {
                Debug.Log("当前房间剩余的权重：" + weightSum);
                GameObject obj = m_enemys[Random.Range(0, m_enemys.Count)];
                Vector3 spawnPos = new Vector3(Random.Range(spawnPosition.x - 5.2f, spawnPosition.x + 5.2f), 0f, Random.Range(spawnPosition.z - 5.2f, spawnPosition.z + 5.2f));

                //GameObjectPoolManager.SpawnObject(obj, spawnPos, Quaternion.identity);
                obj = Instantiate(obj, spawnPos, Quaternion.identity);
                int weight = EventManager.TriggerEvent<Object, int>("GetWeight", obj);
                weightSum -= weight;

                // 获取BattleScene场景的索引
                int battleSceneIndex = SceneManager.GetSceneByName("BattleScene").buildIndex;
                // 将新实例化的物体加载到BattleScene场景中
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
        /// 第一次踏入奖励房
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
        /// 第一次踏入Boss房
        /// </summary>
        private void OnEntryEndRoom()
        {
            //if (m_level % 3 == 0)
            //{
            //    GameObjectPoolManager.SpawnObject(m_boss[Random.Range(0, m_boss.Count)], Vector3.zero,Quaternion.identity,GameObjectPoolManager.EPoolType.None,3f,3f,3f);
            //}
        }

        /// <summary>
        /// 获取player的Transform组件
        /// </summary>
        /// <returns></returns>
        private Transform GetPlayerTransform()
        {
            if (m_player != null)
            {
                return m_player.transform;
            }
            Debug.LogWarning("敌人获取角色的Transform为空");
            return null;
        }

        /// <summary>
        /// 创建地图
        /// </summary>
        private void GenerateMap()
        {
            m_level++;
            EventManager.TriggerEvent("GenerateMap");
        }

        /// <summary>
        /// 怪物死亡时存活怪物数量减一
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

