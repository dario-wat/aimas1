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

	private RRT<DynamicPointState> rrt;

	override protected Stack<Move> moves { get; set; }
	override protected float cost { get; set; }
	override protected float rrtTime { get; set; }


	override protected void LocalStart() {
		// Setting state options
		DynamicPointState.lo = lowLimitRRT;
		DynamicPointState.hi = highLimitRRT;
		DynamicPointState.maxAcc = maxAcceleration;
		DynamicPointState.heuristic = heuristic;
		DynamicPointState.hParam = hParam;
		DynamicPointState.collisionPoints = collisionPoints;

		startState = new DynamicPointState(startPos.x, startPos.y, Vector3.zero);
		goalState = new DynamicPointState(goalPos.x, goalPos.y, Vector3.zero);
		rrt = new RRT<DynamicPointState>(
			startState,
			goalState,
			DynamicPointState.GenerateRandom,
			polys,
			iterations,
			neighborhood
		);

		Move last = rrt.moves[rrt.moves.Count-1];
		Vector3 lastVel = last.velocity * last.speed;
		float lastTime = lastVel.magnitude / maxAcceleration;
		Move lastMove = new Move(lastVel, -lastVel.normalized,
			maxAcceleration, lastTime);
		print(lastVel);
		print(lastMove);
		moves = new Stack<Move>(
			Enumerable.Concat(
				new Move[] {lastMove},
				Enumerable.Reverse(rrt.moves).Skip(1)
			)
		);

		//transform.position = Vector3.zero;
		/*
		moves = new Stack<Move>(new Move[] {
			new Move(Vector3.right * 5, Vector3.forward, 3.0f, 5),
			new Move(Vector3.zero, Vector3.right, 1.0f, 5)

		});
		*/
		//foreach (Move m in moves) {
		//	print(m);
		//}
		// Costs
		cost = rrt.cost + lastTime;
		rrtTime = rrt.runTime;
	}

	// Draws all nice gizmos and shit
	// Can be easily disabled by clicking on Gizmos icon in the scene window
	void OnDrawGizmos() {
		if (rrt != null) {
			if (gizmosEdges) {
				Gizmos.color = Color.red;
				foreach (Edge e in rrt.edges) {
					e.GizmosDraw(Color.red);
				}
			}

			if (gizmosVertices) {
				Gizmos.color = Color.red;
				foreach (Vector3 v in rrt.vertices) {
					Gizmos.DrawSphere(v, 1);
				}
			}
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
