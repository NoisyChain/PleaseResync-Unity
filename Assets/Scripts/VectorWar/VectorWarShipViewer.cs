using System.Collections;
using System.Collections.Generic;
using PleaseResync;
using UnityEngine;
using TMPro;

namespace PleaseResync.Unity
{
    public class VectorWarShipViewer : MonoBehaviour
    {
        //public int ID;
        //public MeshRenderer mesh;
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
    }
}
