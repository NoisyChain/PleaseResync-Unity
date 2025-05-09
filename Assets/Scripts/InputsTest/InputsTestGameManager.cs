using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync;

public class InputsTestGameManager : PleaseResyncManager
{
    protected PlayerInputs controls;

    new void Awake()
    {
        RecordedInputs.Add(new ReplayInputs(new byte[0]));

        controls = new PlayerInputs();

        if (SimulationInfo != null) SimulationInfo.text = "";
        if (RollbackInfo != null) RollbackInfo.text = "";
        if (PingInfo != null) PingInfo.text = "";
    }
    
    public void OnEnable()
    {
        if (controls != null) controls.Enable();
    }

    public new void OnDisable()
    {
        CloseGame();
        if (controls != null) controls.Disable();
    }

    public override void OnlineGame(bool spectate, uint players, uint spectators, uint ID)
    {
        StartOnlineGame(new TestGameState(players, controls), spectate, players, spectators, ID);
    }

    public override void LocalGame(uint players)
    {
        StartOfflineGame(new TestGameState(players, controls), players);
    }

    public override void ReplayMode(uint players)
    {
        StartReplay(new TestGameState(players, controls), players);
    }
}
