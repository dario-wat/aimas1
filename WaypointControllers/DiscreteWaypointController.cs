using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiscreteWaypointController : AbstractVehicleWaypoint {

	// How many frames have to pass to make single move
	public int F = 20;

	// Counting frames
	private int count = 0;


	// Use this for initialization
	override protected void Start () {
		// Base call
		base.Start();
		// Check arguments
		require(F > 0, "F has to be greater than 0");
		
		// It is a discrete car, make it smaller
		transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);

		// Initializes hardcoded sample
		PathGenerator.InitSample();		
	}

	// This function is called every F frames, and then it moves
	// in discrete steps
	override protected bool MoveTo(Vector3 dest, float dt) {
		// Counting frames
		count++;
		if (count <= F) {
			return false;
		}
		count = 0;			// Reset counter

		// Difference to move to
		int dx = (int) (dest.x - transform.position.x);
		int dz = (int) (dest.z - transform.position.z);
		
		// Pick the correct move and make discrete step
		// It can move in diagonal also in 1 time step
		if (dx > 0) {
			transform.Translate(Vector3.right, Space.World);
		} else if (dx < 0) {
			transform.Translate(Vector3.left, Space.World);
		}
		if (dz > 0) {
			transform.Translate(Vector3.forward, Space.World);
		} else if (dz < 0) {
			transform.Translate(Vector3.back, Space.World);
		}

		// Compute again to decide if at destination
		dx = (int) (dest.x - transform.position.x);
		dz = (int) (dest.z - transform.position.z);

		if (dx == 0 && dz == 0) {
			return true;
		}
		return false;
	}
}
