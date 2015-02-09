using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KinematicPointState : IVehicleState<KinematicPointState> {

	// Position
	private float x;
	private float y;

	// Max velocity
	public static float maxVel = 1.0f;

	// Range to generate random number
	public static float limit = 100.0f;


	// Constructor to set position
	public KinematicPointState(float x, float y) {
		this.x = x;
		this.y = y;
	}

	// Construct from Vector2
	public KinematicPointState(Vector2 pos) : this(pos.x, pos.y) {
	}

	// Euclidian distance
	public float Distance(KinematicPointState other) {
		float x = this.x - other.x;
		float y = this.y - other.y;
		return Mathf.Sqrt(x*x + y*y);
	}

	// Creates a lit of moves (which is in this case only one move).
	// One move with constant speed in constant direction
	public List<Move> MovesTo(KinematicPointState other) {
		List<Move> moves = new List<Move>();
		float distance = this.Distance(other);
		float t = distance / maxVel;			// How long to move
		
		float dx = other.x - this.x;			// Direction
		float dy = other.y - this.y;
		Vector2 vel = new Vector2(dx, dy);
		vel.Normalize();
		vel *= maxVel;
		dx = vel.x;								// Normalized direction
		dy = vel.y;
		
		moves.Add(new Move(dx, dy, t));
		return moves;
	}

	// Vector3 representation of location
	public Vector3 Location() {
		return new Vector3(x, 0.0f, y);
	}

	// Generates new random kinematic point state
	public static KinematicPointState GenerateRandom() {
		System.Random r = new System.Random();
		float x = (float) r.NextDouble() * limit;
		float y = (float) r.NextDouble() * limit;
		return new KinematicPointState(x, y);
	}

	// Overriding object's Equals
	override public bool Equals(object other) {
		if (!(other is KinematicPointState)) {
			return false;
		}
		KinematicPointState o = other as KinematicPointState;
		return this.x.Equals(o.x) && this.y.Equals(o.y);
	}
}
