using UnityEngine;
using System.Collections;
using System.Collections.Generic;	


public class DynamicCarState : IVehicleState<DynamicCarState> {

	// Position
	private float x;
	private float y;
	private float speed;

	// Max velocity
	public static float maxAcc = 1.0f;
	public static float maxPhi = 25f;
	public static float L;
	public static float r;  //TODO: calculate this
	public static float w;
	public static float timeStep;
	float toRad = (Mathf.PI/180);
	float toDeg = (180/Mathf.PI);
	
	// Direction
	private Vector3 orientation;
	
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
	
	public Vector3 position {
		get { return new Vector3(x, 0.0f, y); }
	}
	
	public Vector3 velocity {
		get { return speed * (orientation.normalized); }
	}

	
	
	// Constructor to set position
	public DynamicCarState(float x, float y, Vector3 orientation, float speed) {
		this.x = x;
		this.y = y;
		orientation.y = 0.0f;
		this.orientation = orientation.normalized;
		////Debug.Log ("phi:" + (maxPhi*toRad));
		w = speed * Mathf.Tan(maxPhi*toRad) / L;
		w = L / Mathf.Tan (maxPhi * toRad);
//		//Debug.Log ("w: "+ w);

		this.speed = speed;
		////Debug.Log ("angvel: "+w);
		////Debug.Log ("r: "+r);
	}

	// Dubin curve distance
	public float Distance(DynamicCarState other) {
		//float rotAngle = Tangents.RotationAngle (orientation, other.position - this.position);
		//float phi = Mathf.Sign (rotAngle) * Mathf.Min (Mathf.Abs (rotAngle), maxPhi);
		//float r = L / Mathf.Tan (Mathf.Abs(phi) * toRad);



		return Vector2.Distance(this.vec2, other.vec2) / (speed + 0.01f);
			 //+ 0.25f*Vector3.Angle(this.velocity, other.velocity);
	}

	// Creates a list of moves.
	/* Returns the shortest set of moves between two kinematic car states. */
	public Tuple<List<Move>, DynamicCarState> MovesTo(DynamicCarState other) {

		List<Move> moves = new List<Move> ();
		// create the move
		//float dr = Tangents.RotationAngle (orientation, other.position - this.position);
		float time = timeStep;
		// Sign of the acceleration determined by the difference in velocities
		// magnitude of acceleration is either maxAcc or the value needed to accelerate to reach the target speed.
		float acc = (speed < other.speed ? 1 : -1) * Mathf.Min (maxAcc, Mathf.Abs (this.speed - other.speed) / time);
		////Debug.Log ("acc:" + acc);
		// The angle from current orientation to the destination is used to determine how much and in which direction the car should steer.
		float rotAngle = Tangents.RotationAngle (orientation, other.position - this.position);
		// The sign of the steering  * maximum or sufficient steering angle. 
		float phi = Mathf.Sign (rotAngle) * Mathf.Min (Mathf.Abs (rotAngle), maxPhi);
		// turning radius determined and used to calculate the angular velocity
		float r = L / Mathf.Tan (Mathf.Abs(phi) * toRad);


		// Find the new state:
		DynamicCarState newState;
		float endSpeed = speed + acc * time;
		float angle = ((speed + endSpeed) / (2 * r)) * time * toDeg;
		
		Vector3 carToCenter = r * (Quaternion.Euler (0, 90 * Mathf.Sign (phi), 0) * orientation).normalized;
		Vector3 centerToCar = - carToCenter;
		Vector3 center = position + carToCenter;
		Vector3 newPosition = Quaternion.Euler (0, angle * Mathf.Sign(phi), 0) * centerToCar + center;
		Vector3 newOrientation = Quaternion.Euler (0, angle * Mathf.Sign(phi), 0) * orientation;
		
		
		newState = new DynamicCarState (newPosition.x, 
		                                newPosition.z, 
		                                newOrientation, 
		                                speed + acc * time);
		
		////Debug.Log ("pos: " + newState.position);
		//Debug.Log ("Moving from: "+position+", or: "+orientation+"speed: "+speed+ 
//		           "\nTowards: "+other.position+", or: "+other.orientation+"speed: "+other.speed+
//		           "\nphi: " + phi + ", r: "+r+", angle: "+angle+", acc: "+acc+
//		           "\nnew state: "+newState.position + ", or: "+newState.orientation+", speed"+newState.speed);
		//		//Debug.Log (speed + acc * time);



		Move move = new DynamicCarMove (orientation, speed, acc, phi, r, newState, time);
		moves.Add (move);




		return new Tuple<List<Move>, DynamicCarState> (moves, newState);

	}

	//public Tuple<List<Move>, DynamicCarState> MovesToGoal(DynamicCarState other) {

	private float accelerationDistance(float v0, float a, float l) {
		//return l - 0.5f * (v1 * v1 - v0 * v0) / a; 

		return Mathf.Abs( 0.5f * (l + 0.5f * (v0 * v0) / a));
	}

	private float accelerationTime(float v0, float accDist) {
		return (-v0 + Mathf.Sqrt (v0 * v0 + 2 * maxAcc * accDist)) / maxAcc; 
	}


	// Generates new random kinematic point state
	public static DynamicCarState GenerateRandom() {
		float x = Random.Range(lo, hi);
		float y = Random.Range(lo, hi);
		
		Vector3 dir = Random.onUnitSphere;
		Vector2 vec2Dir = new Vector2(dir.x, dir.z).normalized;
		Vector3 orientation = new Vector3(vec2Dir.x, 0, vec2Dir.y);
		float speed = Random.Range(0, 20);
		return new DynamicCarState(x, y, orientation, speed);
	}
}