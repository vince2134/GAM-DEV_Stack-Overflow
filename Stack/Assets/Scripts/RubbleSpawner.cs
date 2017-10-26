using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbleSpawner : MonoBehaviour {

	[SerializeField] private GameObjectPool pool;

	// Use this for initialization
	void Start () {
		EventBroadcaster.Instance.AddObserver (EventNames.ON_REQUEST_RUBBLE, this.CreateRubble);
		this.pool.Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateRubble(Parameters parameters) {
		Vector3 pos = (Vector3) parameters.GetObjectExtra("position");
		Vector3 scale = (Vector3) parameters.GetObjectExtra("scale");
		Material stackMat = (Material)parameters.GetObjectExtra ("material");

		//GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		APoolable go = this.pool.RequestPoolable ();

		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		//go.AddComponent<Rigidbody>();

		go.GetComponent<MeshRenderer>().material = stackMat;

		//ColorMesh(go.GetComponent<MeshFilter>().mesh);
	}
}
