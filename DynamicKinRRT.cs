using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DynamicKinRRT : AbstractVehicle {

	// Dynamic car parameters
	public float maxAcc;
	public float maxPhi;
	public float L;

	// Upper bound
	private string heuristic = "Upper";

	// VState of start and goal
	private DynamicKinState startState;
	private DynamicKinState goalState;

	// RRT
	private RRT<DynamicKinState> rrt;

	// RRT data needed for base class
	override protected Stack<Move> moves { get; set; }
	override protected float cost { get; set; }
	override protected float rrtTime { get; set; }

	// Data points for realistic path
	private List<Vector3> poss = new List<Vector3>();


	// Requirements for kinematic car parameters
	override protected void LocalRequirements() {
		require(maxAcc > 0, "Maximum acceleration must be greater than 0");
		require(maxPhi > 0, "Maximu wheel angle must be greater than 0");
		require(L > 0, "Car length must be greater than 0");
	}

	// Initializes and runs rrt
	override protected void LocalStart() {
		// Setting state options
		DynamicKinState.maxVel = 1;
		DynamicKinState.maxPhi = maxPhi;
		DynamicKinState.L = L;
		DynamicKinState.lo = lowLimitRRT;
		DynamicKinState.hi = highLimitRRT;
		DynamicKinState.heuristic = heuristic;
		
		// Set scale and rotate to right
		transform.localScale = new Vector3(1, 1, L);
		transform.rotation = Quaternion.Euler(0, 90, 0);

		// Create states, rotation is right
		startState = new DynamicKinState(
			startPos.x, startPos.y, Vector3.right);
		goalState = new DynamicKinState(
			goalPos.x, goalPos.y, Vector3.right);
		
		// Run rrt
		rrt = new RRT<DynamicKinState>(
			startState,
			goalState,
			DynamicKinState.GenerateRandom,
			polys,
			iterations,
			neighborhood
		);

		// Recompute all moves for dynamic car
		float distance = rrt.cost;							// full path
		float accUntil = distance / 2;						// braking
		float r = L / Mathf.Tan(maxPhi * Mathf.PI / 180);	// radius
		float v = 0;										// speed
		cost = 0;											// new cost
		
		List<Move> newMoves = new List<Move>();
		foreach (Move m in rrt.moves) {
			float d = m.t;		// This move distance
			KinematicCarMove tmpMove = m as KinematicCarMove;

			if (distance > accUntil && distance - d < accUntil) {
				// Special case when in the middle move, has to start braking
				// First needs to accelerate for some time
				float d1 = distance - accUntil;		// Accelerating distance
				float time = (-v + Mathf.Sqrt(v*v + 2 * maxAcc * d1)) / maxAcc;
				
				newMoves.Add(
					new DynamicKinMove(tmpMove.velocity, v, maxAcc,
					tmpMove.omega, r, time)
				);
				v += maxAcc * time;		// update speed
				cost += time;			// update cost

				// Then needs to deccelerate
				// New direction of the car
				Vector3 newVel = Quaternion.Euler(0, tmpMove.omega * d1, 0)
					* tmpMove.velocity;
				float d2 = d - d1;		// Braking distance
				time = (-v + Mathf.Sqrt(v*v - 2 * maxAcc * d2)) / (-maxAcc);
				newMoves.Add(
					new DynamicKinMove(newVel, v, -maxAcc,
					tmpMove.omega, r, time)
				);
				v -= maxAcc * time;		// update speed
				cost += time;			// update cost

			} else {
				// Other case when it needs only to accelerate or brake
				float a = distance > accUntil ? maxAcc : -maxAcc;
				a = Mathf.Max(a, -0.5f * v*v / d + 0.0001f);   // For last brake
				float time = (-v + Mathf.Sqrt(v*v + 2 * a * d)) / a;
				//TODO there appeared to be a bug here
				newMoves.Add(
					new DynamicKinMove(tmpMove.velocity, v, a,
						tmpMove.omega, r, time)
				);
				v += a*time;			// update speed
				cost += time;			// update cost
			}

			// Reduce full distance by the distance of the last move
			distance -= d;
		}

		// Set moves and other data needed for base
		moves = new Stack<Move>(Enumerable.Reverse(newMoves));
		
		// Update times
		rrtTime = rrt.runTime;
		Debug.Log("Time: " + cost + "  RRT: " + rrtTime);

		// This part generates points for realistic path
		GameObject tmp = new GameObject();
		Transform tr = tmp.transform;
		tr.position = transform.position;
		tr.rotation = transform.rotation;
		List<Move> tmpMoves = new List<Move>();
		foreach (Move m in rrt.moves) {
			tmpMoves.Add(m.Copy());
		}
		int count = 0;
		foreach (Move m in tmpMoves) {
			while (m.t > 0) {
				m.MoveMe(tr, 0.1f);
				if (count % 10 == 0) {		// Put every 10th point
					poss.Add(tr.position);
				}
				count++;
			}
		}
		Destroy(tmp);
	}

	
	// Draws all nice gizmos and shit
	// Can be easily disabled by clicking on Gizmos icon in the scene window
	new void OnDrawGizmos() {
		base.OnDrawGizmos();
		if (rrt != null) {
			// Draws realistic path
			Gizmos.color = Color.blue;
			foreach (Vector3 v in poss) {
				Gizmos.DrawSphere(v, 0.3f);
			}
			
			// Draws edges			
			if (gizmosEdges) {
				Gizmos.color = Color.red;
				foreach (Edge e in rrt.edges) {
					e.GizmosDraw(Color.red);
				}
			}

			// Draws vertices
			if (gizmosVertices) {
				Gizmos.color = Color.red;
				foreach (Vector3 v in rrt.vertices) {
					Gizmos.DrawSphere(v, 1);
				}
			}

			// Draws vertices and edges of the path
			if (gizmosPath) {
				Gizmos.color = Color.green;
				foreach (Vector3 c in rrt.corners) {
					Gizmos.DrawSphere(c, 1);
				}
				
				foreach (Edge e in rrt.pathEdges) {
					e.GizmosDraw(Color.green);
				}
			}

			// Draws a point at the back of the car
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(transform.position, 1);
		}
	}
	
}
