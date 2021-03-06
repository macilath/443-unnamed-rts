﻿using UnityEngine;
using System;
using System.Collections;

public class EnemySight : MonoBehaviour
{
    public float detectionRadius;           // size of detection circle
    public float detectionScale = 2;
    private float fieldOfViewAngle;           // Number of degrees, centred on up, for the enemy see.
    public bool playerInSight;                    // Whether or not the player is currently sighted.
    public bool facingPlayer;
    private SettingsManager settings;

    private float angle;

    private Vector3 directionFromPlayer;
    public Vector3 playerPos;
    private Vector3 pos;

    public Vector3 previousSighting;
    public bool sightingExists;

    void Awake()
    {
        settings = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
    }

    void Start()
    {
        switch( settings.difficultyLevel )
        {
            case 1:
            {
                fieldOfViewAngle = 75;
                break;
            }
            case 2:
            {
                fieldOfViewAngle = 110;
                break;
            }
            case 3:
            {
                fieldOfViewAngle = 180;
                break;
            }
        }
        detectionRadius = detectionScale * transform.localScale.x/2;
        playerPos = Vector3.one;
        playerInSight = false;
        sightingExists = false;
        facingPlayer = false;
        previousSighting = transform.position;
    }


    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerShip")
        {
            playerPos = other.transform.position;
            // Create a vector from the enemy to the player and store the angle between it and forward.
            directionFromPlayer = other.transform.position - transform.position;
            angle = Vector3.Angle(directionFromPlayer, transform.up);

            if( Math.Abs(angle) < 5 )
            {
                facingPlayer = true;
            }
            else
            {
                facingPlayer = false;
            }

            if ( Math.Abs(angle) < fieldOfViewAngle)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, directionFromPlayer.normalized, out hit, detectionRadius))
                {
                    Debug.DrawRay(transform.position, directionFromPlayer.normalized * (detectionRadius));
                    // ... and if the raycast hits the player...
                    if (hit.collider.gameObject.tag == "PlayerShip")
                    {
                        // ... the player is in sight.
                        playerInSight = true;
                        previousSighting = hit.collider.gameObject.transform.position;
                    }
                }
            }
            else
            {
                playerInSight = false;
            }

            /*if (playerInSight)
            {
                Debug.Log(string.Format("Angle: {0}, In Sight", angle));
            }
            else
            {
                Debug.Log(string.Format("Angle: {0}, NOT IN SIGHT", angle));
            }*/
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (playerInSight)
        {
            sightingExists = true;
            playerInSight = false;
            facingPlayer = false;
        }
    }
}