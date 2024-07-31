using System;
using System.Collections.Generic;
using UnityEngine;

namespace YZZ
{
    public class ClientDatas
    {
        //public static string IpAddress = "192.168.0.194";
        public const int serverPort = 7777; // 服务器端口

        /// <summary>
        /// 所选择的角色名称，默认是
        /// </summary>
        public static string SelectedCharacter = "Girl";
        public static int SelectedCharacterNum = 0;//默认选择角色1
    }

    [Serializable]
    public class ClientAttribute
    {
        public string ClientName = "";
        public int ClientState = 0;//0未连接，1连接

        public string ClientPrefabsName = "";//客户端中角色的名字

        public Vector3 ClientScale = new Vector3(1, 1, 1);//人物角色大小

        public Vector3 ClientPosition = new Vector3(0, 0, 0);
        public Vector3 ClientEulerAngle = new Vector3(0, 0, 0);

        public Vector3 ClientHeadPosition=new Vector3(0, 0, 0);//头部关节位置
        public Vector3 ClientHeadEulerAngle= new Vector3(0, 0, 0);//头部关节旋转

        public Vector3 ClientLeftHandPosition=new Vector3(0, 0, 0);//左手关节位置
        public Vector3 ClientLeftHandEulerAngle = new Vector3(0, 0, 0);//左手关节旋转

        public Vector3 ClientRightHandPosition = new Vector3(0, 0, 0);//右手关节位置
        public Vector3 ClientRightHandEulerAngle = new Vector3(0, 0, 0);//右手关节旋转


        /*public int sequenceNumber = 0;//客户端发送的序列号，用来代表客户端顺序的

        //公共物体
        public PublicObjects Objects = new PublicObjects();*/
    }

    [Serializable]
    public class AllMessage
    {
        public List<ClientAttribute> Clients = new List<ClientAttribute>();
        //公共物体
        public PublicObjects Objects = new PublicObjects();
        //public int sendSequenceNumber = 0;//服务端上储存的序列号
    }

    [Serializable]
    public class ObjectAttribute
    {
        public Vector3 ObjectPosition = new Vector3(0, 0, 0);
        public Vector3 ObjectEulerAngle = new Vector3(0, 0, 0);
        public Vector3 ObjectForce = new Vector3(0, 0, 0);//足球受力
    }

    /// <summary>
    /// 公共物体
    /// </summary>
    [Serializable]
    public class PublicObjects
    {
        public string ClientName = "";//判断是哪个客户端的 公共物体

        public bool updateMessage = false;//可否更新数据

        //公共物体_足球_世界坐标_初始位置和旋转
        public ObjectAttribute FootBall = new ObjectAttribute
        {
            ObjectPosition = new Vector3(0, 0, 0),
            ObjectEulerAngle = new Vector3(-0, -0, 0),
            ObjectForce = new Vector3(0, 0, 0)//受力
        };
    }
}