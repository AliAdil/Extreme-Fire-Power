using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	// Use this for initialization
    public static LevelGenerator instance;
    //all level pieces blueprints used to copy from
    public List<LevelPiece> levelPrefabs = new List<LevelPiece>();
    // starting point of the very first level piece
    public Transform levelStartPoint;
    // all level piece that are currently in the level
    public List<LevelPiece> pieces = new List<LevelPiece>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateInitialPieces();
    }
    public void GenerateInitialPieces()
    {
        for (int i = 0; i < 3; i++)
        {
            AddPiece();
        }
    }



    public void AddPiece()
    {
        //pick the random number
        int randomIndex = Random.Range(0, levelPrefabs.Count-1);

        //Instantiate copy of random level prefab and store it in piece variable
        LevelPiece piece = (LevelPiece)Instantiate(levelPrefabs[randomIndex]);
        piece.transform.SetParent(this.transform, false);

        Vector3 spawnPostion = Vector3.zero;

        //position
        if (pieces.Count == 0)
        {
            //first piece
            spawnPostion = levelStartPoint.position;
        }
        else
        {
            //take exit point from last piece as a spawn point new piece 
            spawnPostion = pieces[pieces.Count-1].exitPoint.position;
        }

        piece.transform.position = spawnPostion;
        pieces.Add(piece);
    }
    public void RemoveOldestPiece()
    {
        LevelPiece oldestPiece = pieces[0];

        pieces.Remove(oldestPiece);
        Destroy(oldestPiece.gameObject);
    }
}
