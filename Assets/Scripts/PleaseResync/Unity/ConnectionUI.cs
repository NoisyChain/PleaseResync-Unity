using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            uint finalPlayerCount = PlayerCount.text.Trim().Length > 0 ? uint.Parse(PlayerCount.text) : 2;
            uint finalPlayerID = PlayerID.text.Trim().Length > 0 ? uint.Parse(PlayerID.text) : 0;
            manager.CreateConnections(CreateAdressList(), CreatePortList());
            manager.OnlineGame(finalPlayerCount, finalPlayerID);
            ConnectionMenuObject.SetActive(false);
            ConnectedMenuObject.SetActive(true);
            Debug.Log("Online game started.");
        }

        private void StartLocalGame()
        {
            uint finalPlayerCount = PlayerCount.text.Trim().Length > 0 ? uint.Parse(PlayerCount.text) : 2;
            manager.LocalGame(finalPlayerCount);
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
                string port = connectionAdresses[0].PortField.text.Trim();
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


