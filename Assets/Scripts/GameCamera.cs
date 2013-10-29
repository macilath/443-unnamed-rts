﻿using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    public Camera camera;
	public float moveSpeed;
    public static float y_bounds;
    public static float x_bounds;
    private GameObject nebula;
    private GameObject dust;
    private int nebulaMoveFraction;
    private int dustMoveFraction;
    private int edgeSensitivity;
	
	void Start()
	{
		camera = GetComponent<Camera>();
        nebula = GameObject.Find("Nebula");
        dust = GameObject.Find("SpaceDust");
        moveSpeed = 0.2f;
        y_bounds = 90;
        x_bounds = 30;
        nebulaMoveFraction = 4;
        dustMoveFraction = 2;
        edgeSensitivity = 5;
	}
	
	void FixedUpdate()
	{
        float x_pos = camera.transform.position.x;
        float y_pos = camera.transform.position.y;
        float h_axis = Input.GetAxisRaw("Horizontal");
        float v_axis = Input.GetAxisRaw("Vertical");

        while(x_pos > x_bounds)
        {
            x_pos = camera.transform.position.x;
            Vector3 movement = new Vector3(x_bounds - x_pos, 0, 0);
            camera.transform.Translate(movement);
        }
        while(x_pos < -x_bounds)
        {
            x_pos = camera.transform.position.x;
            Vector3 movement = new Vector3(-x_bounds - x_pos, 0, 0);
            camera.transform.Translate(movement);
        }
        while(y_pos > y_bounds)
        {
            y_pos = camera.transform.position.y;
            Vector3 movement = new Vector3(0, y_bounds - y_pos, 0);
            camera.transform.Translate(movement);
        }
        while(y_pos < -y_bounds)
        {
            y_pos = camera.transform.position.y;
            Vector3 movement = new Vector3(0, -y_bounds - y_pos, 0);
            camera.transform.Translate(movement);
        }

        float mouse_x = Input.mousePosition.x;
        float mouse_y = Input.mousePosition.y;
        if(h_axis != 0 || mouse_x >= Screen.width - edgeSensitivity || mouse_x <= edgeSensitivity)
        {
            Vector3 movement;
            if(h_axis != 0)
            {
                movement = new Vector3((Input.GetAxisRaw("Horizontal") * moveSpeed), 0, 0);
            }
            else if (mouse_x <= edgeSensitivity)
            {
                movement = new Vector3(-moveSpeed, 0, 0);
            }
            else
            {
                movement = new Vector3(moveSpeed, 0, 0);
            }
            
            camera.transform.Translate(movement);
            if(x_pos > -x_bounds && x_pos < x_bounds)
            {
                nebula.transform.Translate(movement / nebulaMoveFraction);
                dust.transform.Translate(movement / dustMoveFraction);
            }
        }
        if(v_axis != 0 || mouse_y >= Screen.height - edgeSensitivity || mouse_y <= edgeSensitivity)
        {
            Vector3 movement;
            if(v_axis != 0)
            {
                movement = new Vector3(0, (Input.GetAxisRaw("Vertical") * moveSpeed), 0);
            }
            else if (mouse_y <= edgeSensitivity)
            {
                movement = new Vector3(0, -moveSpeed, 0);
            }
            else
            {
                movement = new Vector3(0, moveSpeed, 0);
            }

            camera.transform.Translate(movement);
            if(y_pos > -y_bounds && y_pos < y_bounds)
            {
                nebula.transform.Translate(movement / nebulaMoveFraction);
                dust.transform.Translate(movement / dustMoveFraction);
            }
        }
	}
}
