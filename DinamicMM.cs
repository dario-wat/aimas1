using UnityEngine;
using System.Collections;
using System;

public class DinamicMM : AbstractVehicle {

	// Velocity of the vehicle
	public float velocity = 0.0f;

	// Maximum velocity
	public float maxVelocity = 5.0f;

	// Acceleration of the vehicle
	public float acceleration = 0.0f;

	// Maximum acceleration
	public float maxAcceleration = 1.0f;

	// Use this for initialization
	override protected void Start () {
		// Base call
		base.Start();
		// Check parameters
		require(maxVelocity > 0.0f, "Max velocity must be greater than 0");
		require(maxAcceleration > 0.0f, "Max acceleration must be greater than 0");
	}
	
	// Moves vehicle towards the destination
	override protected bool MoveTo(Vector3 dest, float dt) {
		dest.y = 0.0f;			// Reset Y, it is constant

		// Velocity used to move the vehicle one step
		float distance = Vector3.Distance(transform.position, dest);
		if (distance < DIST_THRESHOLD) {	// At destination
			return true;
		}

		// Threshold to start deccelerating
		float deccThresh = 0.5f * velocity * velocity / maxAcceleration;
		if (distance > deccThresh) {
			acceleration = maxAcceleration;
		} else {
			acceleration = -maxAcceleration;
		}
		// Direction vector to destination
		Vector3 goalDir = Vector3.Normalize(dest - transform.position);
		// Change velocity
		velocity += acceleration * dt;
		if (velocity > maxVelocity) {	// Make sure velocity stays in bounds
			velocity = maxVelocity;
		} else if (velocity < -maxVelocity) {
			velocity = -maxVelocity;
		}
		// Translate the vehicle
		transform.Translate(goalDir * velocity * dt, Space.World);
		return false;
	}
}
