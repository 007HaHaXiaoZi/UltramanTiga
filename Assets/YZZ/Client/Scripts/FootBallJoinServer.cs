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
    /// 只用来上传自身数据，收包不解
    /// </summary>
    public class FootBallJoinServer : MonoBehaviour
    {
        public JoinServer joinServer;

        public Vector3 footBallForce;//足球受力

        private Rigidbody rb;

        //需要发送给服务端的公共物体数据
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
        /// 人物碰撞进入的时候，从服务器更新数据
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if(collision.transform.root.tag=="Player")//是用户
                {
                    footBallForce = contact.normal * rb.mass * Physics.gravity.magnitude;

                    publicObjects.ClientName = joinServer.gameObject.name;
                    publicObjects.updateMessage = true;
                    publicObjects.FootBall.ObjectPosition = transform.position;
                    publicObjects.FootBall.ObjectEulerAngle = transform.eulerAngles;
                    publicObjects.FootBall.ObjectForce = footBallForce;

                    joinServer.SendPublicObject();//发送此时的数据
                }
                //publicObjects = joinServer.getPublicObjects;//从服务器更新足球数据

                //先收到服务器对公共物体的返回
                //footBallJoinServer.UpdateFootBall(joinServer..FootBall.ObjectPosition, allMessage.Objects.FootBall.ObjectEulerAngle, allMessage.Objects.FootBall.ObjectForce);
            }
        }

        /// <summary>
        /// 更新足球数据
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

        //清除足球的受力
        public void StopFootBall()
        {
            rb.velocity = Vector3.zero;      // 清除线速度
            rb.angularVelocity = Vector3.zero; // 清除角速度
            rb.Sleep();                      // 清除所有受力
        }
    }
}
