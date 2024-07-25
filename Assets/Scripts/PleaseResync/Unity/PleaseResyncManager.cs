using UnityEngine;
using TMPro;
//using SakugaEngine.Utils;
using System.IO;
using System.Collections.Generic;
using System.Net.NetworkInformation;
//using SakugaEngine.Game;
using System.Threading;

namespace PleaseResync
{
    public partial class PleaseResyncManager : MonoBehaviour
    {
        [SerializeField] private bool SyncTest;
        [SerializeField] private TextMeshProUGUI SimulationInfo;
        [SerializeField] private TextMeshProUGUI RollbackInfo;
        [SerializeField] private TextMeshProUGUI PingInfo;

        protected PlayerInputs controls;

        private bool Started;
        private bool Replay;
        private const uint INPUT_SIZE = 1;
        private const ushort FRAME_DELAY = 2;
        private uint MAX_PLAYERS = 2;
        private uint DEVICE_COUNT = 2;
        private uint DEVICE_ID;

        private string[] Adresses = {"127.0.0.1", "127.0.0.1", "127.0.0.1", "127.0.0.1"};
        private ushort[] Ports = {7001, 7002, 7003, 7004};

        public IGameState sessionState;

        LiteNetLibSessionAdapter adapter;
        Session session;
        PlayerInput[] LastInput;
        List<SessionAction> sessionActions;
        string InputDebug;
        string SimulationText;
        List<ReplayInputs> RecordedInputs = new List<ReplayInputs>();

        Thread PingThread;

        System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
        PingReply r;

        private void StartPinging(string pingIP)
        {
            PingThread = new Thread(() => Ping(pingIP));
            PingThread.IsBackground = true;
            PingThread.Start();
        }

        private void Ping(string PingIP)
        {
            while(Started)
                r = p.Send(PingIP);
        }

        private string ShowPingInfo()
        {
            if (r == null) return "";
            if (r.Status != IPStatus.Success) return "";
            return $"Ping: {r.RoundtripTime} ms";
        }

        public void Awake()
        {
            //SimulationInfo = GetTree().Root.GetNode<Label>("Root/CanvasLayer/PleaseResync_UI/Connection_Status/Simulation_Info");
            //RollbackInfo = GetTree().Root.GetNode<Label>("Root/CanvasLayer/PleaseResync_UI/Connection_Status/Rollback_Info");
            //PingInfo = GetTree().Root.GetNode<Label>("Root/CanvasLayer/PleaseResync_UI/Connection_Status/Ping_Info");
            RecordedInputs.Add(new ReplayInputs(new PlayerInput[0]));

            if (SimulationInfo != null) SimulationInfo.text = "";
            if (RollbackInfo != null) RollbackInfo.text = "";
            if (PingInfo != null) PingInfo.text = "";

            controls = new PlayerInputs();
        }

        public void OnEnable()
        {
            controls.Enable();
        }

        public void OnDisable()
        {
            CloseGame();
            controls.Disable();
        }

        public void FixedUpdate()
        {
            if (!Started) return;

            if (SimulationInfo != null) SimulationInfo.text = NotificationText();

            if (!session.IsOffline())
            {
                session.Poll();

                if (!session.IsRunning()) return;
                
                if (RollbackInfo != null) RollbackInfo.text = "RBF: " + session.RollbackFrames();
                if (PingInfo != null) PingInfo.text = ShowPingInfo();
            }

            GameLoop();
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
            string tempIP = "";

            DEVICE_ID = ID;
            MAX_PLAYERS = playerCount;
            DEVICE_COUNT = playerCount;

            sessionState = state;

            sessionState.Setup();
            
            adapter = new LiteNetLibSessionAdapter(Adresses[DEVICE_ID], Ports[DEVICE_ID]);

            session = new Peer2PeerSession(INPUT_SIZE, DEVICE_COUNT, MAX_PLAYERS, false, adapter);

            LastInput = new PlayerInput[INPUT_SIZE];

            session.SetLocalDevice(DEVICE_ID, 1, FRAME_DELAY);
            
            for (uint i = 0; i < DEVICE_COUNT; i++)
            {
                if (i != DEVICE_ID)
                {
                    session.AddRemoteDevice(i, 1, LiteNetLibSessionAdapter.CreateRemoteConfig(Adresses[i], Ports[i]));
                    tempIP = Adresses[i];
                    
                    Debug.Log($"Device {i} created");
                }
            }
            
            Replay = false;
            Started = true;
            StartPinging(tempIP);
        }

        protected void StartOfflineGame(IGameState state, uint playerCount)
        {
            sessionState = state;
            sessionState.Setup();

            session = new Peer2PeerSession(INPUT_SIZE, 1, playerCount, true, null);
            LastInput = new PlayerInput[(int)playerCount];
            session.SetLocalDevice(DEVICE_ID, playerCount, FRAME_DELAY);

            if (RollbackInfo != null) RollbackInfo.text = "";
            if (PingInfo != null) PingInfo.text = "";

            Replay = false;
            Started = true;
        }

        protected void StartReplay(IGameState state, uint playerCount)
        {
            sessionState = state;
            sessionState.Setup();

            session = new Peer2PeerSession(INPUT_SIZE, 1, playerCount, true, null);
            LastInput = new PlayerInput[(int)playerCount];
            session.SetLocalDevice(DEVICE_ID, playerCount, FRAME_DELAY);

            if (RollbackInfo != null) RollbackInfo.text = "";
            if (PingInfo != null) PingInfo.text = "";

            Replay = true;
            Started = true;
        }

        private void GameLoop()
        {
            if (Replay)
                LastInput = ReadInputs(session.Frame());
            else
                for (int i = 0; i < LastInput.Length; i++)
                    LastInput[i] = SyncTest ? GetRandomInput() : sessionState.GetLocalInput(i);

            sessionActions = session.AdvanceFrame(LastInput);
            
            foreach (var action in sessionActions)
            {
                switch (action)
                {
                    case SessionAdvanceFrameAction AFAction:
                        InputDebug = InputConstructor(AFAction.Inputs);
                        sessionState.GameLoop(AFAction.Inputs);
                        if (!Replay) RecordInput(AFAction.Frame, AFAction.Inputs);
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
                        byte[] state = writerStream.ToArray();
                        SGAction.Save(state, Platform.GetChecksum(state));
                        break;
                }
            }

            string FrameCounter = session.IsOffline() ? $"{session.Frame()}" : $"{session.Frame()} ({session.FrameAdvantage()})";

            SimulationText = FrameCounter + $" || ( {InputDebug} )";

            if (Replay && session.Frame() >= RecordedInputs.Count)
                CloseGame();
        }

        private string NotificationText()
        {
            switch (session.State())
            {
                default:
                    return "";
                case 0:
                    return "Syncing...";
                case 1:
                    return SimulationText;
                case 2:
                    return "Connection lost.";
                case 3:
                    return "Fatal desync detected.";
            }
        }

        public void CloseGame()
        {
            sessionState = null;
            if (adapter != null) adapter.Close();
            Replay = false;
            Started = false;
            SimulationInfo.text = "Disconnected";
        }

        private string InputConstructor(PlayerInput[] PlayerInputs)
        {
            string finalString = " ";

            for (int i = 0; i < PlayerInputs.Length; i++)
            {
                finalString += PlayerInputs[i].ToString() + " ";
                if (i < PlayerInputs.Length - 1) finalString += ":: ";
            }

            return finalString;
        }

        private void RecordInput(int frame, PlayerInput[] inputs)
        {
            if (frame >= RecordedInputs.Count)
                RecordedInputs.Add(new ReplayInputs());
            
            RecordedInputs[frame] = new ReplayInputs(inputs);
            
        }

        private PlayerInput[] ReadInputs(int frame)
        {
            return RecordedInputs[frame].inputs;
        }

        private PlayerInput GetRandomInput()
        {
            return new PlayerInput(Platform.GetRandomUnsignedShort());
        }

        public virtual void OnlineGame(uint maxPlayers, uint ID){}
        public virtual void LocalGame(uint maxPlayers){}
        public virtual void ReplayMode(uint maxPlayers) {}
    }

    public struct ReplayInputs
    {
        public PlayerInput[] inputs;

        public ReplayInputs(PlayerInput[] i)
        {
            inputs = i;
        }
    };
}
