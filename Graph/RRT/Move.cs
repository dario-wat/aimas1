using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move {

	// Rotation axis
	private static Vector3 yAxis = Vector3.up;


	// Turning angle in degrees per second
	private float theta = 0.0f;

	// Velocity vector
	private Vector3 velocity;

	private float speed;

	// Acceleration vector
	private Vector3 acceleration;

	// Absolute value for acceleration, only one of these acceleration
	// variables is used, absAcc accelerates the car in the direction of its
	// velocity
	private float absAcc;

	// Radius of the circle of movement
	private float r;

	// How long move has to be done
	public float t { get; private set; }

	private Vector3 centerOff;

	private bool left;


	private Move(Vector3 velocity, Vector3 acceleration, float speed,
		float absAcc, float theta, float t) {
		
		this.t = t;
		this.velocity = velocity.normalized;
		this.speed = speed;
		this.acceleration = acceleration.normalized;
		this.absAcc = absAcc;
		this.theta = Mathf.Sign(speed) * theta;
		this.left = theta < 0;
		this.r = Mathf.Abs(speed) / (Mathf.Abs(theta) * Mathf.PI / 180.0f);

		if (theta != 0) {
			Vector3 normal;
			if (left) {
				// Turning in left circle
				normal = Quaternion.Euler(0, -90, 0) * velocity;
			} else {
				// Turning in right circle
				normal = Quaternion.Euler(0, 90, 0) * velocity;
			}
			this.centerOff = normal.normalized * r;
		}
	}

	public Move(Vector3 velocity, float speed, float theta, float t)
		: this(velocity, Vector3.zero, speed, 0, theta, t) {
	}

	public Move(Vector3 velocity, Vector3 acceleration, 
		float speed, float theta, float t)
		: this(velocity, acceleration, speed, 0, theta, t) {
	}

	public Move(Vector3 velocity, float absAcc,
		float speed, float theta, float t)
		: this(velocity, Vector3.zero, speed, absAcc, theta, t) {
	}

	public Move(Vector3 velocity, Vector3 acceleration, float absAcc, float t)
		: this(velocity, acceleration, velocity.magnitude, absAcc, 0, t) {
	}

	public float MoveMe(Transform transform, float dt) {
		// Time used for transforming the vehicle
		float time = Mathf.Min(dt, t);

		// Translate and rotate
		transform.Translate(velocity * speed * time, Space.World);
		float rot = (speed < 0 ? -theta : theta);
		rot = theta;
		transform.RotateAround(transform.position, yAxis, rot * time);
		
		// Compute new velocities, accelerations, anggles and what not
		velocity = Quaternion.Euler(0, rot * time, 0) * velocity;
		velocity = (velocity + acceleration).normalized;
		speed += absAcc * time;

		t -= time;
		return dt - time;
	}

	private bool Obstructed(IEnumerable<Polygon> polys, Vector3 startPos) {
		Vector3 newPoint = this.PredictPosition(startPos);
		Vector2 sp = new Vector2(startPos.x, startPos.z);
		Vector2 np = new Vector2(newPoint.x, newPoint.z);
		
		if (theta == 0.0f) {		// Check straight line intersection
			Edge e = new Edge(sp, np);
			foreach (Polygon p in polys) {
				if (p.Intersects(e)) {
					return true;
				}
			}
		} else {				// Check arc intersection
			// Center of turning circle
			Vector3 center = startPos + centerOff;
			Vector2 cp = new Vector2(center.x, center.z);
			Vector2 cenToS = sp - cp;
			Vector2 cenToN = np - cp;

			// Angles of the arc
			float a1, a2;
			if (theta < 0) {
				a1 = Arc.Angle(Vector2.right, cenToS);
				a2 = Arc.Angle(Vector2.right, cenToN);
			} else {
				a1 = Arc.Angle(Vector2.right, cenToN);
				a2 = Arc.Angle(Vector2.right, cenToS);
			}
			
			// Check if arc intersects with any of the polygons
			Arc arc = new Arc(cp, r, a1, a2);
			foreach (Polygon p in polys) {
				if (p.Intersects(arc)) {
					return true;
				}
			}
		}
		return false;
	}

	// Fix this
	private Vector3 PredictPosition(Vector3 pos) {
		if (theta == 0) {
			return pos + velocity * speed * t;
		}

		// TODO this wont work with acceleration
		Vector3 center = pos + centerOff;
		float angle = theta * t;
		Vector3 endVector = - (Quaternion.Euler(0, angle, 0) * centerOff);
		return center + endVector;
	}

	public static bool Obstructed(IEnumerable<Move> moves,
		IEnumerable<Polygon> polys, Vector3 startPos) {
		
		foreach (Move m in moves) {
			if (m.Obstructed(polys, startPos)) {
				return true;
			}
			startPos = m.PredictPosition(startPos);
		}
		return false;
	}

	override public string ToString() {
		return string.Format("{0} {1} {2} {3}", velocity, speed,
			Mathf.Sign(speed) * theta, t);
	}

	public Move Copy() {
		return new Move(velocity, acceleration, speed, absAcc,
			Mathf.Sign(speed) * theta, t);
	}
}
