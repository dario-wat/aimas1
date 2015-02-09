using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinematicPointRRT : AbstractVehicle {

	// Max velocity
	public float maxVel;

	// VState of start and goal
	private KinematicPointState startState;
	private KinematicPointState goalState;

	// Queue of moves
	override protected Queue<Move> moves { get; set; }

	override protected float cost { get; set; }

	// Runs RRT and finds path
	override protected void LocalStart() {
		KinematicPointState.maxVel = maxVel;
		startState = new KinematicPointState(startPos);
		goalState = new KinematicPointState(goalPos);
		RRT<KinematicPointState> rrt = new RRT<KinematicPointState>(
			startState, goalState, KinematicPointState.GenerateRandom, polys);
		moves = new Queue<Move>(rrt.moves);
		cost = 0.0f;
	}
}
