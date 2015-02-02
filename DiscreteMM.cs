using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiscreteMM : AbstractVehicle {

	// How many frames have to pass to make single move
	public int F = 60;

	// Counting frames
	private int count = 0;


	// Use this for initialization
	override protected void Start () {
		// Base call
		base.Start();
		// Check arguments
		require(F > 0, "F has to be greater than 0");

		// TODO add obstacles initialization
		GraphState gph;
		IState st, go;
		GraphFactory.CreateDiscreteFromFile(
			"Assets/_Data/disc.dat", GraphFactory.N4, out gph, out st, out go);
		

		List<Vector3> points = new List<Vector3>();
		AStar ast = new AStar(gph, st, go);
		foreach (IState z in ast.path) {
			points.Add(z.ToVector3());
		}
		PathGenerator.Init(points);
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
		if (dx > 0) {
			transform.Translate(Vector3.right, Space.World);
		} else if (dx < 0) {
			transform.Translate(Vector3.left, Space.World);
		} else if (dz > 0) {
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
