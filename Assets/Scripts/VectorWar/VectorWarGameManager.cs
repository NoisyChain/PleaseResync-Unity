using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync.Unity;

public class VectorWarGameManager : PleaseResyncManager
{
    // Start is called before the first frame update
    void Start()
    {
        StartGame(new VectorWar(4));
    }
}
