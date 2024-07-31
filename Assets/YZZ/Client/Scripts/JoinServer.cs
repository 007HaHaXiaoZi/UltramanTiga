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
        public float sendInterval = 0.2f;//�ӳ��շ�ʱ��
        
        [Header("�����ͻ���Ԥ����")]
        public GameObject OtherClientPrefabs_Girl;
        public GameObject OtherClientPrefabs_RedGirl;

        //��������
        /*public Transform blueBall;
        public Transform yellowBall;*/

        public InputField text_IpAddress;
        public string _IpAddress;

        //���Ӳ���
        public UdpClient client;
        public IPEndPoint serverEndPoint;

        int PlayerID = 0;
        int ClientNum = 0;//�ж��û����������û������ı��ʱ��
        bool isFirst = true;//�ͻ��˵�һ������


        public XROrigin xrOrigin;

        public List<Transform> xrOriginHead=new List<Transform>();

        public List<Transform> xrOriginLeftHand=new List<Transform>();

        public List<Transform> xrOriginRightHand = new List<Transform>();

        public Text text_Debug;

        private CancellationTokenSource cancellationTokenSource;
        //private CancellationTokenSource cancellationTokenSource;

        //---------------------------����---------------------------

        ClientAttribute clientAttribute;//�ͻ���

        public List<GameObject> otherClientsPrefabs = new List<GameObject>();

        //----------------���º�����Ҫ�Զ�ָ���Ķ���--------------
        [Header("�ͻ���˳�����к�")]
        public int getSequenceNumber = 0;//�ͻ������кŴ���
        [Header("��������")]
        public PublicObjects getPublicObjects;//�ӷ�����յ���_��������_����  ����
        public Transform FootBall;//��������_����
        public FootBallJoinServer footBallJoinServer;
        [Header("����ѡ�������")]
        public List<Transform> AllCharacters=new List<Transform>();


        [Header("�û�ѡ��Ľ�ɫ")]// /Resource/Characters
        public Transform ChoosePlayer;

        [Header("�û�������,����ѡ��Ľ�ɫƥ��")]
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

            OtherClientPrefabs_Girl = Resources.Load("Characters/Prefabs_Girl") as GameObject;//�����ͻ�������
            OtherClientPrefabs_RedGirl = Resources.Load("Characters/Prefabs_RedGirl") as GameObject;//�����ͻ�������

            _IpAddress = text_IpAddress.text.Trim();
            PlayerID = UnityEngine.Random.Range(0, 999);
            transform.root.name = "Player" + PlayerID;
            client = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(_IpAddress), ClientDatas.serverPort);


        }

        /// <summary>
        /// �ͻ��˳�ʼ��
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
        /// �ͻ����˳���رյ�ʱ�����
        /// </summary>
        public void ClientClose()
        {
            //�˳���ʱ����Ҫ��һ����Ϣ���߷���ˣ��޸����Ӳ���Ϊ0���ͻ��˲�Ҫ��
            SendPosition(() => { ClientInit(0); });
            StopAsyncOperationsAndCloseConnection();
        }

        /// <summary>
        /// ������û�������ת����
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

            GetComponent<MoveServer>().StartSend();//��ʼ������Ҫ�ŵ����
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="receivedJson"></param>
        void PublicObjectReceive(string receivedJson)
        {
            AllMessage allMessage = JsonConvert.DeserializeObject<AllMessage>(receivedJson);

            if(allMessage.Objects.updateMessage)
            {
                getPublicObjects = allMessage.Objects;//���ĳ���ͻ��˷�����ײ������

                Debug.Log(getPublicObjects.updateMessage);
                Debug.Log(getPublicObjects.ClientName);
                Debug.Log(getPublicObjects.FootBall.ObjectPosition);
                Debug.Log(getPublicObjects.FootBall.ObjectEulerAngle);
                Debug.Log(getPublicObjects.FootBall.ObjectForce);

                for (int i = 0; i < allMessage.Clients.Count; i++)
                {

                    if (this.name != getPublicObjects.ClientName/*&&allMessage.Objects.FootBall.ObjectForce!=getPublicObjects.FootBall.ObjectForce*/)//֤�� �Ǵ˿ͻ��˵����������ݽ����˸���
                    {
                        Debug.Log("���������ͻ��˹������������");
                        footBallJoinServer.UpdateFootBall(getPublicObjects.FootBall.ObjectPosition, getPublicObjects.FootBall.ObjectEulerAngle, getPublicObjects.FootBall.ObjectForce);//���÷���
                        getPublicObjects.ClientName = "";
                        getPublicObjects.updateMessage = false;
                        //allMessage.Objects = getPublicObjects;//�����б��е�Objects����
                    }
                }
            }
        }

        /// <summary>
        /// ���
        /// </summary>
        /// <param name="receivedJson"></param>
        void ClientReceive(string receivedJson)
        {
            //Debug.Log(receivedJson);

            //�����л�JSON�ַ���ΪPlayerScore����
            AllMessage allMessage = JsonConvert.DeserializeObject<AllMessage>(receivedJson);
            //�ж���һ�������У��û������Ƿ���ڴ˴Σ������ڴ���������µ��û������ڼ�������

            //ʵ�������пͻ���
            if (ClientNum != allMessage.Clients.Count)//�����һ�������յ���ֵ�ʹ˴β�ͬ(�����û���/��)
            {
                foreach (var item in otherClientsPrefabs)//ɾ�������û��½�����
                {
                    Destroy(item);
                }
                otherClientsPrefabs.Clear();

                for (int i = 0; i < allMessage.Clients.Count; i++)
                {
                    if (allMessage.Clients[i].ClientName != transform.root.name && allMessage.Clients[i].ClientState == 1)//��������Ϊ��ǰ�û���������
                    {
                        /*GameObject OtherClient;
                        if (allMessage.Clients[i].ClientPrefabsName== "Prefabs_Woman")
                        {
                            OtherClient = Instantiate(OtherClientPrefabs_Girl);//�½������û�
                        }else if(allMessage.Clients[i].ClientPrefabsName == "Prefabs_RedGirl")
                        {
                            var OtherClient = Instantiate(OtherClientPrefabs_RedGirl);//�½������û�
                        }*/

                        var OtherClient = GetGameObject(allMessage.Clients[i].ClientPrefabsName);//�½������û�
                        //var OtherClient = Instantiate(OtherClientPrefabs_RedGirl);//�½������û�
                        OtherClient.name = allMessage.Clients[i].ClientName;
                        otherClientsPrefabs.Add(OtherClient);//��ӵ� �����û�Ԥ���� ���б���

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
                //���������������շ��������ص��б���������Ϣ���ַ��������ͻ���
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
            ClientNum = allMessage.Clients.Count;//���ж��û������м�����ֵ ��ֵ

            /*if(getSequenceNumber < allMessage.sendSequenceNumber)
            {
                getSequenceNumber=allMessage.sendSequenceNumber;
            }*/

            //FootBall.position= allMessage.Objects.FootBall.ObjectPosition;
            //FootBall.eulerAngles= allMessage.Objects.FootBall.ObjectPosition;

            //ʵ������������
            /*if(isFirst)//����ǵ�һ������
            {
                var blueBall = Instantiate(BlueBall);//�½�����
                var yellowBall = Instantiate(YellowBall);//�½�����
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
                return Instantiate(OtherClientPrefabs_Girl);//�½������û�
            }
            else if (name == "RedGirl")
            {
                return Instantiate(OtherClientPrefabs_RedGirl);//�½������û�
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ���� ȫ�ֱ��� �ж��û�ѡ���ʲô��ɫ
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
        //��ֹͣ����ʱ���ô˷�����ȡ���첽�������ر�����
        public void StopAsyncOperationsAndCloseConnection()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(); // ȡ���첽����

                // �ȴ��첽����ȡ�����
                while (!cancellationTokenSource.Token.IsCancellationRequested) { }

                cancellationTokenSource.Dispose();
            }

            if (client != null)
            {
                client.Close(); //�ر�UDP�ͻ���
                client.Dispose();
            }
        }

        //��ֹͣ����ʱ���ô˷�����ȡ���첽�������ر�����
        public void StopAsyncOperationsAndCloseConnection2()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(); // ȡ���첽����

                // �ȴ��첽����ȡ�����
                while (!cancellationTokenSource.Token.IsCancellationRequested) { }

                cancellationTokenSource.Dispose();
            }

            if (client != null)
            {
                client.Close(); //�ر�UDP�ͻ���
                client.Dispose();
            }
        }
        /// <summary>
        /// �첽 ���͵�ǰ�û���Ϣ
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
        /// ���͹����������ݲ�����
        /// </summary>
        /// <returns></returns>
        public async Task SendPublicObject()
        {
            Debug.Log("���͹�����������");
            string jsonMessage = JsonUtility.ToJson(footBallJoinServer.publicObjects);
            //string jsonMessage = JsonUtility.ToJson(message);
            byte[] data = Encoding.UTF8.GetBytes(jsonMessage);
            Debug.Log(jsonMessage);
            //Debug.Log(clientAttribute.sequenceNumber);
            //serverEndPoint = new IPEndPoint(IPAddress.Parse(_IpAddress), ClientDatas.serverPort);

            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                Debug.Log("�������ϴ�����");
                await client.SendAsync(data, data.Length, serverEndPoint);//��Ҫ�ȴ�
                //await ReceivePublicObjectData(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                //StopAsyncOperationsAndCloseConnection();
            }
        }

        /// <summary>
        /// ���ܹ���������Ϣ��ʵʱ��
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ReceivePublicObjectData(CancellationToken cancellationToken)
        {
            Debug.Log("���ܹ���������Ϣ");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult receiveResult = await client.ReceiveAsync();
                    byte[] receivedData = receiveResult.Buffer;
                    string receivedJson = Encoding.UTF8.GetString(receivedData);

                    // �����߳��д����յ�������
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
        /// ��������
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

                    // �����߳��д����յ�������
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
        /// ȷ���û����
        /// </summary>
        void CertainHeight()
        {
            //�ڼ����ģ�ʹ�С
            //��ͷ��λ��/ģ�ͱ���߶�-Head��ֵ��
            var newScale = (MainCamera.position.y / HeightItem[ClientDatas.SelectedCharacterNum].position.y - Head[ClientDatas.SelectedCharacterNum].localPosition.y) * Model[ClientDatas.SelectedCharacterNum].localScale.y;
            Model[ClientDatas.SelectedCharacterNum].localScale = new Vector3(newScale, newScale, newScale);
        }

        private void OnDestroy()
        {
            ClientClose();
        }
    }
}
