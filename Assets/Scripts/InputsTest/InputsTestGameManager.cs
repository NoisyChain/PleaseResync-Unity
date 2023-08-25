using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync.Unity;

public class InputsTestGameManager : PleaseResyncManager
{
    // Start is called before the first frame update
    void Start()
    {
        StartGame(new TestGameState(0, 0, 4));
    }
}
