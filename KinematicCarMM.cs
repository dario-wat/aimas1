using UnityEngine;
using System.Collections;
using System;

public class KinematicCarMM : AbstractVehicle {

	// Velocity of the car
	public float velocity = 0.0f;

	// Maxmimu velocity
	public float maxVelocity = 1.0f;

	// Angle of the car
	public float theta = 0.0f;

	// Angle of the wheels
	public float phi = 0.0f;

	// Maximum angle of the wheels
	public float maxPhi = 40.0f;

	// Angular velocity
	public float omega = 0.0f;

	// Car length
	public float L = 2.0f;


	// Use this for initialization
	override protected void Start () {
		// Calling super initialization
		base.Start();
		// Check arguments
		require(maxPhi > 0.0f, "Max Omega must be greater than 0");
		require(maxVelocity > 0.0f, "Max Velocity must be greater than 0");
		// Set theta, initial angle
		theta = transform.eulerAngles.y;
		// Set car length
		L = transform.localScale.z;
	}
	
	// Make move to next point
	override protected bool MoveTo(Vector3 dest, float dt) {
		dest.y = 0.0f;

		// Velocity used to move the vehicle one step, in dt time
		// aka distance to destination
		float transVel = Vector3.Distance(transform.position, dest);
		if (transVel < DIST_THRESHOLD) {	// At destination
			return true;
		}

		// Angle needed to rotate in degrees with sign, - left, + right
		float rotAngle = RotationAngle(dest);
		// Which way to turn
		int turn = Math.Sign(rotAngle);
		// Tangens of the wheels
		float wheelTan = (float) Math.Tan(maxPhi * toRad);
		
		// Max angle the vehicle can turn in dt time, in degrees
		float maxOmega = turn * (maxVelocity * dt / L) * wheelTan * toDeg;
		// How much to rotate with sign, in degrees, in dt time
		float rotate = turn * Math.Min(Math.Abs(rotAngle), Math.Abs(maxOmega));
		
		// Choose correct velocity per second
		// Maybe a bit complicated, could be simplified using
		// another variable and calculating everything in dt time
		if (Math.Abs(rotAngle) > TURN_THRESHOLD) {	// Need to turn
			// Compute needed velocity to make the turn per second
			velocity = Math.Abs(rotate) * toRad * L / wheelTan / dt;
			// Rotate vehicle, rotate is in degrees
			transform.RotateAround(transform.position, Yaxis, rotate);
		} else {
			// Velocity when translating
			velocity = Math.Min(maxVelocity, transVel / dt);
		}
		
		// Update public variables
		theta = transform.eulerAngles.y;
		omega = rotate / dt;
		phi = (float) Math.Atan(omega * toRad * L / velocity) * toDeg;
		
		// Translate vehicle
		float dz = (float) Math.Cos(theta * toRad) * velocity * dt;
		float dx = (float) Math.Sin(theta * toRad) * velocity * dt;
		transform.Translate(dx, 0.0f, dz, Space.World);
		
		return false;
	}
}
