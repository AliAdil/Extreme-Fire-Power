using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
[RequireComponent (typeof(Player))]
public class PlayerInput : MonoBehaviour {
    // player class refrence 
    Player player;

	void Start () {
        player = GetComponent<Player>();
	}
	
	
	void Update () {
        //Vector2 directionalInput = new Vector2(CrossPlatformInputManager.GetAxisRaw("Horizontal"), CrossPlatformInputManager.GetAxisRaw("Vertical"));
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
       player.SetDirectionalInput(directionalInput);

       if (Input.GetButtonDown("Jump"))
       {
           player.OnJumpInputDown();
       }

       if (Input.GetButtonUp("Jump"))
       {
           player.OnJumpInputUp();
       }
		
	}
}
