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
	
	public static float radius {
		get { return r; }
	}
	
	
	// Constructor to set position
	public DynamicCarState(float x, float y, Vector3 orientation, float speed) {
		this.x = x;
		this.y = y;
		orientation.y = 0.0f;
		this.orientation = orientation.normalized;
		
		w = speed / r;
		//w = L / Mathf.Tan (maxPhi * toRad);

		
		this.speed = speed;
	}

	// Dubin curve distance
	public float Distance(DynamicCarState other) {
		//return Vector2.Distance(this.vec2, other.vec2);
		return Vector2.Distance(this.position, other.position); //+ 0.5f*Vector3.Angle(this.velocity, other.velocity);
	}

	// Creates a list of moves.
	/* Returns the shortest set of moves between two kinematic car states. */
	public Tuple<List<Move>, DynamicCarState> MovesTo(DynamicCarState other) {

		List<Move> moves = new List<Move>();
		// create the move
		//float dr = Tangents.RotationAngle (orientation, other.position - this.position);
		float time = timeStep;
		float acc = (speed < other.speed ? maxAcc : -maxAcc) * Mathf.Min (maxAcc, Mathf.Abs(this.speed - other.speed) / time);
		
		float rotAngle = Tangents.RotationAngle (orientation, other.position - this.position);
		
		float W = Mathf.Sign(rotAngle) * Mathf.Min (w, Mathf.Abs(rotAngle)*toRad/time);
		
		Move move = new DynamicCarMove( orientation, speed, acc, W, time ) ;
		moves.Add (move);


		float r = speed / W;
		float rot = time * W * toDeg;
		// Find the new state:
		DynamicCarState newState;
		if (r >= 1) {
			Vector3 centerToPos = - r * (Quaternion.Euler (0, 90 * Mathf.Sign (W), 0) * orientation).normalized;

			Vector3 newPosition = Quaternion.Euler (0, rot, 0) * centerToPos;
			Vector3 newOrientation = Quaternion.Euler (0, rot, 0) * orientation;
			newState = new DynamicCarState(newPosition.x, 
			                               newPosition.z, 
			                               newOrientation, 
			                               speed + acc*time);

		} else {
			newState = new DynamicCarState(position.x, 
			                               position.z, 
			                               orientation, 
			                               speed + acc*time);
		}

		return new Tuple<List<Move>, DynamicCarState>(moves, newState);

	}

	/* Shortest dubin path between points on two seperate directed circles. */
	private List<Move> getShortestMovesBetween (Vector3 initial, Vector3 veli, Vector3 final, Vector3 velf, Vector3 rci, Vector3 rcf){
		Vector3 center1 = initial + rci;
		Vector3 center2 = final + rcf;
		Vector3 di = Vector3.Cross (veli, rci);
		Vector3 df = Vector3.Cross (velf, rcf);
		
		/*If the circles intersect there exist no crossing tangents*/
		if (Vector3.Dot (di, df) < 1 && (center2-center1).magnitude < 2 * r) {
			return new List<Move>();
		}
		/*Find the tangentpoints */
		
		Vector3[] tangentPoints = (Vector3.Dot(di, df) > 1) ? Tangents.parallelTangentPoints(center1, center2, r) 
															: Tangents.intersectingTangentPoints(center1, center2, r);
		
		/* the angles and paths for the two tangents*/
		float sa1 = Tangents.RotationAngle( -rci, tangentPoints[0] - center1);
		Vector3 straight1 = tangentPoints [1] - tangentPoints [0];
		float s1 = (straight1).magnitude;
		float ea1 = Tangents.RotationAngle ( tangentPoints [1] - center2, -rcf);
		float l1 = Mathf.Abs(sa1) + s1 + Mathf.Abs(ea1);
		
		float sa2 = Tangents.RotationAngle(-rci, tangentPoints[2] - center1);
		Vector3 straight2 = tangentPoints [3] - tangentPoints [2];
		float s2 = (straight2).magnitude;
		float ea2 = Tangents.RotationAngle (tangentPoints [3] - center2, -rcf);
		float l2 = Mathf.Abs(sa2) + s2 + Mathf.Abs(ea2);
		
		
		/*Assign our path the shorter one*/
		float sa, s, ea;
		Vector3 straight;
		if (l1 < l2) {
			sa = sa1; s = s1; ea = ea1; straight = straight1;
		} else {
			sa = sa2; s = s2; ea = ea2;  straight = straight2;
		}
		
		List<Move> moves = new List<Move>();


		float dsa = Mathf.Sign(sa) * Mathf.Sign(di.y);
		Vector3 orient = Quaternion.Euler(0, sa, 0) * veli.normalized;
		float ds = ((Vector3.Angle (orient, straight) < 90) ? 1 : -1);
		float dea = Mathf.Sign(ea) * Mathf.Sign(df.y);

		//float totalDist = s + sa * toRad * r + ea * toRad * r;
		/*
		 * Move(Vector3 velocity, float speed, float absAcc, float theta, float t)
		 */
		float accDist;
		float accTime;
		float v = speed;
		if (dsa + ds == 0) {
			accDist = accelerationDistance(v, maxAcc, sa * toRad * r);
			accTime = accelerationTime(v, accDist);        	// two solutions
			// float accTime = (-speed - Mathf.Sqrt(speed*speed +2 * maxAcc * accDist)) / maxAcc;		// two solutions
			moves.Add ( new DynamicCarMove( orientation, v, maxAcc, w * toDeg * Mathf.Sign(di.y), accTime ) );


			//orientation
			v = v + accTime*maxAcc;
			accTime = v / maxAcc;
			moves.Add ( new DynamicCarMove( orientation, v, -maxAcc, w * toDeg * Mathf.Sign(di.y), accTime ) );
			v = v - accTime*maxAcc;

		} else {
			accDist = sa * toRad * r;
			accTime = accelerationTime(v, accDist); 
			moves.Add ( new DynamicCarMove( orientation, v, maxAcc, w * toDeg * Mathf.Sign(di.y), accTime ) );
			v = v + accTime*maxAcc;
		}

		if (ds + dea == 0) {
			accDist = accelerationDistance(v, maxAcc, s);
			accTime = accelerationTime(v, accDist);         	// two solutions
			// float accTime = (-speed - Mathf.Sqrt(speed*speed +2 * maxAcc * accDist)) / maxAcc;		// two solutions
			moves.Add (new DynamicCarMove (orientation, v, maxAcc, 0, accTime));

			v = v + accTime * maxAcc;
			accTime = v / maxAcc;
			moves.Add (new DynamicCarMove (orientation, v, -maxAcc, 0, accTime));
			v = v - accTime*maxAcc;
		} else {
			accDist = s;
			accTime = accelerationTime(v, accDist); 
			moves.Add ( new DynamicCarMove( orientation, v, maxAcc, 0, accTime ) );
			v = v + accTime*maxAcc;
		}

		//accDist = accelerationDistance(v, maxAcc, ea * toRad * r );
		if (velf.magnitude > v) {

			accTime = Mathf.Abs(velf.magnitude - v ) / maxAcc;
			moves.Add (new DynamicCarMove (orientation, v, maxAcc, w * toDeg * Mathf.Sign(df.y), accTime));

			v = v + accTime*maxAcc;
			moves.Add (new DynamicCarMove (orientation, v, 0, w * toDeg * Mathf.Sign(df.y), accTime));

		} else {
			accTime = (( v - velf.magnitude ) / maxAcc);
			moves.Add (new DynamicCarMove (orientation, v, -maxAcc, w * toDeg * Mathf.Sign(df.y), accTime));

			v = v + accTime*maxAcc;
			moves.Add (new DynamicCarMove (orientation, v, 0, w * toDeg * Mathf.Sign(df.y), accTime));
		}


		accDist = velf.sqrMagnitude - v * v;

		accTime = accelerationTime(v, accDist);        	// two solutions
		
		return moves;
	}

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
		float speed = Random.Range(0, 10);
		return new DynamicCarState(x, y, orientation, speed);
	}

	override public string ToString() {
		return string.Format("{0} {1} {2} {3}", x, y, speed, orientation);
	}
}