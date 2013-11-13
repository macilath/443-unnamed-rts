﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class GameManager : MonoBehaviour {

    public List<GameObject> PlayerShips = new List<GameObject>();
    public List<GameObject> EnemyShips = new List<GameObject>();

    public Stopwatch alertStopwatch = new Stopwatch();
    private int alertWindow = 60000; //in ms, 1 min 
    public int survivingShips = 0;
    public int hackedStations = 0;
    public bool hackerAlive = true;

    void Start()
    {
        AddShips();
    }

    void Update()
    {
        if (alertStopwatch.ElapsedMilliseconds >= alertWindow) 
        {
            if (Application.loadedLevelName != "Level1")
            {
                alertStopwatch.Reset();
            }
            else
            {
                alertStopwatch.Reset();
                Application.LoadLevel("L1Loss");
            }
        }
        if (PlayerShips.Count == 0 && Application.loadedLevelName == "Level1")
        {
            Application.LoadLevel("L1Loss");
        }
        if (!hackerAlive && Application.loadedLevelName == "Level2") //or we lose special ship
        {
			Application.LoadLevel("L2Loss");
        }
    }
	
	void AddShips() {
		if(Application.loadedLevelName == "Level1" || Application.loadedLevelName == "Level2") {
			// Add Ship object to selectable list
            GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerShip");
            for (int i = 0; i < players.Length; i++)
            {
                PlayerShips.Add(players[i]);
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyShip");
            for (int i = 0; i < enemies.Length; i++)
            {
                EnemyShips.Add(enemies[i]);
            }
            GameCamera cam = GameObject.Find("Main Camera").GetComponent<GameCamera>();
            cam.shipIndex = PlayerShips.Count;
            cam.ShipIterate();
            //Selectable.Add(PlayerShip); 
			
		}
	}
}
