using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class TileDropController : MonoBehaviour
{
    public float speed;
    [HideInInspector]
    public TileClass tileDropClass;
    private Vector3 direction;
    private Vector3 initialPos;
    private float displacement;

    public void Awake()
    {
        speed = 0.0010f;
        displacement = 0.04f;
        initialPos = transform.position;
        direction = Vector3.down;
    }
    public void Update()
    {
        floatEffect();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            col.GetComponent<Inventory>().Add(tileDropClass, 1);
        }
    }
    public void floatEffect()
    {
        if (transform.position.y <= initialPos.y - displacement)
        {
            direction = Vector3.up;
        }
        else if (transform.position.y >= initialPos.y + displacement)
        {
            direction = Vector3.down;
        }
        transform.position += speed * direction;
    }
}
