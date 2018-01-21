using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {
    float gravity = -20;
    float moveSpeed = 6;
    Vector2 velocity;
    Controller2D controller;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D>();
	}

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        velocity.x = input.x* moveSpeed;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
