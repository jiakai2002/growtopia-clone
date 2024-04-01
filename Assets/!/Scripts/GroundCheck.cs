using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
            transform.parent.GetComponent<Player>().onGround = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
            transform.parent.GetComponent<Player>().onGround = false;
    }
}
