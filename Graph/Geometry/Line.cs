using UnityEngine;
using System.Collections;
using System;

// Unlike Edge class, this class describes line as infinite construction
public class Line {

	// line parameters as in ax + by + c = 0
	private float a;
	private float b;
	private float c;

	// Constructs line from 2 points
	public Line(Vector2 v, Vector2 w) {
		if (v.x == w.x) {
			a = 1.0f;
			b = 0.0f;
			c = -v.x;
		} else {
			float k = (v.y - w.y) / (v.x - w.x);
			a = -k;
			b = 1.0f;
			c = k * v.x - v.y;
		}
	}

	public Line(Edge e) : this(e.v, e.w) {
	}

	// Finds intersection between this and other line
	// Returns null if matrix is singular (liner are parallel)
	public Vector2? Intersection(Line other) {
		float det = Det2(this.a, this.b, other.a, other.b);
		if (Math.Abs(det) < 10e-8f) {	// Singular matrix, no intersection
			return null;
		}

		float detX = Det2(-this.c, this.b, -other.c, other.b);
		float detY = Det2(this.a, -this.c, other.a, -other.c);
		return new Vector2(detX/det, detY/det);
	}

	// Computes 2-by-2 determinant
	//  a b
	//  c d
	private float Det2(float a, float b, float c, float d) {
		return a * d - b * c;
	}
}