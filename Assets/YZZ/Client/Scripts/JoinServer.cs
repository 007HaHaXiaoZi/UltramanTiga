using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using DG.Tweening;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine.UI;
using System.Threading;
using RootMotion.FinalIK;
using System.Net.Mail;

namespace YZZ
{
    public class JoinServer : MonoBehaviour
    {
        //[HideInInspector]
        public float sendInterval = 0.2f;//延迟收发时间
        
        [Header("其他客户端预制体")]
        public GameObject OtherClientPrefabs_Girl;
        public GameObject OtherClientPrefabs_RedGirl;

        //公共物体
        /*public Transform blueBall;
        public Transform yellowBall;*/

        public InputField text_IpAddress;
        public string _IpAddress;

        //连接参数
        public UdpClient client;
        public IPEndPoint serverEndPoint;

        int PlayerID = 0;
        int ClientNum = 0;//判断用户人数，当用户人数改变的时候
        bool isFirst = true;//客户端第一次连接


        public XROrigin xrOrigin;

        public List<Transform> xrOriginHead=new List<Transform>();

        public List<Transform> xrOriginLeftHand=new List<Transform>();

        public List<Transform> xrOriginRightHand = new List<Transform>();

        public Text text_Debug;

        private CancellationTokenSource cancellationTokenSource;
        //private CancellationTokenSource cancellationTokenSource;

        //---------------------------总体---------------------------

        ClientAttribute clientAttribute;//客户端

        public List<GameObject> otherClientsPrefabs = new List<GameObject>();

        //----------------以下含有需要自动指定的对象--------------
        [Header("客户端顺序序列号")]
        public int getSequenceNumber = 0;//客户端序列号储存
        [Header("公共物体")]
        public PublicObjects getPublicObjects;//从服务端收到的_公共物体_足球  参数
        public Transform FootBall;//公共物体_足球
        public FootBallJoinServer footBallJoinServer;
        [Header("主角选择的所有")]
        public List<Transform> AllCharacters=new List<Transform>();


        [Header("用户选择的角色")]// /Resource/Characters
        public Transform ChoosePlayer;

        [Header("用户身高相关,根据选择的角色匹配")]
        public Transform MainCamera;
        public List<Transform> Head = new List<Transform>();
        public List<Transform> HeightItem = new List<Transform>();
        public List<Transform> Model = new List<Transform>();


        void Awake()
        {
            DontDestroyOnLoad(transform.root);
        }

        void Start()
        {
            Init();
        }


        void Init()
        {
            GetAllCharacters();

            OtherClientPrefabs_Girl = Resources.Load("Characters/Prefabs_Girl") as GameObject;//其他客户端人物
            OtherClientPrefabs_RedGirl = Resources.Load("Characters/Prefabs_RedGirl") as GameObject;//其他客户端人物

            _IpAddress = text_IpAddress.text.Trim();
            PlayerID = UnityEngine.Random.Range(0, 999);
            transform.root.name = "Player" + PlayerID;
            client = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(_IpAddress), ClientDatas.serverPort);


        }

        /// <summary>
        /// 客户端初始化
        /// </summary>
        public void ClientInit(int state)
        {
            clientAttribute = new ClientAttribute
            {
                ClientName = transform.root.name,
                ClientState = state,
                ClientPrefabsName = ClientDatas.SelectedCharacter,

                ClientScale = Model[ClientDatas.SelectedCharacterNum].transform.localScale,

                ClientPosition = xrOrigin.transform.position,
                ClientEulerAngle = xrOrigin.transform.eulerAngles,

                ClientHeadPosition = xrOriginHead[ClientDatas.SelectedCharacterNum].position,
                ClientHeadEulerAngle = xrOriginHead[ClientDatas.SelectedCharacterNum].eulerAngles,

                ClientLeftHandPosition = xrOriginLeftHand[ClientDatas.SelectedCharacterNum].position,
                ClientLeftHandEulerAngle = xrOriginLeftHand[ClientDatas.SelectedCharacterNum].eulerAngles,

                ClientRightHandPosition = xrOriginRightHand[ClientDatas.SelectedCharacterNum].position,
                ClientRightHandEulerAngle = xrOriginRightHand[ClientDatas.SelectedCharacterNum].eulerAngles,
            };
        }

        /// <summary>
        /// 客户端退出或关闭的时候调用
        /// </summary>
        public void ClientClose()
        {
            //退出的时候需要发一条消息告诉服务端，修改连接参数为0，客户端不要了
            SendPosition(() => { ClientInit(0); });
            StopAsyncOperationsAndCloseConnection();
        }

        /// <summary>
        /// 添加新用户并且跳转场景
        /// </summary>
        public void JoinServerFunction()
        {
            if (SceneManager.GetActiveScene().name == "ServerRoom")
            {
                GameObject.Find("PlayerCanvas").gameObject.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene("ServerRoom");
            }

            CertainHeight();

            GetComponent<MoveServer>().StartSend();//初始数据总要放到最后
        }

        /// <summary>
        /// 公共物体解包
        /// </summary>
        /// <param name="receivedJson"></param>
        void PublicObjectReceive(string receivedJson)
        {
            AllMessage allMessage = JsonConvert.DeserializeObject<AllMessage>(receivedJson);

            if(allMessage.Objects.updateMessage)
            {
                getPublicObjects = allMessage.Objects;//获得某个客户端发生碰撞的数据

                Debug.Log(getPublicObjects.updateMessage);
                Debug.Log(getPublicObjects.ClientName);
                Debug.Log(getPublicObjects.FootBall.ObjectPosition);
                Debug.Log(getPublicObjects.FootBall.ObjectEulerAngle);
                Debug.Log(getPublicObjects.FootBall.ObjectForce);

                for (int i = 0; i < allMessage.Clients.Count; i++)
                {

                    if (this.name != getPublicObjects.ClientName/*&&allMessage.Objects.FootBall.ObjectForce!=getPublicObjects.FootBall.ObjectForce*/)//证明 非此客户端的数据且数据进行了更新
                    {
                        Debug.Log("更新其他客户端公共物体的数据");
                        footBallJoinServer.UpdateFootBall(getPublicObjects.FootBall.ObjectPosition, getPublicObjects.FootBall.ObjectEulerAngle, getPublicObjects.FootBall.ObjectForce);//调用方法
                        getPublicObjects.ClientName = "";
                        getPublicObjects.updateMessage = false;
                        //allMessage.Objects = getPublicObjects;//更新列表中的Objects数据
                    }
                }
            }
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="receivedJson"></param>
        void ClientReceive(string receivedJson)
        {
            //Debug.Log(receivedJson);

            //反序列化JSON字符串为PlayerScore对象
            AllMessage allMessage = JsonConvert.DeserializeObject<AllMessage>(receivedJson);
            //判断上一次心跳中，用户数量是否等于此次，不等于代表添加了新的用户，等于继续操作

            //实例化所有客户端
            if (ClientNum != allMessage.Clients.Count)//如果上一次心跳收到的值和此次不同(其他用户加/减)
            {
                foreach (var item in otherClientsPrefabs)//删除所有用户新建物体
                {
                    Destroy(item);
                }
                otherClientsPrefabs.Clear();

                for (int i = 0; i < allMessage.Clients.Count; i++)
                {
                    if (allMessage.Clients[i].ClientName != transform.root.name && allMessage.Clients[i].ClientState == 1)//服务器不为当前用户且有连接
                    {
                        /*GameObject OtherClient;
                        if (allMessage.Clients[i].ClientPrefabsName== "Prefabs_Woman")
                        {
                            OtherClient = Instantiate(OtherClientPrefabs_Girl);//新建其他用户
                        }else if(allMessage.Clients[i].ClientPrefabsName == "Prefabs_RedGirl")
                        {
                            var OtherClient = Instantiate(OtherClientPrefabs_RedGirl);//新建其他用户
                        }*/

                        var OtherClient = GetGameObject(allMessage.Clients[i].ClientPrefabsName);//新建其他用户
                        //var OtherClient = Instantiate(OtherClientPrefabs_RedGirl);//新建其他用户
                        OtherClient.name = allMessage.Clients[i].ClientName;
                        otherClientsPrefabs.Add(OtherClient);//添加到 其他用户预制体 的列表里

                        OtherClient.transform.position = allMessage.Clients[i].ClientPosition;
                        OtherClient.transform.eulerAngles = allMessage.Clients[i].ClientEulerAngle;

                        OtherClient.transform.localScale = allMessage.Clients[i].ClientScale;

                        OtherClient.GetComponentInChildren<ClientPrefab>().Head.position = allMessage.Clients[i].ClientHeadPosition;
                        OtherClient.GetComponentInChildren<ClientPrefab>().Head.eulerAngles = allMessage.Clients[i].ClientHeadEulerAngle;
                        
                        OtherClient.GetComponentInChildren<ClientPrefab>().LeftHand.position = allMessage.Clients[i].ClientLeftHandPosition;
                        OtherClient.GetComponentInChildren<ClientPrefab>().LeftHand.eulerAngles = allMessage.Clients[i].ClientLeftHandEulerAngle;

                        OtherClient.GetComponentInChildren<ClientPrefab>().RightHand.position = allMessage.Clients[i].ClientRightHandPosition;
                        OtherClient.GetComponentInChildren<ClientPrefab>().RightHand.eulerAngles = allMessage.Clients[i].ClientRightHandEulerAngle;

                    }
                }
            }
            else
            {
                //主函数，用来接收服务器返回的列表内所有消息并分发给各个客户端
                for (int i = 0; i < allMessage.Clients.Count; i++)
                {
                    foreach (var item in otherClientsPrefabs)
                    {
                        if (item.name == (allMessage.Clients[i].ClientName))
                        {
                            //var IK = item.GetComponent<BipedIK>();

                            //IK.solvers.pelvis.position = allMessage.Clients[i].ClientPosition;
                            //IK.solvers.pelvis.rotation = allMessage.Clients[i].ClientEulerAngle;

                            item.transform.DOMove(allMessage.Clients[i].ClientPosition, sendInterval).SetEase(Ease.Linear);
                            item.transform.DORotate(allMessage.Clients[i].ClientEulerAngle, sendInterval);

                            var newClientPrefab=item.GetComponentInChildren<ClientPrefab>();

                            newClientPrefab.Head.DOMove(allMessage.Clients[i].ClientHeadPosition,sendInterval).SetEase(Ease.Linear);
                            newClientPrefab.Head.DORotate(allMessage.Clients[i].ClientHeadEulerAngle, sendInterval);

                            newClientPrefab.LeftHand.DOMove(allMessage.Clients[i].ClientLeftHandPosition, sendInterval).SetEase(Ease.Linear);
                            newClientPrefab.LeftHand.DORotate(allMessage.Clients[i].ClientLeftHandEulerAngle, sendInterval);

                            newClientPrefab.RightHand.DOMove(allMessage.Clients[i].ClientRightHandPosition, sendInterval).SetEase(Ease.Linear);
                            newClientPrefab.RightHand.DORotate(allMessage.Clients[i].ClientRightHandEulerAngle, sendInterval);
                        }
                    }
                }
            }
            ClientNum = allMessage.Clients.Count;//给判断用户参数有几个的值 赋值

            /*if(getSequenceNumber < allMessage.sendSequenceNumber)
            {
                getSequenceNumber=allMessage.sendSequenceNumber;
            }*/

            //FootBall.position= allMessage.Objects.FootBall.ObjectPosition;
            //FootBall.eulerAngles= allMessage.Objects.FootBall.ObjectPosition;

            //实例化公共物体
            /*if(isFirst)//如果是第一次连接
            {
                var blueBall = Instantiate(BlueBall);//新建蓝球
                var yellowBall = Instantiate(YellowBall);//新建黄球
                publicObjectsPrefabs.Add(blueBall);
                publicObjectsPrefabs.Add(yellowBall);
                isFirst = false;
            }*/


            /*blueBall.DOMove(allMessage.PublicObjects.BlueBall.BallPosition, sendInterval).SetEase(Ease.Linear);
            blueBall.DORotate(allMessage.PublicObjects.BlueBall.BallEulerAngle, sendInterval);

            yellowBall.DOMove(allMessage.PublicObjects.YellowBall.BallPosition, sendInterval).SetEase(Ease.Linear);
            yellowBall.DORotate(allMessage.PublicObjects.YellowBall.BallEulerAngle, sendInterval);*/
        }

        public GameObject GetGameObject(string name)
        {
            if (name == "Girl")
            {
                return Instantiate(OtherClientPrefabs_Girl);//新建其他用户
            }
            else if (name == "RedGirl")
            {
                return Instantiate(OtherClientPrefabs_RedGirl);//新建其他用户
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据 全局变量 判断用户选择的什么角色
        /// </summary>
        public void GetAllCharacters()
        {
            foreach (var item in AllCharacters)
            {
                item.gameObject.SetActive(false);
            }

            for (int i = 0; i < AllCharacters.Count; i++)
            {
                if (ClientDatas.SelectedCharacter != null && AllCharacters[i].name == ClientDatas.SelectedCharacter)
                {
                    ChoosePlayer = AllCharacters[i];
                    ClientDatas.SelectedCharacterNum = i;
                    ChoosePlayer.gameObject.SetActive(true);
                    break;
                }
            }
        }
        //在停止播放时调用此方法来取消异步操作并关闭连接
        public void StopAsyncOperationsAndCloseConnection()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(); // 取消异步操作

                // 等待异步操作取消完成
                while (!cancellationTokenSource.Token.IsCancellationRequested) { }

                cancellationTokenSource.Dispose();
            }

            if (client != null)
            {
                client.Close(); //关闭UDP客户端
                client.Dispose();
            }
        }

        //在停止播放时调用此方法来取消异步操作并关闭连接
        public void StopAsyncOperationsAndCloseConnection2()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(); // 取消异步操作

                // 等待异步操作取消完成
                while (!cancellationTokenSource.Token.IsCancellationRequested) { }

                cancellationTokenSource.Dispose();
            }

            if (client != null)
            {
                client.Close(); //关闭UDP客户端
                client.Dispose();
            }
        }
        /// <summary>
        /// 异步 发送当前用户信息
        /// </summary>
        public async Task SendPosition(Action action = null)
        {
            //ClientInit(state);
            //ObjectsInit();
            action?.Invoke();

            string jsonMessage = JsonUtility.ToJson(clientAttribute);
            //string jsonMessage = JsonUtility.ToJson(message);
            byte[] data = Encoding.UTF8.GetBytes(jsonMessage);
            //Debug.Log(jsonMessage);
            //Debug.Log(clientAttribute.sequenceNumber);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(_IpAddress), ClientDatas.serverPort);

            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await client.SendAsync(data, data.Length, serverEndPoint);
                await ReceiveData(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                //StopAsyncOperationsAndCloseConnection();
            }
        }
        /// <summary>
        /// 发送公共物体数据并接受
        /// </summary>
        /// <returns></returns>
        public async Task SendPublicObject()
        {
            Debug.Log("发送公共物体数据");
            string jsonMessage = JsonUtility.ToJson(footBallJoinServer.publicObjects);
            //string jsonMessage = JsonUtility.ToJson(message);
            byte[] data = Encoding.UTF8.GetBytes(jsonMessage);
            Debug.Log(jsonMessage);
            //Debug.Log(clientAttribute.sequenceNumber);
            //serverEndPoint = new IPEndPoint(IPAddress.Parse(_IpAddress), ClientDatas.serverPort);

            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                Debug.Log("这里有上传数据");
                await client.SendAsync(data, data.Length, serverEndPoint);//需要等待
                //await ReceivePublicObjectData(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                //StopAsyncOperationsAndCloseConnection();
            }
        }

        /// <summary>
        /// 接受公共物体消息（实时）
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ReceivePublicObjectData(CancellationToken cancellationToken)
        {
            Debug.Log("接受公共物体消息");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult receiveResult = await client.ReceiveAsync();
                    byte[] receivedData = receiveResult.Buffer;
                    string receivedJson = Encoding.UTF8.GetString(receivedData);

                    // 在主线程中处理收到的数据
                    if (Application.isPlaying)
                    {
                        PublicObjectReceive(receivedJson);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveData(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult receiveResult = await client.ReceiveAsync();
                    byte[] receivedData = receiveResult.Buffer;
                    string receivedJson = Encoding.UTF8.GetString(receivedData);

                    // 在主线程中处理收到的数据
                    if (Application.isPlaying)
                    {
                        ClientReceive(receivedJson);
                        PublicObjectReceive(receivedJson);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 确定用户身高
        /// </summary>
        void CertainHeight()
        {
            //期间更改模型大小
            //（头显位置/模型本身高度-Head差值）
            var newScale = (MainCamera.position.y / HeightItem[ClientDatas.SelectedCharacterNum].position.y - Head[ClientDatas.SelectedCharacterNum].localPosition.y) * Model[ClientDatas.SelectedCharacterNum].localScale.y;
            Model[ClientDatas.SelectedCharacterNum].localScale = new Vector3(newScale, newScale, newScale);
        }

        private void OnDestroy()
        {
            ClientClose();
        }
    }
}
