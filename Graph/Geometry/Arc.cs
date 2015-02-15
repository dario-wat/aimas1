using UnityEngine;
using System.Collections;
using System;

/**
	Defines arc with center, radius, start and end angle.
	Start and end angle are calculated from the zero angle defined
	by vector (1, 0), in other words, vector right. Start angle
	is always smaller than end angle.
**/
public class Arc {

	// Eps
	private static float eps = 0.0001f;

	// Arc center
	private Vector2 center;

	// Radius
	private float r;

	// Star and end angle
	private float sAngle;
	private float eAngle;


	// Converts both angles so they are inside [0, 360] interval
	// It means that if the parameters are sAngle = 90, eAngle = 540,
	// the actual arc will be between 90 and 180, and not full circle
	public Arc(Vector2 center, float r, float sAngle, float eAngle) {
		this.center = center;
		this.r = r;
		this.sAngle = sAngle - Mathf.Floor(sAngle / 360.0f) * 360.0f;
		this.eAngle = eAngle - Mathf.Floor(eAngle / 360.0f) * 360.0f;
	}

	// Check if the edge (line segment) intersects with arc (circle segment)
	public bool Intersects(Edge e) {
		Line l = new Line(e);

		// Intersecting points
		float x1, x2, y1, y2;

		// Check if they intersect first (circle and line)
		if (l.b == 0.0f) {		// Line is vertical
			x1 = -l.c / l.a;
			x2 = x1;
			float ydet = r * r - (x1 - center.x) * (x1 - center.x);
			if (ydet <= 0) {
				return false;
			}
			float ysqrt = Mathf.Sqrt(ydet);
			y1 = ysqrt + center.y;
			y2 = -ysqrt + center.y;
		} else {				// Line not vertical
			// Number crunching
			// It looks ugly as hell but it needs to have as few operations
			// as possible to make it run as fast as possible
			float lbSqr = l.b * l.b;
			float a = l.a * l.a + lbSqr;
			float b = 2 * (-lbSqr * center.x + l.a * l.c + l.a * l.b * center.y);
			float c = center.x * center.x * lbSqr + l.c * l.c
				+ center.y * center.y * lbSqr + 2 * l.c * center.y * l.b
				- r * r * lbSqr;

			// Discriminant of quadratic equation
			float d = b * b - 4 * a * c;
			if (d <= 0) {
				// No intersection between line and circle, touch is ok
				return false;
			}

			// Finding intersecting points
			float dSqrt = Mathf.Sqrt(d);
			float twoA = 2 * a;
			x1 = (-b + dSqrt) / twoA;
			x2 = (-b - dSqrt) / twoA;
			float k = -l.a / l.b;
			float m = -l.c / l.b;
			y1 = k * x1 + m;
			y2 = k * x2 + m;
		}

		// Easier check if the point is on arc and edge
		Vector2 p1 = new Vector2(x1-center.x, y1-center.y);
		Vector2 p2 = new Vector2(x2-center.x, y2-center.y);
		float a1 = Angle(Vector2.right, p1);
		float a2 = Angle(Vector2.right, p2);

		float xs = Mathf.Min(e.v.x, e.w.x);
		float xb = Mathf.Max(e.v.x, e.w.x);
		float ys = Mathf.Min(e.v.y, e.w.y);
		float yb = Mathf.Max(e.v.y, e.w.y);

		bool sLessE = sAngle < eAngle;

		// Yeah, basically checking if any of the 2 found points
		// is at the same time on the arc and on the edge
		return ((sLessE && a1 > sAngle && a1 < eAngle
			|| !sLessE && (a1 >= sAngle || a1 <= eAngle))
			&& x1 + eps >= xs && x1 - eps <= xb
			&& y1 + eps >= ys && y1 - eps <= yb)
			||
			((sLessE && a2 > sAngle && a2 < eAngle
			|| !sLessE && (a2 >= sAngle || a2 <= eAngle))
			&& x2 + eps >= xs && x2 - eps <= xb
			&& y2 + eps >= ys && y2 - eps <= yb);
	}

	// Return angle between two vectors in 0-360 interval going counter-lockwise
	public static float Angle(Vector2 a, Vector2 b) {
		float sign = Mathf.Sign(Vector3.Cross(a, b).z);
		if (sign < 0) {
			return 360 - Vector2.Angle(a, b);
		}
		return Vector2.Angle(a, b);
	}

}
