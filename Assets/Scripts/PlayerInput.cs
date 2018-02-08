using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Player))]
public class PlayerInput : MonoBehaviour {
    // player class refrence 
    Player player;

	void Start () {
        player = GetComponent<Player>();
	}
	
	
	void Update () {
		
	}
}
