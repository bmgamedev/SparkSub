using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour {

    private Rigidbody2D rb;

	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(18.0f, 0.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "TorpedoBarrier") {
            //trigger explosion
            Destroy(this.gameObject);
        }
    }
}
