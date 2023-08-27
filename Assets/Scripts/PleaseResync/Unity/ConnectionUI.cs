using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PleaseResync;
using Unity.VisualScripting;

namespace PleaseResync.Unity
{
    public class ConnectionUI : MonoBehaviour
    {
        private const uint MAX_CONNECTIONS = 4;
        private PleaseResyncManager manager;
        public ConnectionAddress[] connectionAdresses;
        public TMP_InputField PlayerCount;
        public TMP_InputField PlayerID;
        public Button ConnectGameButton;
        public Button LocalGameButton;
        public Button CloseGameButton;
        public GameObject ConnectionMenuObject;
        public GameObject ConnectedMenuObject;

        private void Start()
        {
            manager = FindObjectOfType<PleaseResyncManager>();
            ConnectGameButton.onClick.AddListener(() => StartOnlineGame());
            LocalGameButton.onClick.AddListener(() => StartLocalGame());
            CloseGameButton.onClick.AddListener(() => CloseGame());
        }

        private void StartOnlineGame()
        {
            manager.CreateConnections(CreateAdressList(), CreatePortList());
            manager.OnlineGame(uint.Parse(PlayerCount.text), uint.Parse(PlayerID.text));
            ConnectionMenuObject.SetActive(false);
            ConnectedMenuObject.SetActive(true);
            Debug.Log("Online game started.");
        }

        private void StartLocalGame()
        {
            manager.LocalGame(uint.Parse(PlayerCount.text));
            ConnectionMenuObject.SetActive(false);
            ConnectedMenuObject.SetActive(true);
            Debug.Log("Local game started.");
        }

        private void CloseGame()
        {
            manager.CloseGame();
            ConnectionMenuObject.SetActive(true);
            ConnectedMenuObject.SetActive(false);
            Debug.Log("Game aborted.");
        }

        private string[] CreateAdressList()
        {
            string[] temp = new string[MAX_CONNECTIONS];
            for (int i = 0; i < temp.Length; ++i)
            {
                string address = connectionAdresses[0].IPField.text.Trim();
                temp[i] = address.Length > 0 ? address : "";
            }

            return temp;
        }

        private ushort[] CreatePortList()
        {
            ushort[] temp = new ushort[MAX_CONNECTIONS];
            for (int i = 0; i < temp.Length; ++i)
            {
                string port = connectionAdresses[0].IPField.text.Trim();
                temp[i] = port.Length > 0 ? ushort.Parse(port) : (ushort)0;
            }

            return temp;
        }
    }

    [System.Serializable]
    public class ConnectionAddress
    {
        public TMP_InputField IPField;
        public TMP_InputField PortField;
    }
}


