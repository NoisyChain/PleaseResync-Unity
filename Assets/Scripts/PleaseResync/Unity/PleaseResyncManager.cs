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
        [SerializeField] private TextMeshProUGUI PingInfo;
        public bool OfflineMode;
        private PlayerInputs controls;

        private const uint INPUT_SIZE = 1;
        private const ushort FRAME_DELAY = 1;
        private const int MAX_PLAYERS = 4;
        private const int DEVICE_COUNT = 4;

        public int deviceID;
        public string LocalAdress = "127.0.0.1";
        public ushort[] Ports = {7001, 7002, 7003, 7004};

        [HideInInspector] public BaseGameState sessionState;

        UdpSessionAdapter adapter;

        Peer2PeerSession session;

        byte[] LastInput;

        List<SessionAction> sessionActions;

        //Queue<SessionEvent> sessionEvents;

        string InputDebug;

        void Awake()
        {
            controls = new PlayerInputs();
        }
        public void OnEnable()
        {
            //if (sessionState == null) StartOnlineGame(new TesteGameState(0, 0));
            controls.Enable();
        }
        public void OnDisable()
        {
            CloseGame();
            controls.Disable();
        }

        private void FixedUpdate()
        {
            if (OfflineMode) OfflineGameLoop();
            else
            {
                if (!session.IsRunning())
                {
                    session.Poll();
                    if (SimulationInfo != null) SimulationInfo.text = "Syncing...";
                    return;
                }

                StartCoroutine(Ping());
                
                OnlineGameLoop();
            }
        }

        void OnDestroy()
        {
            CloseGame();
        }

        IEnumerator Ping()
        {
            Ping GetPing = new Ping("127.0.0.1");
            while (!GetPing.isDone) yield return null;
            if (PingInfo != null) PingInfo.text = "Ping: " + GetPing.time.ToString() + " ms";
        }

        protected void StartOnlineGame(BaseGameState state)
        {
            sessionState = state;
            
            sessionState.controls = controls;

            adapter = new UdpSessionAdapter(Ports[deviceID]);

            session = new Peer2PeerSession(INPUT_SIZE, DEVICE_COUNT, MAX_PLAYERS, adapter);

            LastInput = new byte[INPUT_SIZE];

            session.SetLocalDevice((uint)deviceID, 1, FRAME_DELAY);
            
            for (int i = 0; i < MAX_PLAYERS; i++)
                if (i != deviceID) session.AddRemoteDevice((uint)i, 1, UdpSessionAdapter.CreateRemoteConfig(LocalAdress, Ports[i]));
            
            session.Poll();
        }

        public void StartGame(BaseGameState state)
        {
            if (OfflineMode) StartOfflineGame(state);
            else StartOnlineGame(state);
        }

        protected void StartOfflineGame(BaseGameState state)
        {
            sessionState = state;
            sessionState.controls = controls;

            LastInput = new byte[2];
            if (PingInfo != null) PingInfo.text = "";
        }

        private void OnlineGameLoop()
        {
            session.Poll();

            //for (int i = 0; i < MAX_PLAYERS; i++)
            for (int i = 0; i < LastInput.Length; i++)
                LastInput[i] = sessionState.GetLocalInput(i);

            //sessionEvents = session.Events();

            sessionActions = session.AdvanceFrame(LastInput);
            
            /*while (sessionEvents.Count > 0)
            {
                Debug.Log(sessionEvents.Dequeue().Desc());
            }*/

            foreach (var action in sessionActions)
            {
                switch (action)
                {
                    case SessionAdvanceFrameAction AFAction:
                        InputDebug = InputConstructor(AFAction.Inputs);
                        sessionState.Update(AFAction.Inputs);
                        break;
                    case SessionLoadGameAction LGAction:
                        MemoryStream readerStream = new MemoryStream(LGAction.Load());
                        BinaryReader reader = new BinaryReader(readerStream);
                        sessionState.Load(reader);
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
                SimulationInfo.text = $"{sessionState.frame} ({session.FrameAdvantage()}) || ( {InputDebug} )";
        }

        private void OfflineGameLoop()
        {
            for (int i = 0; i < 2; i++)
                LastInput[i] = sessionState.GetLocalInput(i);

            sessionState.Update(LastInput);

            if (SimulationInfo != null) 
                SimulationInfo.text = $"{sessionState.frame} || ( {LastInput[0]} :: {LastInput[1]} )";
        }

        private void CloseGame()
        {
            sessionState = null;
            if (adapter != null) adapter.Close();
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
    }
}
