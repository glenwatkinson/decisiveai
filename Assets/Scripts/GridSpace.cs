using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace : MonoBehaviour {

    public enum GridState{Inactive, Available, PlacementOption};
    public GridState currentState;

    MeshRenderer tileMesh;
    MeshRenderer available;
    MeshRenderer placementOption;

	// Use this for initialization
	void Awake () {
        tileMesh = this.GetComponent<MeshRenderer>();
        available = this.transform.Find("Available").gameObject.GetComponent<MeshRenderer>();
        placementOption = this.transform.Find("Placement").gameObject.GetComponent<MeshRenderer>();
	}

    public void SetGridState(GridState setTo)
    {
        switch (setTo)
        {
            case GridState.Inactive:
                tileMesh.enabled = true;
                available.enabled = false;
                placementOption.enabled = false;
                break;
            case GridState.Available:
                tileMesh.enabled = true;
                available.enabled = true;
                placementOption.enabled = false;
                break;
            case GridState.PlacementOption:
                tileMesh.enabled = true;
                available.enabled = false;
                placementOption.enabled = true;
                break;
        }
        currentState = setTo;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
