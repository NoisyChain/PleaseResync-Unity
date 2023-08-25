using System.Collections;
using System.Collections.Generic;
using PleaseResync.Unity;
using UnityEngine;

public class VectorWarBulletViewer : MonoBehaviour
{
    public void UpdateVisuals(Bullet bullet)
    {
        transform.position = bullet.position;
        transform.right = bullet.velocity;
    }
}