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
	public static float hi = 100.0f;
	public static float lo = 0.0f;

	// Vector2 representation
	public Vector2 vec2 {
		get { return new Vector2(x, y); }
	}

	// Vector3 representation
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

	// Euclidean distance in time
	public float Distance(KinematicPointState other) {
		return EuclideanDistance(other) / maxVel; 
	}

	// Euclidean distance between 
	private float EuclideanDistance(KinematicPointState other) {
		return Vector2.Distance(this.vec2, other.vec2);
	}

	// Creates a lit of moves (which is in this case only one move).
	// One move with constant speed in constant direction
	public Tuple<List<Move>, KinematicPointState> MovesTo(
		KinematicPointState other) {
		
		float distance = this.EuclideanDistance(other);
		float t = distance / maxVel;			// How long to move
		Vector2 vel = (other.vec2 - this.vec2).normalized * maxVel;
		
		// Giving a real velocity vector, not normalized
		List<Move> moves = new List<Move>();
		moves.Add(new KinematicPointMove(new Vector3(vel.x, 0, vel.y), t));
		return new Tuple<List<Move>, KinematicPointState>(moves, other);
	}

	// Overriding object's Equals
	override public bool Equals(object other) {
		if (!(other is KinematicPointState)) {
			return false;
		}
		KinematicPointState o = other as KinematicPointState;
		return this.x.Equals(o.x) && this.y.Equals(o.y);
	}

	// Hashcode so compiler stops complaining
	override public int GetHashCode() {
		return x.GetHashCode() + 31 * y.GetHashCode();
	}

	// For debugging
	override public string ToString() {
		return string.Format("{0} {1}", x, y);
	}

	// Generates new random kinematic point state
	public static KinematicPointState GenerateRandom() {
		float x = Random.Range(lo, hi);
		float y = Random.Range(lo, hi);
		return new KinematicPointState(x, y);
	}
}
