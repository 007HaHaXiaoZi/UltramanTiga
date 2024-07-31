using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace YZZ
{
    public class ClientUI : MonoBehaviour
    {
        public Button JoinServerBtn;
        public InputField InputServerAddress;

        public Text DebugText;
        void Start()
        {
            JoinServerBtn.onClick.AddListener(() => {
                FindObjectOfType<JoinServer>().JoinServerFunction();
                //StartCoroutine(FindAnyObjectByType<JoinServer>().JoinServerFunction());
            });
            //StartCoroutine(DelayFunction());
        }
    }

}