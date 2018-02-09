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
        Vector2 directionalInput = new Vector2(CrossPlatformInputManager.GetAxisRaw("Horizontal"), CrossPlatformInputManager.GetAxisRaw("Vertical"));
       player.SetDirectionalInput(directionalInput);

       if (CrossPlatformInputManager.GetButtonDown("Jump"))
       {
           player.OnJumpInputDown();
       }

       if (CrossPlatformInputManager.GetButtonUp("Jump"))
       {
           player.OnJumpInputUp();
       }
		
	}
}
