using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinematicCarState : IVehicleState<KinematicCarState> {
	
	// Position
	private float x;
	private float y;
	
	// Max velocity
	public static float maxVel = 1.0f;
	public static float maxPhi = 25f;
	public static float L;
	public static float r;  //TODO: calculate this
	public static float w;
	public static string heuristic;

	private static string dubinH = "Dubin";
	private static string euclH = "Eucl";
	private static string distH = "Distance";
	private static string upperH = "Upper";

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
	public KinematicCarState(float x, float y, Vector3 orientation) {
		this.x = x;
		this.y = y;
		orientation.y = 0.0f;
		this.orientation = orientation.normalized;
		//Debug.Log ("phi:" + (maxPhi*toRad));
		w = maxVel * Mathf.Tan(maxPhi*toRad) / L;
		KinematicCarState.r = maxVel / w;
		//Debug.Log ("angvel: "+w);
		//Debug.Log ("r: "+r);
	}
	
	// Dubin curve distance
	public float Distance(KinematicCarState other) {
		if (heuristic.Equals(upperH)) {
			return (Vector2.Distance(this.vec2, other.vec2)
				+ 2*r + 3*Mathf.PI*r)/maxVel;
		}
		if (heuristic.Equals(euclH)) {
			return Vector2.Distance(this.vec2, other.vec2) / maxVel;
		}
		if (heuristic.Equals(dubinH)) {
			List<Move> moves = MovesTo (other)._1;
			float total = 0;
			foreach (Move m in moves) {
				total += m.t;
			}
			return total;
		}
		if (heuristic.Equals(distH)) {
			return Vector3.Distance(this.position, other.position) /maxVel
				+ 0.5f*Vector3.Angle(this.velocity, other.velocity) / (w*toDeg);
		}
		return 0;
	}
	
	// Creates a list of moves.
	/* Returns the shortest set of moves between two kinematic car states. */
	public Tuple<List<Move>, KinematicCarState> MovesTo(KinematicCarState other) {
		Vector3 initial = this.position;
		Vector3 final = other.position;
		Vector3 veli = this.velocity;
		Vector3 velf = other.velocity;
		
		Vector3 rci1 = r*(Quaternion.Euler (0, 90, 0) * veli).normalized;

		Vector3 rci2 = -rci1;
		Vector3 rcf1 = r*(Quaternion.Euler (0, 90, 0) * velf).normalized;
		Vector3 rcf2 = -rcf1;
		
		/* find the shortest set of moves using the four circles*/
		List<List<Move>> moves = new List<List<Move>>();
		moves.Add(getShortestMovesBetween (initial, veli, final, velf, rci1, rcf1));
		moves.Add(getShortestMovesBetween (initial, veli, final, velf, rci1, rcf2));
		moves.Add(getShortestMovesBetween (initial, veli, final, velf, rci2, rcf1));
		moves.Add(getShortestMovesBetween (initial, veli, final, velf, rci2, rcf2));
		
		float shortestDist = float.MaxValue;
		List<Move> shortestPath = null;
		float tmpDist;
		foreach (List<Move> m in moves ){
			tmpDist = 0;
			foreach(Move mm in m) {
				tmpDist += mm.t;
			}
			/*The > 0 condition is  to make sure it is not empty*/
			if (tmpDist < shortestDist && tmpDist > 0) {
				shortestDist = tmpDist;
				shortestPath = m;
			}
		}
		return new Tuple<List<Move>, KinematicCarState>(shortestPath, other);
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
		/*negative speed: back up!  if the angle needed to make and the direction of the circle are of opposite sign the speed should be negative*/
		float speed = maxVel * Mathf.Sign(sa) * Mathf.Sign(di.y);
		moves.Add( new KinematicCarMove(veli.normalized, speed, w * toDeg * Mathf.Sign(di.y), Mathf.Abs(sa) * toRad/ w) );
		
		Vector3 orientation = Quaternion.Euler(0, sa, 0) * veli.normalized;
		speed = maxVel * ((Vector3.Angle (orientation, straight) < 90) ? 1 : -1);  // If the orientation and straight path are opposite we need a negative velocity
		moves.Add( new KinematicCarMove(orientation, speed, 0, s / Mathf.Abs(speed)) );
		
		speed = maxVel * Mathf.Sign(ea) * Mathf.Sign(df.y);
		moves.Add( new KinematicCarMove(orientation, speed, w * toDeg * Mathf.Sign(df.y), Mathf.Abs(ea) * toRad / w) );

		return moves;
	}
	
	// Overriding object's Equals
	override public bool Equals(object other) {
		if (!(other is KinematicCarState)) {
			return false;
		}
		KinematicCarState o = other as KinematicCarState;
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
	public static KinematicCarState GenerateRandom() {
		float x = Random.Range(lo, hi);
		float y = Random.Range(lo, hi);

		Vector3 dir = Random.onUnitSphere;
		Vector2 vec2Dir = new Vector2(dir.x, dir.z).normalized;
		Vector3 orientation = new Vector3(vec2Dir.x, 0, vec2Dir.y);
		return new KinematicCarState(x, y, orientation);
	}
}