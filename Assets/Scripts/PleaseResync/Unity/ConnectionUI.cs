using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PleaseResync.Unity
{
    public class ConnectionUI : MonoBehaviour
    {
        private const uint MAX_CONNECTIONS = 4;
        private PleaseResyncManager manager;
        public ConnectionAddress[] playerConnections;
        public ConnectionAddress[] spectatorConnections;
        public TMP_InputField PlayerCount;
        public TMP_InputField PlayerID;
        public TMP_InputField SpectatorCount;
        public Button ConnectGameButton;
        public Button LocalGameButton;
        public Button ReplayButton;
        public Button CloseGameButton;
        public Toggle IsSpectatorMode;
        public GameObject SpectatorMenuObject;
        public GameObject ConnectionMenuObject;
        public GameObject ConnectedMenuObject;

        private void Start()
        {
            manager = FindObjectOfType<PleaseResyncManager>();
            ConnectGameButton.onClick.AddListener(() => StartOnlineGame());
            LocalGameButton.onClick.AddListener(() => StartLocalGame());
            ReplayButton.onClick.AddListener(() => StartReplay());
            CloseGameButton.onClick.AddListener(() => CloseGame());
        }

        private void StartOnlineGame()
        {
            uint finalPlayerCount = PlayerCount.text.Trim().Length > 0 ? uint.Parse(PlayerCount.text) : 0;
            uint finalSpectatorCount = SpectatorCount.text.Trim().Length > 0 ? uint.Parse(SpectatorCount.text) : 0;
            uint finalPlayerID = PlayerID.text.Trim().Length > 0 ? uint.Parse(PlayerID.text) : 0;
            bool spectate = IsSpectatorMode.isOn;
            manager.CreatePlayerConnections(CreateAddressesList(finalPlayerCount), CreatePortsList(finalPlayerCount));
            manager.CreateSpectatorConnections(CreateAddressesList(finalSpectatorCount, true), CreatePortsList(finalSpectatorCount, true));
            manager.OnlineGame(spectate, finalPlayerCount, finalSpectatorCount, finalPlayerID);
            ConnectionMenuObject.SetActive(false);
            SpectatorMenuObject.SetActive(false);
            ConnectedMenuObject.SetActive(true);
            Debug.Log("Online game started.");
        }

        private void StartLocalGame()
        {
            uint finalPlayerCount = PlayerCount.text.Trim().Length > 0 ? uint.Parse(PlayerCount.text) : 2;
            manager.LocalGame(finalPlayerCount);
            ConnectionMenuObject.SetActive(false);
            SpectatorMenuObject.SetActive(false);
            ConnectedMenuObject.SetActive(true);
            Debug.Log("Local game started.");
        }

        private void StartReplay()
        {
            uint finalPlayerCount = PlayerCount.text.Trim().Length > 0 ? uint.Parse(PlayerCount.text) : 2;
            manager.ReplayMode(finalPlayerCount);
            ConnectionMenuObject.SetActive(false);
            SpectatorMenuObject.SetActive(false);
            ConnectedMenuObject.SetActive(true);
            Debug.Log("Executing replay...");
        }

        private void CloseGame()
        {
            manager.CloseGame();
            ConnectionMenuObject.SetActive(true);
            SpectatorMenuObject.SetActive(true);
            ConnectedMenuObject.SetActive(false);
            Debug.Log("Game aborted.");
        }

        private string[] CreateAddressesList(uint length, bool isSpectator = false)
        {
            string[] temp = new string[length];
            for (int i = 0; i < temp.Length; ++i)
            {
                string address = "";
                if (isSpectator)
                    spectatorConnections[i].IPField.text.Trim();
                else
                    playerConnections[i].IPField.text.Trim();
                
                temp[i] = address.Length > 0 ? address : "127.0.0.1";
            }

            return temp;
        }

        private ushort[] CreatePortsList(uint length, bool isSpectator = false)
        {
            ushort[] temp = new ushort[length];
            int num = isSpectator ? 8000 : 7000;
            for (int i = 0; i < temp.Length; ++i)
            {
                string port = "";
                if (isSpectator)
                    spectatorConnections[i].PortField.text.Trim();
                else
                    playerConnections[i].PortField.text.Trim();
                temp[i] = port.Length > 0 ? ushort.Parse(port) : (ushort)(num + i + 1);
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


