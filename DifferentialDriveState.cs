using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DifferentialDriveState : IVehicleState<DifferentialDriveState> {
	
	// Position
	private float x;
	private float y;
	
	// Max velocity
	public static float maxVel = 1.0f;
	public static float L;
	public static float r;  //TODO: calculate this
	public static float w;
	public static float maxOmega;

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
		get { return maxVel * (orientation.normalized); }
	}
	
	public float radius {
		get { return r; }
	}
	
	
	// Constructor to set position
	public DifferentialDriveState(float x, float y, Vector3 orientation) {
		this.x = x;
		this.y = y;
		orientation.y = 0.0f;
		this.orientation = orientation.normalized;
		w = maxOmega;
		DifferentialDriveState.r = maxVel / w;
	}
	
	// Dubin curve distance
	public float Distance(DifferentialDriveState other) {
		Vector3 destination = other.position;
		float angle = Tangents.RotationAngle (orientation, destination - position);
		float v = maxVel;
		float omega = w;

		if (Mathf.Abs (angle) > 90) {
			angle = angle - 180 * Mathf.Sign(angle);
		}
		
		float d = Vector3.Distance (position, destination);
		
		//Debug.Log ("d: "+d);
		float absAngle = Mathf.Abs (angle);
		
		//Debug.Log ("phi: "+phi);
		float radius = (d/2) / Mathf.Sin (absAngle * toRad);
		if (radius > r) {
			omega = v / radius;
		}
		return Mathf.Abs (2 * (absAngle)) * toRad / omega;
		
	}
	
	// Creates a list of moves.
	/* Returns the shortest set of moves between two kinematic car states. */
	public Tuple<List<Move>, DifferentialDriveState> MovesTo(DifferentialDriveState other) {

		Vector3 destination = other.position;
		float angle = Tangents.RotationAngle (orientation, destination - position);
		float v = maxVel;
		float omega = w;
		//Debug.Log ("from: " + position + " to: " + destination);
		//Debug.Log ("Angle: "+angle);
		//Debug.Log ("orientation: "+orientation);
		
		//Debug.Log ("omega: "+omega);
		//Debug.Log ("v: "+v);

		Vector2 ori = orientation;
		int directionOfSpeed = 1;

		if (Mathf.Abs (angle) > 90) {
			angle = angle - 180 * Mathf.Sign(angle);
			ori = - ori;
			directionOfSpeed = -1;
		}
		//Debug.Log ("new Angle: "+angle + ", neworientation: " + ori + ", direction of speed: "+directionOfSpeed);

		float d = Vector3.Distance (position, destination);
		
		//Debug.Log ("d: "+d);
		float absAngle = Mathf.Abs (angle);
		
		//Debug.Log ("phi: "+phi);
		float radius = (d/2) / Mathf.Sin (absAngle * toRad);
		
		//Debug.Log ("radius: "+radius);

		if (radius > r) {
			omega = v / radius;
		} else {
			v = omega * radius;
		}
		//Debug.Log ("new omega: "+omega);
		//Debug.Log ("new v: "+v);


		//Debug.Log ("orientation: "+orientation + ", v*direction: " + (v * directionOfSpeed) + ", omega*signAngle: "+(omega * toDeg * Mathf.Sign (angle) )+"time: "+Mathf.Abs (2 * phi) * toRad / omega);
		List<Move> move = new List<Move>();
		move.Add (new KinematicCarMove (orientation, 
				                        v * directionOfSpeed, 
						                omega * toDeg * Mathf.Sign (angle*directionOfSpeed), 
				                        Mathf.Abs (2 * absAngle) * toRad / omega));

		Vector3 newOrientation = Quaternion.Euler (0, 2*angle, 0) * orientation;
		//Debug.Log ("destination.x: "+destination.x + ", destination.z: " + destination.z + ", newOrientation: "+newOrientation);

		DifferentialDriveState newState = new DifferentialDriveState (destination.x, 
		                                                             destination.z,  
		                                                             newOrientation);

		return new Tuple<List<Move>, DifferentialDriveState>(move, newState);
	}
	

	
	// Overriding object's Equals
	override public bool Equals(object other) {
		if (!(other is DifferentialDriveState)) {
			return false;
		}
		DifferentialDriveState o = other as DifferentialDriveState;
		return this.x.Equals(o.x) && this.y.Equals(o.y);
	}
	
	// Compiler complaining
	override public int GetHashCode() {
		return x.GetHashCode() + 31 * y.GetHashCode()
			+ 31 * 31 * orientation.GetHashCode();
	}
	
	// For debugging
	override public string ToString() {
		return string.Format("{0} {1} {2}", x, y, orientation);
	}
	
	// Generates new random kinematic point state
	public static DifferentialDriveState GenerateRandom() {
		float x = Random.Range(lo, hi);
		float y = Random.Range(lo, hi);
		
		Vector3 dir = Random.onUnitSphere;
		Vector2 vec2Dir = new Vector2(dir.x, dir.z).normalized;
		Vector3 orientation = new Vector3(vec2Dir.x, 0, vec2Dir.y);
		return new DifferentialDriveState(x, y, orientation);
	}
}