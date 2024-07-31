using System.Collections;
using UnityEngine;

namespace YZZ
{
    /// <summary>
    /// �����������͵�ǰλ�ã���ʵʱ�������пͻ���λ��
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
            Debug.Log("����Э��");
            while (true)
            {
                joinServer.SendPosition(() => {
                    //joinServer.getSequenceNumber += 1;//���к��Զ�+1
                    joinServer.ClientInit(1);
                    //footBallJoinServer.FootBallInit();
                });
                //footBallJoinServer.SendFootBall();
                yield return new WaitForSeconds(joinServer.sendInterval);
            }
        }
    }
}
