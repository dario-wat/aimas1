using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class KinematicCarRRT : AbstractVehicle {

	public float maxVel;
	public float maxPhi;
	public float L;

	// VState of start and goal
	private KinematicCarState startState;
	private KinematicCarState goalState;

	private RRT<KinematicCarState> rrt;

	override protected Stack<Move> moves { get; set; }
	override protected float cost { get; set; }
	override protected float rrtTime { get; set; }


	override protected void LocalStart() {
		// Setting state options
		KinematicCarState.maxVel = maxVel;
		KinematicCarState.maxPhi = maxPhi;
		KinematicCarState.L = L;
		KinematicCarState.lo = lowLimitRRT;
		KinematicCarState.hi = highLimitRRT;
		transform.localScale = new Vector3(1, 1, L);
		transform.rotation = Quaternion.Euler(0, 90, 0);

		startState = new KinematicCarState(startPos.x, startPos.y, Vector3.right);
		goalState = new KinematicCarState(goalPos.x, goalPos.y, Vector3.right);
		rrt = new RRT<KinematicCarState>(
			startState,
			goalState,
			KinematicCarState.GenerateRandom,
			polys,
			iterations,
			neighborhood
		);

		moves = new Stack<Move>(Enumerable.Reverse(rrt.moves));
		//moves = new Stack<Move>(new Move[] {
		//	new Move(Vector3.back, -10, 30, 3),
		//	new Move(Vector3.right, -10, 120.19f, 3.387f)
		//});

		foreach (Move m in moves) {
			print(m);
		}

		cost = rrt.cost;
		rrtTime = rrt.runTime;

		GameObject tmp = new GameObject();
		Transform tr = tmp.transform;
		tr.position = transform.position;
		tr.rotation = transform.rotation;
		List<Move> tmpMoves = new List<Move>();
		foreach (Move m in rrt.moves) {
			tmpMoves.Add(m.Copy());
		}
		foreach (Move m in tmpMoves) {
			while (m.t > 0) {
				m.MoveMe(tr, 0.1f);
				poss.Add(tr.position);
			}
		}
		Destroy(tmp);
	}

	List<Vector3> poss = new List<Vector3>();
	// Draws all nice gizmos and shit
	// Can be easily disabled by clicking on Gizmos icon in the scene window
	void OnDrawGizmos() {
		if (rrt != null) {
			Gizmos.color = Color.blue;
			foreach (Vector3 v in poss) {
				Gizmos.DrawSphere(v, 0.3f);
			}
			
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

			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(transform.position, 1);
		}
	}
	
}
