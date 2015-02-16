using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DynamicPointRRT : AbstractVehicle {

	// Absolute value for maximum acceleration
	public float maxAcceleration;

	// Heuristic to use for moving
	public string heuristic;

	// Parameter for heuristic
	public float hParam;

	// How many points to use for collision detecting
	public int collisionPoints;

	// VState of start and goal
	private DynamicPointState startState;
	private DynamicPointState goalState;

	// RRT
	private RRT<DynamicPointState> rrt;

	// RRT data
	override protected Stack<Move> moves { get; set; }
	override protected float cost { get; set; }
	override protected float rrtTime { get; set; }


	// Initialize path
	override protected void LocalStart() {
		// Setting state options
		DynamicPointState.lo = lowLimitRRT;
		DynamicPointState.hi = highLimitRRT;
		DynamicPointState.maxAcc = maxAcceleration;
		DynamicPointState.heuristic = heuristic;
		DynamicPointState.hParam = hParam;
		DynamicPointState.collisionPoints = collisionPoints;

		// Set states with zero initial velocity and run rrt
		startState = new DynamicPointState(
			startPos.x, startPos.y, Vector3.zero);
		goalState = new DynamicPointState(
			goalPos.x, goalPos.y, Vector3.zero);
		DynamicPointState.goalState = goalState;
		rrt = new RRT<DynamicPointState>(
			startState,
			goalState,
			DynamicPointState.GenerateRandom,
			polys,
			iterations,
			neighborhood
		);

		// Remove the last move in the list and replace it with breaking
		DynamicPointMove last =
			rrt.moves[rrt.moves.Count-1] as DynamicPointMove;
		Vector3 lastVel = last.velocity;
		float lastTime = lastVel.magnitude / maxAcceleration;
		Move lastMove = new DynamicPointMove(lastVel,
			-lastVel.normalized * maxAcceleration, lastTime);
		moves = new Stack<Move>(
			Enumerable.Concat(
				new Move[] {lastMove},
				Enumerable.Reverse(rrt.moves).Skip(1)
			)
		);

		// Set times
		cost = rrt.cost + lastTime;
		rrtTime = rrt.runTime;
	}

	// Draws all nice gizmos and shit
	// Can be easily disabled by clicking on Gizmos icon in the scene window
	void OnDrawGizmos() {
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
			// Draw path
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
