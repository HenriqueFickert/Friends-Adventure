using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : PhysicsObject
{


    public enum States
    {
        WALK,
        JUMP,
        ATTACK1,
        ATTACK2,
        ATTACK3,
        SPECIALATTACK1,
        SPECIALATTACK2,
        SPECIALATTACK3,
        DASH,
        DASHATTACK,
        TAKEDAMAGE,
        COUNTER,
        DISABLE,
        DEAD
    }

    [Header("Player State")]
    public States state;

    [HideInInspector] public int id;

    [Header("Informations")]
    public float moveSpeed;
    public float jumpForce;
    public float currentHealth;
    public float maxHealth;
    public bool isDead;

    [Header("Attack Informations")]
    public int damage;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;

    public Rigidbody2D rb;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator anim;

    //Local Player
    public static PlayerController isMe;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        if (player.IsLocal)
            isMe = this;
        else
            rb2d.isKinematic = false;
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, 0);

        if (Input.GetKeyDown(KeyCode.UpArrow) && grounded)
        {
            velocity.y = jumpForce;
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();


    }

    void Die()
    {

    }
}
