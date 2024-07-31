using System.Collections;
using UnityEngine;

namespace YZZ
{
    /// <summary>
    /// 给服务器发送当前位置，并实时更新所有客户端位置
    /// </summary>
    public class MoveServer : MonoBehaviour
    {
        public JoinServer joinServer;
        public FootBallJoinServer footBallJoinServer;

        private void Start()
        {
            joinServer = GetComponent<JoinServer>();
        }

        public void StartSend()
        {
            StartCoroutine(SendPos());
        }

        IEnumerator SendPos()
        {
            Debug.Log("触发协程");
            while (true)
            {
                joinServer.SendPosition(() => {
                    //joinServer.getSequenceNumber += 1;//序列号自动+1
                    joinServer.ClientInit(1);
                    //footBallJoinServer.FootBallInit();
                });
                //footBallJoinServer.SendFootBall();
                yield return new WaitForSeconds(joinServer.sendInterval);
            }
        }
    }
}
