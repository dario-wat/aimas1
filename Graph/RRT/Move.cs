using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Move {

	// Rotation axis
	protected static Vector3 yAxis = Vector3.up;

	// How long this move has to be performed
	public float t { get; protected set; }


	// Base constructor
	public Move(float t) {
		this.t = t;
	}

	// Moves transform with given move for dt time
	public float MoveMe(Transform transform, float dt) {
		// Time used for transforming the vehicle
		float time = Mathf.Min(dt, t);

		// Performs transform
		MoveTransform(transform, time);

		// Returning how much time is left
		t -= time;
		return dt - time;
	}

	// Local transform
	protected abstract void MoveTransform(Transform transform, float time);

	// Checks if the point is obstructed
	protected abstract bool Obstructed(
		IEnumerable<Polygon> polys, Vector3 startPos);

	// Precicts where a move will end
	protected abstract Vector3 PredictPosition(Vector3 pos);

	// Creates a copy of the move
	public abstract Move Copy();


	// Checks if sequence of moves is obstructed by any of the polygons
	public static bool Obstructed(IEnumerable<Move> moves,
		IEnumerable<Polygon> polys, Vector3 startPos) {
	
		foreach (Move m in moves) {
			if (m.Obstructed(polys, startPos)) {
				return true;
			}
			startPos = m.PredictPosition(startPos);
		}
		return false;
	}
}
