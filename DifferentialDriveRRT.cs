﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DifferentialDriveRRT : AbstractVehicle {

		
		// DifferentialDrive  parameters
		public float maxVel;
		public float maxOmega;
		public float L;
		
		
		// VState of start and goal
		private DifferentialDriveState startState;
		private DifferentialDriveState goalState;
		
		// RRT
		private RRT<DifferentialDriveState> rrt;
		
		// RRT data needed for base class
		override protected Stack<Move> moves { get; set; }
		override protected float cost { get; set; }
		override protected float rrtTime { get; set; }
		
		// Data points for realistic path
		private List<Vector3> poss = new List<Vector3>();
		
		
		// Requirements for kinematic car parameters
		override protected void LocalRequirements() {
			require(maxVel > 0, "Maximum velocity must be greater than 0");
			//require(maxPhi > 0, "Maximu wheel angle must be greater than 0");
			require(L > 0, "Car length must be greater than 0");
		}
		
		// Initializes and runs rrt
		override protected void LocalStart() {
			// Setting state options
			DifferentialDriveState.maxVel = maxVel;
			DifferentialDriveState.maxOmega = maxOmega;
			DifferentialDriveState.L = L;
			DifferentialDriveState.lo = lowLimitRRT;
			DifferentialDriveState.hi = highLimitRRT;
			
			// Set scale and rotate to right
			transform.localScale = new Vector3(1, 1, L);
			transform.rotation = Quaternion.Euler(0, 90, 0);
			
			// Create states, rotation is right
			startState = new DifferentialDriveState(
				startPos.x, startPos.y, Vector3.right);
			goalState = new DifferentialDriveState(
				goalPos.x, goalPos.y, Vector3.right);
			
			// Run rrt
			rrt = new RRT<DifferentialDriveState>(
				startState,
				goalState,
				DifferentialDriveState.GenerateRandom,
				polys,
				iterations,
				neighborhood
				);
			
			// Set moves and other data needed for base
			moves = new Stack<Move>(Enumerable.Reverse(rrt.moves));
			cost = rrt.cost;
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
			foreach (Move m in tmpMoves) {
				while (m.t > 0) {
					m.MoveMe(tr, 0.1f);
					poss.Add(tr.position);
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