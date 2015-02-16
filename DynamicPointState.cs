using UnityEngine;
using System.Collections.Generic;
using System;

public class DynamicPointState : IVehicleState<DynamicPointState> {

	// Range to generate random number
	public static float hi = 100.0f;
	public static float lo = 0.0f;

	// Maximum acceleration
	public static float maxAcc = 1.0f;

	// Number of points to use for collision detecting
	public static int collisionPoints = 3;

	// Heuristic for moving
	public static string heuristic;

	// Heuristic parameter
	public static float hParam;

	// Heuristics
	private static string distanceH = "Distance";
	private static string timeH = "Time";

	// Goal state needed for braking
	public static DynamicPointState goalState;

	// Coordinates
	private float x;
	private float y;

	// Velocity
	private Vector3 velocity;

	// Vector2 representation
	public Vector2 vec2 {
		get { return new Vector2(x, y); }
	}

	// Vector3 representation
	public Vector3 vec3 {
		get { return new Vector3(x, 0, y); }
	}


	// Just constructor
	public DynamicPointState(float x, float y, Vector3 velocity) {
		this.x = x;
		this.y = y;
		this.velocity = velocity;
	}

	// Vector constructor
	public DynamicPointState(Vector3 position, Vector3 velocity)
		: this(position.x, position.z, velocity) {
	}

	// Uses time for linear distance
	public float Distance(DynamicPointState other) {
		return Vector2.Distance(this.vec2, other.vec2)
			/ (velocity.magnitude + 0.1f);
	}

	// desired_velocity = normalize (position - target) * max_speed
	// steering = desired_velocity - velocity
	public Tuple<List<Move>, DynamicPointState> MovesTo(
		DynamicPointState other) {

		Vector3 diff = other.vec3 - this.vec3;
		Vector3 desired = diff.normalized * Vector3.Distance(this.vec3, other.vec3);
		Vector3 acc = (desired - velocity).normalized * maxAcc;
		List<Move> moves = new List<Move>();
		
		float t;
		if (heuristic.Equals(timeH)) {
			t = hParam;
		} else if (heuristic.Equals(distanceH)) {
			float s = hParam;
			t=(-velocity.magnitude
				+ Mathf.Sqrt(velocity.magnitude * velocity.magnitude
					- 2 * maxAcc * (-s)))
				/ (maxAcc);
		} else {
			throw new ArgumentException("No such heuristic");
		}

		// This stops the car, I have no idea if it really is working
		// **** Move out if bad results
		float dist = Vector2.Distance(this.vec2, goalState.vec2);
		if (dist < 0.5f * velocity.magnitude * velocity.magnitude / maxAcc) {
			acc = - (velocity.normalized * maxAcc);
		}

		Vector3 newPos = this.vec3 + velocity * t + 0.5f * acc * t * t;
		Vector3 newVel = velocity + acc * t;
		DynamicPointState newState = new DynamicPointState(newPos, newVel); 
		moves.Add(new DynamicPointMove(velocity, acc, t));
		return new Tuple<List<Move>, DynamicPointState>(moves, newState);
	}

	// For Debugging
	override public string ToString() {
		return "Loc: " + x.ToString("0.00") + " " + y.ToString("0.00")
			+ "  Vel: " + velocity.x.ToString("0.00") + " "
			+ velocity.z.ToString("0.00"); 
	}

	// State generator
	public static DynamicPointState GenerateRandom() {
		float x = UnityEngine.Random.Range(lo, hi);
		float y = UnityEngine.Random.Range(lo, hi);

		// Velocity is not used anyway
		float dx = UnityEngine.Random.Range(-maxAcc*10, maxAcc*10);
		float dy = UnityEngine.Random.Range(-maxAcc*10, maxAcc*10);
		return new DynamicPointState(x, y, new Vector3(dx, 0, dy));
	}
}
