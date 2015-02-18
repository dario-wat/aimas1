using UnityEngine;
using System.Collections;
public class Tangents  {

	public static float RotationAngle(Vector3 from, Vector3 to) {
		// Cross product to find out if I have to back up or go forward
		float angle = Vector3.Angle(from, to);          // Angle between
		
		Vector3 cross = Vector3.Cross(from, to);        // Cross product
		if (angle == 180.0f) {          // To make sure that it turns if it's 180
			cross.y = 1.0f;
		}
		return Mathf.Sign(cross.y) * angle;
	}
	
	
	/* Returns the parallel tangent points on two circles with radius r centered at c1 and c2 */
	public static Vector3 [] parallelTangentPoints(Vector3 c1, Vector3 c2, float r) {
		Vector3 centerLine = c2 - c1;
		Vector3 normal = (Quaternion.Euler (0, 90, 0) * centerLine).normalized;
		Vector3 s1 = c1 + r * normal;
		Vector3 s2 = c1 - r * normal;
		Vector3 e1 = c2 + r * normal;
		Vector3 e2 = c2 - r * normal;
		Vector3[] returnValues = {s1,e1,s2,e2};
		return returnValues;
	}
	
	/* Returns the intersecting tangent points on two circles with radius r centered at c1 and c2 */
	public static Vector3 [] intersectingTangentPoints(Vector3 c1, Vector3 c2, float r) {
		Vector3 centerLine = c2 - c1;
		float d = 0.5f * centerLine.magnitude;
		float l = Mathf.Sqrt (d*d - r*r);
		
		float v = Mathf.Atan (l / r);
		v = v * 180 / Mathf.PI;
		//Debug.Log (v);
		//Debug.Log (Quaternion.Euler (0, v, 0));
		Vector3 shift1 = r * (Quaternion.Euler (0, v, 0) * centerLine.normalized) ;
		Vector3 shift2 = r *  (Quaternion.Euler (0, -v, 0) * centerLine.normalized) ;
		
		//Debug.Log ("---------"+ shift1+"----------");
		//Debug.Log (centerLine + ", "+Quaternion.Euler (0, v, 0) * centerLine + ", " + Quaternion.Euler (0, -v, 0) * centerLine);
		
		Vector3 s1 = c1 + shift1;
		Vector3 s2 = c1 + shift2;
		Vector3 e1 = c2 - shift1;
		Vector3 e2 = c2 - shift2;
		
		Vector3[] returnValues = {s1,e1,s2,e2};
		return returnValues;
	}
}