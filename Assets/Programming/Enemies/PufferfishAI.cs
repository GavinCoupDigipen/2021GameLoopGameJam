﻿//------------------------------------------------------------------------------
//
// File Name:	PufferfishAI.cs
// Author(s):	Gavin Cooper (gavin.cooper)
// Project:		Asteroids
// Course:		WANIC VGP
//
// Copyright © 2021 DigiPen (USA) Corporation.
//
//------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PufferfishAI : MonoBehaviour
{
    [Tooltip("The tag of the object the pufferfish is targeting")]
    public string TargetTag = "Player";
    [Tooltip("Rotation speed of pufferfish")]
    public float RotationSpeed = 0.5f;
    [Tooltip("Minimum speed of pufferfish")]
    public float MinPufferfishSpeed = 0.75f;
    [Tooltip("Maximum speed of pufferfish")]
    public float MaxPufferfishSpeed = 2;

    [Tooltip("Explosion sound")]
    public AudioClip ExplosionSound;
    [Tooltip("Prefab for the spines")]
    public GameObject Spine;
    [Tooltip("Number of spines to spawn in")]
    public int NumberOfSpines = 8;
    [Tooltip("Speed of spines")]
    public float SpineSpeed = 1;

    private GameObject Target;
    private Transform mTransform;
    private Rigidbody2D body;
    private Animator PlayerAnim;
    private AudioSource myAudio;

    private bool Exploding = false;
    private int ObjectSpawnCounter = 0;
    private bool PlayedSound = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get things
        Target = GameObject.FindGameObjectWithTag(TargetTag);
        mTransform = GetComponent<Transform>();
        body = GetComponent<Rigidbody2D>();
        PlayerAnim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //find vector pointing from object to target
        var direction = (Target.transform.position - transform.position).normalized;

        //convert vector to angle rotation
        var lookAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        //rotate
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(-lookAngle + 90, Vector3.forward), Time.deltaTime * RotationSpeed);

        //get move dirrection
        Vector2 moveDirection = new Vector2(
            Mathf.Cos(mTransform.eulerAngles.z * Mathf.Deg2Rad),
            Mathf.Sin(mTransform.eulerAngles.z * Mathf.Deg2Rad));

        // Random speed
        var speed = Random.Range(MinPufferfishSpeed, MaxPufferfishSpeed);

        // Move object
        if (!Exploding)
        {
            body.velocity = moveDirection * speed;
        }
    }

    // pufferfish collide
    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if it dirrectly collided with the player
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            return;
        }

        Explode();
    }

    // When pufferfish in in range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player
        if (collision.gameObject.tag != "Player")
        {
            return;
        }

        Explode();
    }

    private void Explode()
    {
        //turn off hit box
        GetComponent<BoxCollider2D>().enabled = false;

        //stop movement
        body.velocity = new Vector2(0, 0);
        Exploding = true;

        //start animation
        PlayerAnim.SetBool("Explode", true);

        Invoke("SpawnSpines", 1.25f);
    }

    private void SpawnSpines()
    {
        //audio
        if (!PlayedSound)
        {
            PlayedSound = true;
            Invoke("PlaySound", 0.1f);
        }

        //spawn spines
        for (; ObjectSpawnCounter < NumberOfSpines; ObjectSpawnCounter++)
        {
            // Spawn objet
            var spawnedObject = Instantiate(Spine, transform.position, Quaternion.identity);

            // Rotate object
            spawnedObject.transform.rotation = Quaternion.AngleAxis((float)360 / (float)NumberOfSpines * (float)ObjectSpawnCounter, Vector3.forward);

            // Get move dirrection
            Vector2 moveDirection = new Vector2(
                Mathf.Cos(spawnedObject.gameObject.GetComponent<Transform>().eulerAngles.z * Mathf.Deg2Rad),
                Mathf.Sin(spawnedObject.gameObject.GetComponent<Transform>().eulerAngles.z * Mathf.Deg2Rad));

            // Move object
            spawnedObject.gameObject.GetComponent<Rigidbody2D>().velocity = moveDirection * SpineSpeed;
        }

        // destory self
        Destroy(gameObject, 2);
    }

    private void PlaySound()
    {
        //audio
        myAudio.clip = ExplosionSound;
        myAudio.Play();
    }
}
