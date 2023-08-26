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
            int ParsedPlayerCount = int.Parse(PlayerCount.text);
            int ParsedPlayerID = int.Parse(PlayerID.text);
            manager.OnlineGame((uint)ParsedPlayerCount, (uint)ParsedPlayerID);
            ConnectionMenuObject.SetActive(false);
            ConnectedMenuObject.SetActive(true);
            Debug.Log("Online game started.");
        }

        private void StartLocalGame()
        {
            int ParsedPlayerCount = int.Parse(PlayerCount.text);
            manager.LocalGame((uint)ParsedPlayerCount);
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
    }

    [System.Serializable]
    public class ConnectionAddress
    {
        public TMP_InputField IPField;
        public TMP_InputField PortField;
    }
}


