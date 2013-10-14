﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour, UnitController {

    /*
     * For Player Unit Selection:
     * We can check on mouse click if the rect contains / collides with the game object
     * Then we wait on right click (orders) to assign the unit's destination, or if another left click is detected we deselect the unit
     */
	//public Transform target; 
	public GameObject playerShip;
	private bool isSelected = false;
	public static GameManager manager; 
	public int shipSpeed = 10;
	public int shipAccel = 3;
    public Vector3 targetDest;

	void Start () {
		// For level 1 we are just looking for 1 ship
		playerShip = GameObject.Find("Ship");
        targetDest = playerShip.transform.position;
	}
	
	void Update () {
        Vector3 shipPosition = playerShip.transform.position;
		Vector3 shipPositionScreen = Camera.main.WorldToScreenPoint(shipPosition);
        
        getShipSelected(shipPositionScreen);
        setTarget();
        move(shipPosition);
	}

    void getShipSelected(Vector3 shipPosition)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isSelected == false)
            {
                print("Mouse0 detected");
                print(Input.mousePosition);
                // Create our bounding rectangle - the size of which still needs some testing/debugging
                Rect boundingRect = new Rect(Input.mousePosition.x - 75, Input.mousePosition.y - 75, 150, 150);
                // See if our Ship object is in the bounding rectangle
                if (boundingRect.Contains(shipPosition))
                {
                    print("Found object");
                    isSelected = true;
                }
            }
            else isSelected = false; // Clear out previous selection
        }
    }

    void setTarget()
    {
        // Assign movement orders to ship
        if (Input.GetMouseButtonDown(1) && isSelected == true)
        {
            targetDest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetDest.z = 0.0f;
            print("Orders: GOTO " + targetDest);
        }
    }

    void move(Vector3 shipPosition)
    {
        // Move ship
        Vector3 forceVector = (targetDest - shipPosition);
        forceVector.Normalize();
        Vector3 shipVelocity = playerShip.rigidbody.velocity;

        Rect boundingRect = new Rect(shipPosition.x, shipPosition.y, 2, 2);
        Debug.Log(shipPosition - targetDest);
        if (boundingRect.Contains(targetDest))
        {
            //playerShip.rigidbody.AddRelativeForce(-shipVelocity * playerShip.rigidbody.mass);
            playerShip.rigidbody.velocity = new Vector3(0, 0, 0);    //rigidbody.velocity = new Vector3(0, 0, 0);
            Debug.Log("Destination Reached.");
            return;
        }
        /*
        if (targetDest == shipPosition)
        {
           
            playerShip.rigidbody.velocity = new Vector3(0, 0, 0);
            Debug.Log("Destination Reached.");
            return;
        }
         * */

        if (shipVelocity.sqrMagnitude < shipSpeed)
        {
            forceVector = shipVelocity + (forceVector * shipAccel);
        }
        else
        {
            forceVector = new Vector3(0, 0, 0);
        }

        playerShip.rigidbody.AddForce(forceVector);
    }
}
