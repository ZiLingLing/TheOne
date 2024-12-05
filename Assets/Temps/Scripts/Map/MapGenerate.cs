using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;


namespace Roguelike
{
    public class MapGenerate : MonoBehaviour
    {

        #region ��ͼ���
        /// <summary>
        /// ������������ɷ�����б�
        /// </summary>
        private List<GameObject> m_roomList;

        /// <summary>
        /// ������������ɷ����ǽ��
        /// </summary>
        private List<GameObject> m_wallList;

        private enum ERoomType
        {
            Entry,
            End,
            Fight,
            Reward,
        }

        /// <summary>
        /// ս�������б�
        /// </summary>
        public List<GameObject> m_fightRoomList;

        /// <summary>
        /// Boss���б�
        /// </summary>
        public List<GameObject> m_endRoomList;

        /// <summary>
        /// �������б�
        /// </summary>
        public List<GameObject> m_rewardRoomList;

        /// <summary>
        /// ��ڷ����б�
        /// </summary>
        public List<GameObject> m_entryRoomList;

        /// <summary>
        /// ���ſ��ǽ�����ڷ���������
        /// </summary>
        public GameObject m_wallWithDoor;

        /// <summary>
        /// û���ſ��ǽ
        /// </summary>
        public GameObject m_wallWithoutDoor;

        #endregion

        /// <summary>
        /// �����������䷶Χ
        /// </summary>
        public int m_minRoomCount = 7;
        public int m_maxRoomCount = 10;
        private int m_roomCount;
        private int m_currentRoomCount = 0;
        private int m_entryRoomCount = 1;
        private int m_endRoomCount = 1;
        [SerializeField,Tooltip("ս��������")]
        private int m_fightRoomCount;
        [SerializeField,Tooltip("����������")]
        private int m_rewardRoomCount = 1;

        /// <summary>
        /// ��ʼ����λ��
        /// </summary>
        private Vector3 m_spawnPoint=Vector3.zero;

        /// <summary>
        /// ���ݷ�������ڷ���ƫ����
        /// </summary>
        public float m_verticalOffset;
        public float m_horizontalOffset;

        /// <summary>
        /// ��¼������Ϣ���ֵ�
        /// </summary>
        private Dictionary<Vector3, ERoomType> m_roomDic = new Dictionary<Vector3, ERoomType>();

        /// <summary>
        /// ��¼ǽλ����Ϣ���ֵ�,keyΪǽ��λ��,valueΪ�Ƿ����
        /// </summary>
        private Dictionary<Vector3, bool> m_wallDic = new Dictionary<Vector3, bool>();

        /// <summary>
        /// ��¼ǽ���Ƿ���ת���ֵ�,keyΪǽ��λ��,valueΪ�Ƿ���ת90��
        /// </summary>
        private Dictionary<Vector3, bool> m_wallRotate = new Dictionary<Vector3, bool>();

        /// <summary>
        /// �������ɷ���
        /// </summary>
        private enum EGenerateDirection
        {
            Front=0,
            Right,
            Back,
            Left,
        }
        private int m_bannedGenerateDirection;

        private List<GameObject> m_doors = new List<GameObject>();

        #region �������ں���

        private void OnEnable()
        {
            EventManager.AddEventListener("GenerateMap", GenerateMap);
            EventManager.AddEventListener<Vector3>("GetEntryPoint", GetEntryRoomPosition);
            EventManager.AddEventListener<List<GameObject>>("GetRoomList", GetRoomList);
            EventManager.AddEventListener<List<GameObject>>("GetDoorList", GetDoorList);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener("GenerateMap", GenerateMap);
            EventManager.RemoveEventListener<Vector3>("GetEntryPoint", GetEntryRoomPosition);
            EventManager.RemoveEventListener<List<GameObject>>("GetRoomList", GetRoomList);
            EventManager.RemoveEventListener<List<GameObject>>("GetDoorList", GetDoorList);
        }

        #endregion

        /// <summary>
        /// ����ʵ��
        /// </summary>
        //private static MapGenerate s_instance;

        //public static MapGenerate Instance
        //{
        //    get
        //    {
        //        s_instance = FindObjectOfType<MapGenerate>();
        //        if (s_instance == null)
        //        {
        //            s_instance = new MapGenerate();
        //        }
        //        return s_instance;
        //    }
        //}

        //private MapGenerate()
        //{

        //}

        /// <summary>
        /// ���ɵ�ͼ
        /// </summary>
        /// 
        [InspectorButton("��ͼ����")]
        public void GenerateMap()
        {
            Init();
            //���ɵ�һ�����䣬��һ������ض�Ϊս����
            //ս������������ʼ��
            //��¼�ӵ�һ��ս�������������ķ���
            //������򲻻��Ϊ������������ɷ���
            GameObject mapRoot = GameObject.Find("MapRoot");
            if (mapRoot==null)
            {
                mapRoot = new GameObject("MapRoot");
                //Instantiate(mapRoot, m_spawnPoint, Quaternion.identity);
                // ��ȡBattleScene����������
                int battleSceneIndex = SceneManager.GetSceneByName("BattleScene").buildIndex;
                // ����ʵ������������ص�BattleScene������
                SceneManager.MoveGameObjectToScene(mapRoot, SceneManager.GetSceneByBuildIndex(battleSceneIndex));
            }
            //���ɵ�һ�������Լ�������������ʼ���䣬��ñ���ֹ���ɷ���ķ���
            m_roomDic.Add(m_spawnPoint, ERoomType.Fight);
            m_bannedGenerateDirection = UnityEngine.Random.Range(0, Enum.GetValues(typeof(EGenerateDirection)).Length);
            Vector3 entryPoint = RoomPointAfterOffset(m_spawnPoint, m_bannedGenerateDirection);
            Debug.Log(entryPoint);
            m_roomDic.Add(entryPoint, ERoomType.Entry);
            m_currentRoomCount += 2;

            //���ɷ���
            DefineRoomInfo(m_spawnPoint);
            GenerateRoom(m_roomDic);

            //����ǽ��
            DefineWallInfo();
            GenerateWall(m_wallDic, m_wallRotate);

            //���ϼ�������������һ������ʣ�µ���������û�����ɷ��䣬����û�������㹻��������������������
            if (m_currentRoomCount != m_roomCount)
            {
                GenerateMap();
            }

        }

        /// <summary>
        /// ��ʼ����ͼ�ĸ�������
        /// </summary>
        private void Init()
        {
            if(m_roomList == null || m_wallList == null)
            {
                m_roomList = new List<GameObject>();
                m_wallList = new List<GameObject>();
            }
            else
            {
                m_roomDic.Clear();
                m_wallDic.Clear();
                m_wallRotate.Clear();
                for (int i = m_roomList.Count - 1; i >= 0; i--)
                {
                    DestroyImmediate(m_roomList[i]);
                    m_roomList.RemoveAt(i);
                }
                for (int i = m_wallList.Count - 1; i >= 0; i--)
                {
                    DestroyImmediate(m_wallList[i]);
                    m_wallList.RemoveAt(i);
                }
            }
            m_roomCount = Random.Range(m_minRoomCount, m_maxRoomCount);
            m_currentRoomCount = 0;
            //m_fightRoomCount = m_roomCount - m_rewardRoomCount - m_endRoomCount - m_entryRoomCount;
        }

        /// <summary>
        /// ���ռ�¼������Ϣ���ֵ����ɷ���
        /// </summary>
        /// <param name="roomPoint"></param>
        private void GenerateRoom(Dictionary<Vector3,ERoomType> dic)
        {
            Debug.Log(dic.Count+"--"+m_roomCount);
            foreach(var roomInfo in dic)
            {
                GameObject room = null;
                switch (roomInfo.Value)
                {
                    case ERoomType.Fight:
                        room = Instantiate(m_fightRoomList[UnityEngine.Random.Range(0, m_fightRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    case ERoomType.Entry:
                        room = Instantiate(m_entryRoomList[UnityEngine.Random.Range(0, m_entryRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    case ERoomType.End:
                        room = Instantiate(m_endRoomList[UnityEngine.Random.Range(0, m_endRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    case ERoomType.Reward:
                        room = Instantiate(m_rewardRoomList[UnityEngine.Random.Range(0, m_rewardRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    default:
                        Debug.LogWarning("���ڲ������͵ķ��䣬λ����" + roomInfo.Key);
                        room = Instantiate(m_fightRoomList[UnityEngine.Random.Range(0, m_fightRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                }
                m_roomList.Add(room);

            }
        }

        /// <summary>
        /// ȷ�����з����λ��
        /// </summary>
        /// <param name="startPoint"></param>
        private void DefineRoomInfo(Vector3 startPoint)
        {
            Queue<Vector3> points = new Queue<Vector3>();
            points.Enqueue(startPoint);
            int rewardRoomNumber = UnityEngine.Random.Range(3, m_roomCount - 1);
            while (points.Count > 0)
            {
                //��Ϊ���ɵķ���ֵΪ���ɸ���
                Dictionary<int, float> orders = DefineOrder(m_bannedGenerateDirection);
                Vector3 oldPoint = points.Peek();
                foreach (var order in orders)
                {
                    if (m_currentRoomCount < m_roomCount)
                    {
                        //��������������õ㱻���Ϊ������״̬�����Ҹõ��λ��֮ǰû�з���
                        if(TryAddRoom(order.Value) == true && m_roomDic.ContainsKey(RoomPointAfterOffset(oldPoint,order.Key))==false)
                        {
                            Vector3 newPoint = RoomPointAfterOffset(oldPoint, order.Key);
                            points.Enqueue(newPoint);
                            if(m_currentRoomCount == m_roomCount - 1)
                            {
                                m_roomDic.Add(newPoint, ERoomType.End);
                            }
                            else if(m_currentRoomCount == rewardRoomNumber - 1)
                            {
                                m_roomDic.Add(newPoint, ERoomType.Reward);
                            }
                            else
                            {
                                m_roomDic.Add(newPoint, ERoomType.Fight);
                            }      
                            m_currentRoomCount++;
                        }
                    }

                }
                points.Dequeue();
            }
        }

        /// <summary>
        /// ȷ���÷���ķ�������˳���Լ����ɸ���
        /// </summary>
        /// <param name="bannedDirection"></param>
        /// <returns></returns>
        private Dictionary<int,float> DefineOrder(int bannedDirection)
        {
            Dictionary<int, float> order = new Dictionary<int, float>();
            List<int> directions = new List<int>(new int[] { 0, 1, 2, 3 });
            while (directions.Count > 0)
            {
                //ÿ����һ�������һ������
                int direction = directions[UnityEngine.Random.Range(0, directions.Count)];
                directions.Remove(direction);
                if (direction == bannedDirection)
                {
                    continue;
                }

                order.Add(direction,UnityEngine.Random.Range(0,1f));
            }
            return order;
        }

        /// <summary>
        /// ��λ���Ƿ�Ҫ���Ϊ������״̬
        /// </summary>
        /// <returns></returns>
        private bool TryAddRoom(float probability)
        {
            return UnityEngine.Random.Range(0, 1f) >= probability ? true : false;
        }

        /// <summary>
        /// �ӵ�ǰ������չ��ͼ�������һ�������λ��
        /// </summary>
        /// <param name="oldPoint"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Vector3 RoomPointAfterOffset(Vector3 oldPoint,int direction)
        {
            Vector3 newPoint=Vector3.zero;
            EGenerateDirection Edirection;
            switch (direction)
            {
                case 0:
                    Edirection = EGenerateDirection.Front;
                    break;
                case 1:
                    Edirection = EGenerateDirection.Right;
                    break;
                case 2:
                    Edirection = EGenerateDirection.Back;
                    break;
                case 3:
                    Edirection = EGenerateDirection.Left;
                    break;
                default:
                    Edirection = EGenerateDirection.Front;
                    Debug.LogError("���е�ͼ��չʱ�·���ķ������");
                    break;
            }
            switch (Edirection)
            {
                case EGenerateDirection.Front:
                        newPoint = new Vector3(oldPoint.x, oldPoint.y, oldPoint.z + m_verticalOffset);
                    break;
                case EGenerateDirection.Right:
                        newPoint = new Vector3(oldPoint.x + m_horizontalOffset, oldPoint.y, oldPoint.z);
                    break;
                case EGenerateDirection.Back:
                        newPoint = new Vector3(oldPoint.x, oldPoint.y, oldPoint.z - m_verticalOffset);
                    break;
                case EGenerateDirection.Left:
                        newPoint = new Vector3(oldPoint.x - m_horizontalOffset, oldPoint.y, oldPoint.z);
                    break;
                default:
                    Debug.LogWarning("�и������Ӧ��null");
                    break;
            }
            return newPoint;
        }

        /// <summary>
        /// ����ǽ����Ϣ
        /// </summary>
        private void DefineWallInfo()
        {
            foreach(var roomInfo in m_roomDic)
            {
                for(int i = 0; i < 4; i++)
                {
                    Vector3 wallPoint = WallPointAfterOffset(roomInfo.Key, i);
                    if(m_wallDic.ContainsKey(wallPoint) == false)
                    {
                        if (JudgeWallType(roomInfo.Key, i))
                        {
                            m_wallDic.Add(wallPoint, true);
                        }
                        else
                        {
                            m_wallDic.Add(wallPoint, false);
                        }
                    }
                    if (m_wallRotate.ContainsKey(wallPoint) == false)
                    {
                        if(i==0 || i == 2)
                        {
                            m_wallRotate.Add(wallPoint, true);
                        }
                        else
                        {
                            m_wallRotate.Add(wallPoint, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����ǽ������
        /// </summary>
        /// <param name="dic"></param>
        private void GenerateWall(Dictionary<Vector3, bool> vectorDic,Dictionary<Vector3, bool> rotationDic)
        {
            GameObject wall = null;
            foreach(var wallInfo in vectorDic)
            {
                if (wallInfo.Value == true)
                {
                    wall = Instantiate(m_wallWithDoor, wallInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                    m_doors.Add(wall.transform.Find("Door").gameObject);
                }
                else
                {
                    wall = Instantiate(m_wallWithoutDoor, wallInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                }
                if (rotationDic[wallInfo.Key] == false)
                {
                    wall.transform.Rotate(new Vector3(0, 90f, 0));
                }
                m_wallList.Add(wall);
            }
            //foreach(var wallInfo in rotationDic)
            //{
            //    if (rotationDic[wallInfo.Key] == false)
            //    {
            //        wall.transform.Rotate(new Vector3(0, 90f, 0));
            //    }
            //}
        }

        /// <summary>
        /// �ж�����λ�����Ƿ��з���
        /// </summary>
        /// <param name="roomPoint"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private bool JudgeWallType(Vector3 roomPoint,int direction)
        {
            if (m_roomDic.ContainsKey(RoomPointAfterOffset(roomPoint, direction)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �ӵ�ǰ������չ��ͼ����ø÷�����ǽ�ڵ�λ��
        /// </summary>
        /// <param name="oldPoint"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Vector3 WallPointAfterOffset(Vector3 roomPoint, int direction)
        {
            Vector3 wallPoint = Vector3.zero;
            EGenerateDirection Edirection;
            switch (direction)
            {
                case 0:
                    Edirection = EGenerateDirection.Front;
                    break;
                case 1:
                    Edirection = EGenerateDirection.Right;
                    break;
                case 2:
                    Edirection = EGenerateDirection.Back;
                    break;
                case 3:
                    Edirection = EGenerateDirection.Left;
                    break;
                default:
                    Edirection = EGenerateDirection.Front;
                    Debug.LogError("���е�ͼ��չʱ�·���ķ������");
                    break;
            }
            switch (Edirection)
            {
                case EGenerateDirection.Front:
                    wallPoint = new Vector3(roomPoint.x, roomPoint.y, roomPoint.z + m_verticalOffset * 0.5f);
                    break;
                case EGenerateDirection.Right:
                    wallPoint = new Vector3(roomPoint.x + m_horizontalOffset * 0.5f, roomPoint.y, roomPoint.z);
                    break;
                case EGenerateDirection.Back:
                    wallPoint = new Vector3(roomPoint.x, roomPoint.y, roomPoint.z - m_verticalOffset * 0.5f);
                    break;
                case EGenerateDirection.Left:
                    wallPoint = new Vector3(roomPoint.x - m_horizontalOffset * 0.5f, roomPoint.y, roomPoint.z);
                    break;
                default:
                    Debug.LogWarning("�и������Ӧ��null");
                    break;
            }
            return wallPoint;
        }

        /// <summary>
        /// ��ȡEntryRoom��λ��
        /// </summary>
        /// <returns></returns>
        private Vector3 GetEntryRoomPosition()
        {
            Vector3 entryPosition = Vector3.zero;
            foreach (var roomInfo in m_roomDic)
            {
                if (roomInfo.Value == ERoomType.Entry)
                {
                    entryPosition = roomInfo.Key;
                    break;
                }
            }
            return entryPosition;
        }

        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <returns></returns>
        private List<GameObject> GetRoomList()
        {
            if (m_roomList != null)
            {
                return m_roomList;
            }
            Debug.LogWarning("�����б�Ϊ��");
            return null;
        }

        /// <summary>
        /// ��ȡ���б�
        /// </summary>
        /// <returns></returns>
        private List<GameObject> GetDoorList()
        {
            if (m_doors != null)
            {
                return m_doors;
            }
            Debug.LogWarning("���б�Ϊ��");
            return null;
        }
    }
}
