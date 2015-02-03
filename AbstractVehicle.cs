using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class AbstractVehicle : MonoBehaviour {

	/** Definitions of all variables that are used for all vehicle models. **/

	// Z axis if forward-back, X axis is left-right

	// Starting coordinates
	public Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);

	// Size of the cube (vehicle)
	public Vector3 size = new Vector3(1.0f, 0.5f, 2.0f);

	// Initial rotation of the vehicle
	public Vector3 rotation = new Vector3(0.0f, 0.0f, 0.0f);


	// Used for rotating around the Y axis.
	protected Vector3 Yaxis = new Vector3(0, 1, 0);

	// This is float PI constant
	protected const float PI = (float) Math.PI;

	// Threshold for the distance to the goal
	protected const float DIST_THRESHOLD = 0.01f;

	// Threshold for angle when it's precise enough to go straight
	protected const float TURN_THRESHOLD = 0.01f;

	// Multiply to convert to radians
	protected const float toRad = PI / 180.0f;

	// Multiply to convert to degrees
	protected const float toDeg = 180.0f / PI;

	// PathGenerator object instance
	protected PathGenerator pg = PathGenerator.instance;


	// Use this for initialization
	protected virtual void Start() {
		transform.position = start;			// Sets the starting coordinates
		transform.localScale = size;		// Sets the size of the vehicle
		transform.eulerAngles = rotation;	// Sets initial rotation

		
		PathGenerator.InitSample();
	}
	
	// Update is called once per frame
	protected virtual void Update() {
		// Performing movement to destination points
		if (pg.HasMore) {
			bool arrived = MoveTo(pg.Current, Time.deltaTime);
			if (arrived) {
				pg.Next();
			}
		}
	}

	// Defined in subclasses, move to destination
	// This is the main modelling function
	// Returns true if at destination, else false
	protected abstract bool MoveTo(Vector3 dest, float dt);


	/** Following functions are used in all vehicle models. **/

	// Computes how much vehicle has to rotate
	// This function is used for vehicle modes which rotate in order
	// to get to the goal
	// Gives solution [-180, 180]
	protected float RotationAngle(Vector3 destination) {
		// Very important condition
		assert(destination.y == 0.0f);
		
		// Current direction of the vehicle
		Vector3 currDir = transform.forward;

		// Direction of the goal from the vehicle's position
		Vector3 goalDir = Vector3.Normalize(destination - transform.position);

		// Cross product to find out if I have to turn left or right
		float angle = Vector3.Angle(currDir, goalDir);		// Angle between

		Vector3 cross = Vector3.Cross(currDir, goalDir);	// Cross product
		if (angle == 180.0f) {		// To make sure that it turns if it's 180
			cross.y = 1.0f;
		}
		return Math.Sign(cross.y) * angle;
	}



	#region Debug functions

	// Function for checking arguments
	protected void require(bool predicate, String message) {
		if (!predicate) {
			throw new ArgumentException(message);
		}
	}

	// Similar to require but has different meaning
	protected void assert(bool predicate) {
		if (!predicate) {
			throw new ArgumentException(
				"This is assert exception! Your code is incorrect!"
			);
		}
	}

	#endregion
}
