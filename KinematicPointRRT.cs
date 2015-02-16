using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KinematicPointRRT : AbstractVehicle {

	// Max velocity
	public float maxVel;

	// VState of start and goal
	private KinematicPointState startState;
	private KinematicPointState goalState;

	// Reference to RRT to get all the needed data
	private RRT<KinematicPointState> rrt;

	// Stack of moves
	override protected Stack<Move> moves { get; set; }

	// The cost of the trip
	override protected float cost { get; set; }

	// RRT runtime
	override protected float rrtTime { get; set; }
	

	// Runs RRT and finds path
	override protected void LocalStart() {
		// Setting state options
		KinematicPointState.maxVel = maxVel;
		KinematicPointState.lo = lowLimitRRT;
		KinematicPointState.hi = highLimitRRT;

		// Initialize and run RRT
		startState = new KinematicPointState(startPos);
		goalState = new KinematicPointState(goalPos);
		rrt = new RRT<KinematicPointState>(
			startState,
			goalState,
			KinematicPointState.GenerateRandom,
			polys,
			iterations,
			neighborhood
		);
		// Create a stack of moves, very important to use stack
		moves = new Stack<Move>(Enumerable.Reverse(rrt.moves));

		// Set cost and init time which will be printed to screen later
		cost = rrt.cost;
		rrtTime = rrt.runTime;
		Debug.Log("Time: " + cost + "  RRT: " + rrtTime);
	}

	// Check requirements
	override protected void LocalRequirements() {
		require(maxVel > 0.0f, "Maximum velocity has to be positive");
	}

	// Draws all nice gizmos and shit
	// Can be easily disabled by clicking on Gizmos icon in the scene window
	new void OnDrawGizmos() {
		base.OnDrawGizmos();
		if (rrt != null) {
			// Draw edges
			if (gizmosEdges) {
				Gizmos.color = Color.red;
				foreach (Edge e in rrt.edges) {
					e.GizmosDraw(Color.red);
				}
			}
			// Draw vertices
			if (gizmosVertices) {
				Gizmos.color = Color.red;
				foreach (Vector3 v in rrt.vertices) {
					Gizmos.DrawSphere(v, 1);
				}
			}
			// Draw path edges and vertices
			if (gizmosPath) {
				Gizmos.color = Color.green;
				foreach (Vector3 c in rrt.corners) {
					Gizmos.DrawSphere(c, 1);
				}
				foreach (Edge e in rrt.pathEdges) {
					e.GizmosDraw(Color.green);
				}
			}
		}
	}

}
