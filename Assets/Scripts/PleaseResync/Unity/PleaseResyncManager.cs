using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PleaseResync.Unity
{
    public class PleaseResyncManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SimulationInfo;
        [SerializeField] private TextMeshProUGUI RollbackInfo;
        private bool OfflineMode;
        private bool Started;

        private const uint INPUT_SIZE = 1;
        private const ushort FRAME_DELAY = 2;
        private uint MAX_PLAYERS = 2;
        private uint DEVICE_COUNT = 2;
        private uint DEVICE_ID;

        private string[] Adresses = {"127.0.0.1", "127.0.0.1", "127.0.0.1", "127.0.0.1"};
        private ushort[] Ports = {7001, 7002, 7003, 7004};

        [HideInInspector] public IGameState sessionState;

        LiteNetLibSessionAdapter adapter;

        Peer2PeerSession session;

        byte[] LastInput;

        List<SessionAction> sessionActions;

        string InputDebug;

        public void OnDisable()
        {
            CloseGame();
        }

        private void FixedUpdate()
        {
            if (!Started) return;

            if (OfflineMode) OfflineGameLoop();
            else
            {
                if (!session.IsRunning())
                {
                    session.Poll();
                    if (SimulationInfo != null) SimulationInfo.text = "Syncing...";
                    return;
                }
                
                OnlineGameLoop();
                if (RollbackInfo != null) RollbackInfo.text = "RBF: " + session.RollbackFrames();
            }
        }

        void OnDestroy()
        {
            CloseGame();
        }

        public void CreateConnections(string[] IPAdresses, ushort[] ports)
        {
            for (uint i = 0; i < IPAdresses.Length; i++)
            {
                if (IPAdresses[i] != "") Adresses[i] = IPAdresses[i];
                if (ports[i] > 0) Ports[i] = ports[i];
            }
        }

        protected void StartOnlineGame(IGameState state, uint playerCount, uint ID)
        {
            OfflineMode = false;

            DEVICE_ID = ID;
            MAX_PLAYERS = playerCount;
            DEVICE_COUNT = playerCount;

            sessionState = state;
            
            adapter = new LiteNetLibSessionAdapter(Adresses[DEVICE_ID], Ports[DEVICE_ID]);

            session = new Peer2PeerSession(INPUT_SIZE, DEVICE_COUNT, MAX_PLAYERS, adapter);

            LastInput = new byte[INPUT_SIZE];

            session.SetLocalDevice(DEVICE_ID, 1, FRAME_DELAY);
            
            for (uint i = 0; i < DEVICE_COUNT; i++)
            {
                if (i != DEVICE_ID)
                {
                    session.AddRemoteDevice(i, 1, LiteNetLibSessionAdapter.CreateRemoteConfig(Adresses[i], Ports[i]));
                    Debug.Log($"Device {i} created");
                }
            }
            session.Poll();
            Started = true;
        }

        protected void StartOfflineGame(IGameState state, uint playerCount)
        {
            OfflineMode = true;
            sessionState = state;

            LastInput = new byte[(int)playerCount];
            if (RollbackInfo != null) RollbackInfo.text = "";

            Started = true;
        }

        private void OnlineGameLoop()
        {
            session.Poll();

            for (int i = 0; i < LastInput.Length; i++)
                LastInput[i] = sessionState.GetLocalInput(i);

            sessionActions = session.AdvanceFrame(LastInput);
            
            foreach (var action in sessionActions)
            {
                switch (action)
                {
                    case SessionAdvanceFrameAction AFAction:
                        InputDebug = InputConstructor(AFAction.Inputs);
                        sessionState.GameLoop(AFAction.Inputs);
                        break;
                    case SessionLoadGameAction LGAction:
                        MemoryStream readerStream = new MemoryStream(LGAction.Load());
                        BinaryReader reader = new BinaryReader(readerStream);
                        sessionState.Deserialize(reader);
                        break;
                    case SessionSaveGameAction SGAction:
                        MemoryStream writerStream = new MemoryStream();
                        BinaryWriter writer = new BinaryWriter(writerStream);
                        sessionState.Serialize(writer);
                        SGAction.Save(writerStream.ToArray());
                        break;
                }
            }

            if (SimulationInfo != null) 
                SimulationInfo.text = $"{sessionState.GetStateFrame()} ({session.FrameAdvantage()}) || ( {InputDebug} )";
        }

        private void OfflineGameLoop()
        {
            for (int i = 0; i < LastInput.Length; i++)
                LastInput[i] = sessionState.GetLocalInput(i);

            sessionState.GameLoop(LastInput);
            InputDebug = InputConstructor(LastInput);

            if (SimulationInfo != null) 
                SimulationInfo.text = $"{sessionState.GetStateFrame()} || ( {InputDebug} )";
        }

        public void CloseGame()
        {
            sessionState = null;
            if (adapter != null) adapter.Close();
            Started = false;
        }

        private string InputConstructor(byte[] PlayerInputs)
        {
            string finalString = " ";

            for (int i = 0; i < PlayerInputs.Length; i++)
            {
                finalString += PlayerInputs[i] + " ";
                if (i < PlayerInputs.Length - 1) finalString += ":: ";
            }

            return finalString;
        }

        public virtual void OnlineGame(uint maxPlayers, uint ID) {}

        public virtual void LocalGame(uint maxPlayers) {}
    }
}
