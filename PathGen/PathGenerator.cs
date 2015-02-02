﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathGenerator {

	// A list of points to describe path for vehicles to follow
	public readonly List<Vector3> points = new List<Vector3>();


	#region Singleton

	// Publix readonly instance
	public static readonly PathGenerator instance = new PathGenerator();

	
	// Private constructor 
	private PathGenerator() {
	}

	// Use this for initialization
	// It sets the path as the new given iterator of points
	public static void Init (IEnumerable<Vector3> pointsEnum) {
		instance.points.Clear();
		foreach (Vector3 v in pointsEnum) {
			instance.points.Add(v);
		}
		instance.FireInitialized();
	}

	#endregion



	#region Iterator

	// Index of the current point in the list
	private int currIdx = 0;

	// Checks if there is more to iterate
	public bool HasMore {
		get { return currIdx < points.Count; }
	}

	// Returns current point
	public Vector3 Current {
		get {
			if (!HasMore) {
				throw new InvalidOperationException("No more points.");
			}
			return points[currIdx];
		}
	}

	// Moves to next point on path
	public void Next() {
		if (!HasMore) {
			throw new InvalidOperationException("No more points.");
		}

		FireReachedIndex(currIdx);		// Fire observers
		currIdx++;
	}

	// Sets points enumerator to initial position
	public void Reset() {
		currIdx = 0;
		FireResetted();
	}

	#endregion



	#region Observer
	
	// List of observers
	private List<IPathListener> observers = new List<IPathListener>();

	// Add observer to list
	public void Attach(IPathListener l) {
		observers.Add(l);
	}

	// Remove observer from the list
	public void Detach(IPathListener l) {
		observers.Remove(l);
	}

	// Fires reaching indexed point
	private void FireReachedIndex(int idx) {
		foreach (IPathListener l in observers) {
			l.UpdateReachedIndex(idx);
		}
	}

	// Fires resetting of the path
	private void FireResetted() {
		foreach (IPathListener l in observers) {
			l.UpdateResetted();
		}
	}
	
	// Fires initialization of the path
	private void FireInitialized() {
		foreach (IPathListener l in observers) {
			l.UpdateInitialized();
		}	
	}

	#endregion

}
