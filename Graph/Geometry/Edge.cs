using UnityEngine;
using System.Collections;
using System;

/**
	Class describes edges or in other words line segments. Includes
	methods which are used for graph construction.
*/
public class Edge {

	// Const to ensure that the algorithms finds proper intersections
	private const float e = 0.0001f;

	// Two vertices that define the edge
	public Vector2 v { get; private set; }
	public Vector2 w { get; private set; }

	// Constructor
	public Edge(Vector2 v, Vector2 w) {
		this.v = v;
		this.w = w;
	}

	// Checks if two edges intersect
	// Edges do not intersect if they lie on one another
	// or if they have a common vertex
	public bool Intersect(Edge other) {
		// Checking for common vertex
		if (	this.v.Equals(other.v) || this.v.Equals(other.w)
			|| 	this.w.Equals(other.v) || this.w.Equals(other.w)) {
			
			return false;
		}

		// Create lines
		Line l1 = new Line(this);
		Line l2 = new Line(other);

		// Find intersect
		Vector2? tmp = l1.Intersection(l2);
		if (!tmp.HasValue) {	// No intersection
			return false;
		}
		Vector2 p = tmp.Value;

		// Making this for easier last return
		Vector2 v1 = this.v;
		Vector2 w1 = this.w;
		Vector2 v2 = other.v;
		Vector2 w2 = other.w;

		// Complicated last return
		return 	p.x+e >= Math.Min(v1.x, w1.x) && p.x-e <= Math.Max(v1.x, w1.x)
			&&	p.x+e >= Math.Min(v2.x, w2.x) && p.x-e <= Math.Max(v2.x, w2.x)
			&&	p.y+e >= Math.Min(v1.y, w1.y) && p.y-e <= Math.Max(v1.y, w1.y)
			&&	p.y+e >= Math.Min(v2.y, w2.y) && p.y-e <= Math.Max(v2.y, w2.y);
	}

	// Two edges are equal if they have equal vertices
	override public bool Equals(object other) {
		if (!(other is Edge)) {
			return false;
		}
		Edge o = other as Edge;
		return 	(o.v.Equals(this.v) && o.w.Equals(this.w))
			||	(o.v.Equals(this.w) && o.w.Equals(this.v));
	}

	// To use in dictionaries
	override public int GetHashCode() {
		return v.GetHashCode() + w.GetHashCode();
	}

	// For debugging
	override public string ToString() {
		return "[ " + v.ToString() + ", " + w.ToString() + " ]";
	}

}
