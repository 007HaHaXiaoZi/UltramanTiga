using System;
using System.Collections.Generic;
using UnityEngine;

namespace YZZ
{
    public class ClientDatas
    {
        //public static string IpAddress = "192.168.0.194";
        public const int serverPort = 7777; // �������˿�

        /// <summary>
        /// ��ѡ��Ľ�ɫ���ƣ�Ĭ����
        /// </summary>
        public static string SelectedCharacter = "Girl";
        public static int SelectedCharacterNum = 0;//Ĭ��ѡ���ɫ1
    }

    [Serializable]
    public class ClientAttribute
    {
        public string ClientName = "";
        public int ClientState = 0;//0δ���ӣ�1����

        public string ClientPrefabsName = "";//�ͻ����н�ɫ������

        public Vector3 ClientScale = new Vector3(1, 1, 1);//�����ɫ��С

        public Vector3 ClientPosition = new Vector3(0, 0, 0);
        public Vector3 ClientEulerAngle = new Vector3(0, 0, 0);

        public Vector3 ClientHeadPosition=new Vector3(0, 0, 0);//ͷ���ؽ�λ��
        public Vector3 ClientHeadEulerAngle= new Vector3(0, 0, 0);//ͷ���ؽ���ת

        public Vector3 ClientLeftHandPosition=new Vector3(0, 0, 0);//���ֹؽ�λ��
        public Vector3 ClientLeftHandEulerAngle = new Vector3(0, 0, 0);//���ֹؽ���ת

        public Vector3 ClientRightHandPosition = new Vector3(0, 0, 0);//���ֹؽ�λ��
        public Vector3 ClientRightHandEulerAngle = new Vector3(0, 0, 0);//���ֹؽ���ת


        /*public int sequenceNumber = 0;//�ͻ��˷��͵����кţ���������ͻ���˳���

        //��������
        public PublicObjects Objects = new PublicObjects();*/
    }

    [Serializable]
    public class AllMessage
    {
        public List<ClientAttribute> Clients = new List<ClientAttribute>();
        //��������
        public PublicObjects Objects = new PublicObjects();
        //public int sendSequenceNumber = 0;//������ϴ�������к�
    }

    [Serializable]
    public class ObjectAttribute
    {
        public Vector3 ObjectPosition = new Vector3(0, 0, 0);
        public Vector3 ObjectEulerAngle = new Vector3(0, 0, 0);
        public Vector3 ObjectForce = new Vector3(0, 0, 0);//��������
    }

    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public class PublicObjects
    {
        public string ClientName = "";//�ж����ĸ��ͻ��˵� ��������

        public bool updateMessage = false;//�ɷ��������

        //��������_����_��������_��ʼλ�ú���ת
        public ObjectAttribute FootBall = new ObjectAttribute
        {
            ObjectPosition = new Vector3(0, 0, 0),
            ObjectEulerAngle = new Vector3(-0, -0, 0),
            ObjectForce = new Vector3(0, 0, 0)//����
        };
    }
}