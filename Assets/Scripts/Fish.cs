﻿using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour {
	const int fishIdling = 0;
	const int fishPuffing1 = 1;
	const int fishPuffing2 = 2;
	const int fishPuffing3 = 3;

	public int actionState = 0;
	public int shiftSpeed;
	public int jumpForce = 5000;
	public float swimmingAltitude;

	Rigidbody2D rigidBody;
	Animator animator;
	public AudioClip airJumpSound;
	public AudioClip bulbJumpSound;
	private AudioSource audioSource;

	void Awake() {
		this.rigidBody = GetComponent<Rigidbody2D>();
		this.animator = GetComponent<Animator>(); // Set fish's animation
		this.audioSource = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
		this.rigidBody.velocity = Vector2.right * shiftSpeed; // Move forward right
		this.swimmingAltitude = this.transform.position.y; // Fish's swimming altitude
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) { // Key pressed
			TriggerStateChange ();
		}
	}

	// Key pressed, trigger changing of fish's action state
	void TriggerStateChange() {
		if (actionState == fishIdling) {
			this.animator.Play ("FishPuff1");
			this.actionState = fishPuffing1;
			this.audioSource.PlayOneShot (airJumpSound);
			this.rigidBody.AddForce (Vector2.up * jumpForce); // Take a jump
		} else if (actionState == fishPuffing1) {
			this.animator.Play ("FishPuff2");
			this.actionState = fishPuffing2;
		} else if (actionState == fishPuffing2) {
			this.animator.Play ("FishPuff3");
			this.actionState = fishPuffing3;
		}
	}

	// Force the fish to idle when it enter the water
	void ForceIdling() {
		this.animator.Play ("FishIdle");
		this.actionState = fishIdling;
	}

	// Check collision between fish & bubble
	void OnTriggerStay2D(Collider2D bubble) {
		if (bubble.gameObject.tag == "Bubble" && Input.anyKeyDown) { // Fish jump & destroy bubble when key pressed
			if (this.actionState != fishPuffing3) { // Fish in state puffing3 can't puff any more
				this.rigidBody.velocity = new Vector2 (shiftSpeed, 0); // Stop speed in y axis to prevent it flying away
				this.audioSource.PlayOneShot (bulbJumpSound);
				this.rigidBody.AddForce (Vector2.up * jumpForce); // Take a jump
				GameController.instance.GetScore();
				Destroy (bubble.gameObject);
			}
		}
	}

	// Check collision between fish & barrier / ground
	void OnCollisionEnter2D(Collision2D blockingObject) {
		if (blockingObject.gameObject.tag == "Barrier") {
			GameController.instance.FishDone ();
		} else if (blockingObject.gameObject.tag == "Ground") {
			ForceIdling ();
		}
	}
}
