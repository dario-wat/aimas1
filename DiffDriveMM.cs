using UnityEngine;
using System.Collections;
using System;

public class DiffDriveMM : AbstractVehicle {

	// Velocity of the vehicle
	public float velocity = 1.0f;

	// Maximum velocity of the car
	public float maxVelocity = 5.0f;

	// Angle (direction) of the vehicle
	public float theta = 0.0f;

	// Angular velocity
	public float omega = 15.0f;

	// Max angular velocity
	public float maxOmega = 50.0f;


	// Use this for initialization
	override protected void Start () {
		// Calling super initialization
		base.Start();
		// Check arguments
		require(maxOmega > 0.0f, "Max Omega must be greater than 0");
		require(maxVelocity > 0.0f, "Max Velocity must be greater than 0");
		// Set theta, initial angle
		theta = transform.eulerAngles.y;
	}


	// Moves vehicle to destination point
	// All Y coordinates of position are set to 0 to make calculations easier
	// Returns true if it is at destination
	override protected bool MoveTo(Vector3 dest, float dt) {
		dest.y = 0.0f;			// Reset Y, it is constant

		// Velocity used to move the vehicle one step
		float transVel = Vector3.Distance(transform.position, dest);
		if (transVel < DIST_THRESHOLD) {	// At destination
			return true;
		}

		// Angle to rotate
		float rotAngle = RotationAngle(dest);
		if (Math.Abs(rotAngle) > TURN_THRESHOLD) {
			/**** Rotation part ****/

			// Maximum turn vehicle can make given the parameters
			float maxRotate = Math.Sign(rotAngle) * maxOmega * dt;
			// Angle which will be used to rotate vehicle in this time step
			float rotate = Math.Sign(rotAngle)
				* Math.Min(Math.Abs(rotAngle), Math.Abs(maxRotate));
			// Finally rotate the vehicle
			transform.RotateAround(transform.position, Yaxis, rotate);
			// Set speed to the outside world so it can be visible, per second
			omega = rotate / dt;
			theta = transform.eulerAngles.y;	// Set public variable
		} else {
			/**** Translation part ***/

			// Maximum velocity vehicle can achieve for this time step
			float maxVel = maxVelocity * dt;
			// Velocity used to translate the car
			float vel = Math.Min(transVel, maxVel);
			// Translate the vehicle
			theta = transform.eulerAngles.y;	// Set public variable
			float dz = (float) Math.Cos(theta * PI / 180.0f) * vel;
			float dx = (float) Math.Sin(theta * PI / 180.0f) * vel;
			transform.Translate(dx, 0.0f, dz, Space.World);
			// Set public velocity, per second
			velocity = vel / dt;
		}

		return false;
	}

}
