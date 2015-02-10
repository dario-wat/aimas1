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

	// Queue of moves
	override protected Stack<Move> moves { get; set; }

	override protected float cost { get; set; }
	RRT<KinematicPointState> rrt;

	// Runs RRT and finds path
	override protected void LocalStart() {
		KinematicPointState.maxVel = maxVel;
		startState = new KinematicPointState(startPos);
		goalState = new KinematicPointState(goalPos);
		rrt = new RRT<KinematicPointState>(
			startState, goalState, KinematicPointState.GenerateRandom, polys);
		moves = new Stack<Move>(Enumerable.Reverse(rrt.moves));
		foreach (Move m in moves) {
			Debug.Log(m);
		}
		Debug.Log(moves.Count);
		cost = 0.0f;
	}

	void OnDrawGizmos() {
		if (rrt != null) {
			Gizmos.color = Color.red;
			foreach (Edge e in rrt.edges) {
				e.GizmosDraw(Color.red);
			}
			Gizmos.color = Color.red;
			foreach (Vector3 v in rrt.vertices) {
				Gizmos.DrawSphere(v, 1);
			}
			Gizmos.color = Color.green;
			foreach (Vector3 c in rrt.corners) {
				Gizmos.DrawSphere(c, 1);
			}
		}
	}

}
