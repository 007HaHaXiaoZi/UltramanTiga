using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using YZZ;
namespace YZZ
{
    /// <summary>
    /// ֻ�����ϴ��������ݣ��հ�����
    /// </summary>
    public class FootBallJoinServer : MonoBehaviour
    {
        public JoinServer joinServer;

        public Vector3 footBallForce;//��������

        private Rigidbody rb;

        //��Ҫ���͸�����˵Ĺ�����������
        public PublicObjects publicObjects;
        private void Start()
        {
            rb=GetComponent<Rigidbody>();

            /*publicObjects = new PublicObjects()
            {
                ClientName = joinServer.gameObject.name,

                FootBall = new ObjectAttribute
                {
                    ObjectPosition = transform.position,
                    ObjectEulerAngle = transform.eulerAngles,
                    ObjectForce = footBallForce,
                }
            };*/

        }

        /*private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                footBallForce = contact.normal * rb.mass * Physics.gravity.magnitude;
                Debug.Log("Detected force: " + footBallForce);
            }
        }*/
        /// <summary>
        /// ������ײ�����ʱ�򣬴ӷ�������������
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if(collision.transform.root.tag=="Player")//���û�
                {
                    footBallForce = contact.normal * rb.mass * Physics.gravity.magnitude;

                    publicObjects.ClientName = joinServer.gameObject.name;
                    publicObjects.updateMessage = true;
                    publicObjects.FootBall.ObjectPosition = transform.position;
                    publicObjects.FootBall.ObjectEulerAngle = transform.eulerAngles;
                    publicObjects.FootBall.ObjectForce = footBallForce;

                    joinServer.SendPublicObject();//���ʹ�ʱ������
                }
                //publicObjects = joinServer.getPublicObjects;//�ӷ�����������������

                //���յ��������Թ�������ķ���
                //footBallJoinServer.UpdateFootBall(joinServer..FootBall.ObjectPosition, allMessage.Objects.FootBall.ObjectEulerAngle, allMessage.Objects.FootBall.ObjectForce);
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void UpdateFootBall(Vector3 pos,Vector3 eul,Vector3 force)
        {
            //StopFootBall();
            //rb.WakeUp();
            transform.position =pos;
            //transform.DOMove(pos, 0.1f).SetEase(Ease.Linear);
            transform.eulerAngles = eul;
            rb.AddForce(force);
        }

        //������������
        public void StopFootBall()
        {
            rb.velocity = Vector3.zero;      // ������ٶ�
            rb.angularVelocity = Vector3.zero; // ������ٶ�
            rb.Sleep();                      // �����������
        }
    }
}
