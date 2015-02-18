using UnityEngine;
using System.Collections;
using System;

/**
	Class describes edges or in other words line segments. Includes
	methods which are used for graph construction.
*/
public class Edge : IGizmosDrawable {

	// Const to ensure that the algorithms finds proper intersections
	private const float eps = 0.0001f;

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

		float pxPe = p.x + eps;
		float pxMe = p.x - eps;
		float pyPe = p.y + eps;
		float pyMe = p.y - eps;

		// Complicated last return
		return 	pxPe >= Mathf.Min(v1.x, w1.x) && pxMe <= Mathf.Max(v1.x, w1.x)
			&&	pxPe >= Mathf.Min(v2.x, w2.x) && pxMe <= Mathf.Max(v2.x, w2.x)
			&&	pyPe >= Mathf.Min(v1.y, w1.y) && pyMe <= Mathf.Max(v1.y, w1.y)
			&&	pyPe >= Mathf.Min(v2.y, w2.y) && pyMe <= Mathf.Max(v2.y, w2.y);
	}

	// Draws the edge using Gizmos
	public void GizmosDraw(Color color) {
		Color tmp = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(
			new Vector3(v.x, 0.0f, v.y),
			new Vector3(w.x, 0.0f, w.y)
		);
		Gizmos.color = tmp;
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
