using System.Collections;
using System.Collections.Generic;
using PleaseResync.Unity;
using UnityEngine;
using TMPro;

public class VectorWarShipViewer : MonoBehaviour
{
    //public int ID;
    public SpriteRenderer[] PlayerColorObject;
    public Color[] playerColors = new[]{Color.red, Color.blue, Color.yellow, Color.green};
    public Transform mesh;
    public TextMeshPro positionViewer;

    public void UpdateVisuals(Ship player)
    {
        //Color finalColor = ID == 0 ? Color.red : Color.blue;
        //mesh.material.color = finalColor;
        transform.position = player.position;
        mesh.rotation = Quaternion.Euler(0, 0, player.heading);
        if (positionViewer != null) positionViewer.text = player.position.ToString();
    }

    public void ChangePlayerColor(int playerIndex)
    {
        for (int i = 0; i < PlayerColorObject.Length; i++)
        PlayerColorObject[i].color = playerColors[playerIndex];
    }
}
