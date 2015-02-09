using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiscreteMM : AbstractVehicleWaypoint {

	// How many frames have to pass to make single move
	public int F = 20;

	// Counting frames
	private int count = 0;

	// Object to use to create obstacles
	public GameObject obstacle;

	// Which neighborhood to use
	public int neighborhood = 4;

	// Parent object for obstacles
	private GameObject parent;

	// GUI style to use for labels
	private GUIStyle labelStyle;

	// Rectangle to use for labels
	private Rect labelRect;

	// String to print in label
	private string strCost;


	// Use this for initialization
	override protected void Start () {
		// Base call
		base.Start();
		// Check arguments
		require(F > 0, "F has to be greater than 0");
		require(obstacle != null, "Obstacle GameObject has to be set");

		// It is a discrete car, make it smaller
		transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);

		// Initializes some variables for graph creation
		GraphState graph;
		IState start, goal;
		List<Vector3> obstacles = new List<Vector3>();
		// Create graph
		GraphFactory.CreateDiscreteFromFile("Assets/_Data/disc.dat",
			neighborhood, out graph, out start, out goal, obstacles);
		
		// Parent so it makes a nice structure
		parent = new GameObject("Obstacles");
		
		// Generate all obstacles
		foreach (Vector3 v in obstacles) {
			Vector3 pos = new Vector3(v.x, -0.5f, v.z);
			GameObject temp = (GameObject) 
				Instantiate(obstacle, pos, Quaternion.identity);
			temp.SetActive(true);
			temp.transform.parent = parent.transform;
		}

		// Now run astar and find path, add path to list
		AStar ast = new AStar(graph, start, goal, AStar.HDisc);
		List<Vector3> points = new List<Vector3>();
		foreach (IState s in ast.path) {
			points.Add(s.ToVector3());
		}

		// Generate path
		PathGenerator.Init(points);

		// Move to starting position
		transform.position = start.ToVector3();

		// Just to initialize and set color
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
		strCost = "Cost: " + ast.cost;
	}

	// Otputs the cost to the screen
	void OnGUI() {
		GUI.Label(labelRect, strCost, labelStyle);
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
