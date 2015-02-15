using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
	This is the class for Kinematic Point Motion Model. It is a waypoint
	model which gives optimal path using Visibility graph wit hA* search.
**/
public class KinematicMM : AbstractVehicleWaypoint {

	// Velocity of the vehicle
	public float velocity = 1.0f;

	// Maximum velocity
	public float maxVelocity = 5.0f;

	// Material for obstacles
	public Material material;

	// GUI style to use for labels
	private GUIStyle labelStyle;

	// Rectangle to use for labels
	private Rect labelRect;

	// String to print in label
	private string strCost;

	// Name of the file for poly data
	public string obstacleFilename;


	// Use this for initialization
	override protected void Start () {
		// Base call
		base.Start();
		// Check parameters
		require(maxVelocity > 0.0f, "Max velocity must be greater than 0");

		// Change size
		transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);

		// Create Visibility graph
		GraphState graph;
		IState start, goal;
		List<Polygon> polys = new List<Polygon>();
		GraphFactory.CreatePolyFromFile("Assets/_Data/" + obstacleFilename,
			out graph, out start, out goal, polys);
		
		// Generate and render all obstacles
		GameObject parent = new GameObject();
		parent.name = "Polygonal Obstacles";
		foreach (Polygon pol in polys) {
			GameObject go = pol.ToGameObject(material);
			go.transform.parent = parent.transform;
		}

		// Running the big guy and adding path to the list
		AStar ast = new AStar(graph, start, goal, AStar.HCont);
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
		strCost = "Distance: " + ast.cost.ToString("0.00");
	}

	// Prints on screen labels for cost and time
	void OnGUI() {
		string toLab = strCost
			+ "\nTime: " + Time.realtimeSinceStartup.ToString("0.00") + " s";
		if (totalTime > 0.0f) {
			toLab += "\nBest: " + totalTime.ToString("0.00") + " s";
		}
		GUI.Label(labelRect, toLab,	labelStyle);
	}

	// Moves vehicle in kinematic style, directly towards to
	// destination with constant velocity
	override protected bool MoveTo(Vector3 dest, float dt) {
		dest.y = 0.0f;			// Reset Y, it is constant

		// Velocity used to move the vehicle one step
		float transVel = Vector3.Distance(transform.position, dest);
		if (transVel < DIST_THRESHOLD) {	// At destination
			return true;
		}

		// Maximum velocity vehicle can achieve for this time step
		float maxVel = maxVelocity * dt;
		// Velocity used to translate the car
		float vel = Math.Min(transVel, maxVel);
		// Direction vector to destination
		Vector3 goalDir = Vector3.Normalize(dest - transform.position);
		// Translate the vehicle
		transform.Translate(goalDir * vel, Space.World);
		// Set public velocity, per second
		velocity = vel / dt;

		return false;
	}

}
