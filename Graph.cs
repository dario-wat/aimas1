using UnityEngine;
using System.Collections.Generic;
using System;

public class Graph {

	public static List<Vector3> findDiscrete(int xs, int zs, int xe, int ze) {
		List<Vector3> path = new List<Vector3>();
		int dx = xe - xs;
		int dz = ze - zs;

		if (dx < 0) {
			dx = Math.Abs(dx);
			for (int i = 0; i < dx; i++) {
				path.Add(Vector3.left);
			}
		} else {
			for (int i = 0; i < dx; i++) {
				path.Add(Vector3.right);
			}
		}

		if (dz < 0) {
			dz = Math.Abs(dz);
			for (int i = 0; i < dz; i++) {
				path.Add(Vector3.back);
			}
		} else {
			for (int i = 0; i < dz; i++) {
				path.Add(Vector3.forward);
			}
		}
		return path;
	}
}
