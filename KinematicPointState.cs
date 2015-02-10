using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinematicPointState : IVehicleState<KinematicPointState> {

	// Position
	private float x;
	private float y;

	// Max velocity
	public static float maxVel = 1.0f;

	// Range to generate random number
	public static float limit = 100.0f;

	public Vector2 vec2 {
		get { return new Vector2(x, y); }
	}

	public Vector3 vec3 {
		get { return new Vector3(x, 0.0f, y); }
	}


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
		
		Vector2 vel = other.vec2 - this.vec2;
		vel.Normalize();
		vel *= maxVel;
		float dx = vel.x;								// Normalized direction
		float dy = vel.y;
		
		moves.Add(new Move(dx, dy, t));
		return moves;
	}


	// Generates new random kinematic point state
	public static KinematicPointState GenerateRandom() {
		float x = Random.Range(0.0f, limit);
		float y = Random.Range(0.0f, limit);
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

	// For debugging
	override public string ToString() {
		return string.Format("{0} {1}", x, y);
	}
}
