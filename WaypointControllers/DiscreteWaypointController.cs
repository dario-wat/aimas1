using UnityEngine;
using System.Collections;

public class DiscreteWaypointController : DiscreteMM {

	override protected void Start() {
		// Base call
		base.Start();
		// Check arguments
		require(F > 0, "F has to be greater than 0");
		// Initializes hardcoded path
		PathGenerator.InitSample();
	}
}
