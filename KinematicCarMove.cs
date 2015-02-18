using UnityEngine;
using System.Collections.Generic;

public class KinematicCarMove : Move {

	// Angular velocity
	public float omega { get; private set; }

	// Normalized velocity, direction of moving
	public Vector3 velocity { get; private set; }

	// Speed, can be negative when moving backwards
	private float speed;

	// Rotation radius
	private float r;

	// Center offset for faster and easier center finding
	private Vector3 centerOff;


	public KinematicCarMove(Vector3 velocity, float speed, float omega,
		float t) : base(t) {
		
		this.velocity = velocity.normalized;
		this.speed = speed;
		this.omega = Mathf.Sign(speed) * omega;
		this.r = Mathf.Abs(speed) / (Mathf.Abs(omega) * Mathf.PI / 180.0f);

		// Setting centerOff
		if (omega != 0) {
			Vector3 normal;
			if (omega < 0) {	// Turning in left circle
				normal = Quaternion.Euler(0, -90, 0) * velocity;
			} else {			// Turning in right circle
				normal = Quaternion.Euler(0, 90, 0) * velocity;
			}
			this.centerOff = normal.normalized * r;
		}
	}

	// Translates and rotates, as kinematic car would want
	override protected void MoveTransform(Transform transform, float time) {
		// Translate and rotate
		transform.Translate(velocity * speed * time, Space.World);
		transform.RotateAround(transform.position, yAxis, omega * time);
		
		// Compute new velocity
		velocity = Quaternion.Euler(0, omega * time, 0) * velocity;
	}

	// Obstructions, must check line and arc intersection
	override protected bool Obstructed(
		IEnumerable<Polygon> polys, Vector3 startPos) {
		
		Vector3 newPoint = this.PredictPosition(startPos);
		Vector2 sp = new Vector2(startPos.x, startPos.z);
		Vector2 np = new Vector2(newPoint.x, newPoint.z);

		// Length of the car
		float L = KinematicCarState.L;

		if (omega == 0.0f) {		// Check straight line intersection
			Vector2 transVec = (np - sp).normalized * L;
			Vector2 midPoint = (sp + np) / 2;
			// TODO midPoint not necessary if you fix arc
			Edge e = new Edge(sp, np + transVec);		// TODO check correctness
			foreach (Polygon p in polys) {
				if (p.Intersects(e) || p.IsInside(midPoint)) {
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
			if (omega < 0) {
				a1 = Arc.Angle(Vector2.right, cenToS);
				a2 = Arc.Angle(Vector2.right, cenToN);
			} else {
				a1 = Arc.Angle(Vector2.right, cenToN);
				a2 = Arc.Angle(Vector2.right, cenToS);
			}
			/*Vector2 normal = Quaternion.Euler(0, 90, 0) * (sp - np);
			float na = (a1 + a2) / 2;
			Vector2 aaa2 = cp + normal * r;
			Vector2 aaa3 = cp - normal * r;
			float aaaa2 = Arc.Angle(Vector2.right, aaa2);
			float aaaa3 = Arc.Angle(Vector2.right, aaa3);
			Vector2 ffff = aaa2;
			if (a1 < a2) {
				if (aaaa2 > a1 && aaaa2 < a2) {
					ffff = aaa2;
				}
				if (aaaa3 > a1 && aaaa3 < a2) {
					ffff = aaa3;
				}
			} else {
				if (aaaa2 > a1 && aaaa2 < a2 && aaaa2 < 360 && aaaa2 > 0) {
					ffff = aaa2;
				}
				if (aaaa3 > a1 && aaaa3 < a2 && aaaa3 < 360 && aaaa3 > 0) {
					ffff = aaa3;
				}
			}*/
			// Checking the front of the vehicle
			Vector3 transVec = velocity.normalized * L;
			Vector3 cenToFront = -centerOff + transVec;
			float frontR = cenToFront.magnitude;
			float diffAngle = Tangents.RotationAngle(-centerOff, cenToFront);
			
			// Check if arc intersects with any of the polygons
			Arc arc = new Arc(cp, r, a1, a2);
			// TODO check correctness, if it is - or + diffAngle
			Arc frontArc = new Arc(cp, frontR, a1 - diffAngle, a2 - diffAngle);
			foreach (Polygon p in polys) {
				if (p.Intersects(arc) || p.Intersects(frontArc)) {
					return true;
				}
			}
		}
		return false;
	}

	public bool Obstructed2(
		IEnumerable<Polygon> polys, Vector3 startPos) {
		return Obstructed(polys, startPos);
	}

	// Predict the point, depends if its arc or line
	override protected Vector3 PredictPosition(Vector3 pos) {
		if (omega == 0) {
			return pos + velocity * speed * t;
		}

		Vector3 center = pos + centerOff;
		float angle = omega * t;
		Vector3 endVector = - (Quaternion.Euler(0, angle, 0) * centerOff);
		return center + endVector;
	}

	// Creates a copy
	override public Move Copy() {
		return new KinematicCarMove(velocity, speed,
			Mathf.Sign(speed) * omega, t);
	}

	// For debugging
	override public string ToString() {
		return string.Format(
			"Velocity: {0}, Speed: {1}, Omega: {2}, t: {3}, Left: {4}, r: {5}",
			velocity, speed, Mathf.Sign(speed) * omega, t, omega < 0, r);
	}

}
