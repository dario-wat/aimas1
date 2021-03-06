﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour, IPathListener {

	// Marker object has to be set
	public GameObject markerObject;

	// A list of markers
	private List<GameObject> markers = new List<GameObject>();

	// Parent game object for storing markers
	private GameObject parentObject = null;

	// PathGenerator object that is used to render the path
	private PathGenerator pg = PathGenerator.instance;


	// Attaches itself to the pathgenerator before anything starts
	void Awake() {
		// Register itself as observer
		pg.Attach(this);
	}

	// Use this for initialization
	void Start () {
		// You have to set marker object in the IDE (Inspector)
		if (!markerObject) {
			Debug.LogError("You have to set marker object.", markerObject);
		}
	}

	// Removes the first marker from the list and destroys it
	public void UpdateReachedIndex(int idx) {
		markers[idx].SetActive(false);
	}

	// Activates all markers
	public void UpdateResetted() {
		foreach (GameObject go in markers) {
			go.SetActive(true);
		}
	}

	// Initializes new path, clears all markers and adds new ones
	public void UpdateInitialized() {
		if (parentObject != null) {		// Clear last game object
			Destroy(parentObject);
		}

		// Create parent
		parentObject = new GameObject("Path Markers");

		// Marker reinitialize
		markers.Clear();

		// Create marker for each point, set active and add to parent
		foreach (Vector3 p in pg.points) {
			// Make a clone of the marker in the correct position
			GameObject tempMarker = (GameObject) Instantiate(
				markerObject, p, Quaternion.identity);
			tempMarker.SetActive(true);
			tempMarker.transform.parent = parentObject.transform;
			markers.Add(tempMarker);
		}
	}
	
}
