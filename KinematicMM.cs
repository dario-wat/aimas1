using UnityEngine;
using System.Collections;
using System;

public class KinematicMM : AbstractVehicle {

	// Velocity of the vehicle
	public float velocity = 1.0f;

	// Maximum velocity
	public float maxVelocity = 5.0f;


	// Use this for initialization
	override protected void Start () {
		// Base call
		base.Start();
		// Check parameters
		require(maxVelocity > 0.0f, "Max velocity must be greater than 0");
	}

	// Moves vehicle in kinematic style, directly towards to
	// destination with constant velocity
	override protected bool MoveTo(Vector3 dest, float dt) {
		dest.y = 0.0f;			// Reset Y, it is constant

		// Velocity used to move the vehicle one step
		float transVel = Vector3.Distance(transform.position, dest);
		if (transVel < DIST_THRESHOLD) {	// At destination
			return true;
		}

		// Maximum velocity vehicle can achieve for this time step
		float maxVel = maxVelocity * dt;
		// Velocity used to translate the car
		float vel = Math.Min(transVel, maxVel);
		// Direction vector to destination
		Vector3 goalDir = Vector3.Normalize(dest - transform.position);
		// Translate the vehicle
		transform.Translate(goalDir * vel, Space.World);
		// Set public velocity, per second
		velocity = vel / dt;

		return false;
	}
}
