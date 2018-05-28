using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkComponents
{
    public class NetworkLobbyTest : MonoBehaviour, IObserver
    {
        public void ObserveNotices()
        {
            Notice.Instance.Observe(NoticeName.OnPlayerConnected, this);
            Notice.Instance.Observe(NoticeName.OnPlayerDisconnected, this);
        }

        public void OnNotice(string notice, params object[] param)
        {
            if (notice == NoticeName.OnPlayerConnected) {

            }

            if (notice == NoticeName.OnPlayerDisconnected) {

            }
        }

        public void RemoveNotices()
        {
            Notice.Instance.Remove(NoticeName.OnPlayerConnected, this);
            Notice.Instance.Remove(NoticeName.OnPlayerDisconnected, this);
        }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}