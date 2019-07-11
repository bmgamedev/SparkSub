using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator animator;
    AudioSource audioSource;
    public AudioClip explosionSFX;

    void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(18.0f, 0.0f);

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "TorpedoBarrier") {
            audioSource.PlayOneShot(explosionSFX);
            rb.velocity = new Vector2(0.0f, 0.0f);
            Vector3 pos = gameObject.transform.position;
            animator.transform.position = new Vector3(pos.x + (gameObject.GetComponent<Renderer>().bounds.size.x*0.5f), pos.y, pos.z);
            animator.Play("Explosion");
            Destroy(gameObject,1.0f);
        }
    }
}
