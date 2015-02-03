using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiscreteMM : AbstractVehicle {

	// How many frames have to pass to make single move
	public int F = 20;

	// Counting frames
	private int count = 0;

	// Object to use to create obstacles
	public GameObject obstacle;

	public int neighborhood = 4;


	// Use this for initialization
	override protected void Start () {
		// Base call
		base.Start();
		// Check arguments
		require(F > 0, "F has to be greater than 0");
		require(obstacle != null, "Obstacle GameObject has to be set");

		// It is a discrete car, make it smaller
		transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);


		//*****
		// TODO add obstacles initialization
		//****

		
		GraphState gph;
		IState st, go;
		List<Vector3> obstacles = new List<Vector3>();
		GraphFactory.CreateDiscreteFromFile(
			"Assets/_Data/disc.dat", neighborhood, out gph, out st, out go,
			obstacles);
		GameObject parent = new GameObject("Obstacles");
		
		foreach (Vector3 v in obstacles) {
			Vector3 pos = new Vector3(v.x, -0.5f, v.z);
			GameObject temp = (GameObject) 
				Instantiate(obstacle, pos, Quaternion.identity);
			temp.SetActive(true);
			temp.transform.parent = parent.transform;
		}

		List<Vector3> points = new List<Vector3>();
		AStar ast = new AStar(gph, st, go);
		foreach (IState z in ast.path) {
			points.Add(z.ToVector3());
		}
		PathGenerator.Init(points);
		transform.position = points[0];
		
	}

	// This function is called every F frames, and then it moves
	// in discrete steps
	override protected bool MoveTo(Vector3 dest, float dt) {
		// To make sure
		dest.y = 0.0f;

		// Counting frames
		count++;
		if (count <= F) {
			return false;
		}
		count = 0;			// Reset counter

		// Teleport
		transform.position = dest;

		return true;
	}
}
