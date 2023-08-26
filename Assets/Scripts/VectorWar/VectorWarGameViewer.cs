using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync.Unity;

public class VectorWarGameViewer : MonoBehaviour
{
    public PleaseResyncManager manager;
    public VectorWarShipViewer shipPrefab;
    public VectorWarBulletViewer bulletPrefab;

    private VectorWarShipViewer[] ships;
    private VectorWarBulletViewer[][] bullets;

    private VectorWar GameState;

    void Update()
    {
        if (manager.sessionState == null) return;

        if (GameState == null)
            GameState = (VectorWar)manager.sessionState;

        if (ships == null)
        {
            ships = new VectorWarShipViewer[GameState._ships.Length];
            bullets = new VectorWarBulletViewer[ships.Length][];
            for (int i = 0; i < GameState._ships.Length; ++i)
            {
                ships[i] = Instantiate(shipPrefab, transform);
                ships[i].ChangePlayerColor(i);
                bullets[i] = new VectorWarBulletViewer[GameState._ships[i].bullets.Length];
                for (int j = 0; j < GameState._ships[i].bullets.Length; ++j)
                {
                    bullets[i][j] = Instantiate(bulletPrefab, transform);
                }
            }
        }
        else
        {
            for (int i = 0; i < GameState._ships.Length; ++i)
            {
                ships[i].UpdateVisuals(GameState._ships[i]);
                for (int j = 0; j < GameState._ships[i].bullets.Length; ++j)
                {
                    bullets[i][j].gameObject.SetActive(GameState._ships[i].bullets[j].active);
                    bullets[i][j].UpdateVisuals(GameState._ships[i].bullets[j]);
                }
            }
        }
    }
}