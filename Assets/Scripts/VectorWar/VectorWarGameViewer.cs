using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PleaseResync.Unity
{
    public class VectorWarGameViewer : MonoBehaviour
    {
        public PleaseResyncManager manager;
        public VectorWarShipViewer[] Players;

        private VectorWar GameState;

        void Update()
        {
            if (manager.sessionState == null) return;

            if (GameState == null)
                GameState = (VectorWar)manager.sessionState;

            for (int i = 0; i < GameState._ships.Length; ++i)
                Players[i].UpdateVisuals(GameState._ships[i]);
        }
    }
}