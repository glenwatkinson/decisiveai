using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public enum TileState{InDeck, InHand, InPlacement, OnGrid};
    TileState currentState = TileState.InDeck;

    public int tileValue;
    public int sumValue;

    TextMesh valueText;
    MeshRenderer blockMesh;
    Collider collider;

	// Use this for initialization
	void Awake () {
        valueText = this.transform.Find("Value Text").GetComponent<TextMesh>();
        blockMesh = this.GetComponent<MeshRenderer>();
        collider = this.GetComponent<Collider>();
	}

    public void SetTileState(TileState setTo)
    {
        switch (setTo)
        {
            case TileState.InDeck:
                blockMesh.enabled = false;
                valueText.text = "";
                collider.enabled = false;
                break;
            case TileState.InHand:
                blockMesh.enabled = true;
                valueText.text = "" + tileValue;
                collider.enabled = true;
                break;
            case TileState.InPlacement:
                blockMesh.enabled = true;
                valueText.text = "" + tileValue;
                collider.enabled = true;
                break;
            case TileState.OnGrid:
                blockMesh.enabled = true;
                valueText.text = "" + tileValue + System.Environment.NewLine + sumValue;
                collider.enabled = false;
                break;
        }
        currentState = setTo;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
