using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class Inputs : MonoBehaviour {

	public IObservable<Vector2> Movement {get; private set;}
	

	// Use this for initialization
	void Awake () {
		Movement = this.FixedUpdateAsObservable ()
			.Select(_ => {
				var x = Input.GetAxis("Horizontal");
				var y = Input.GetAxis("Vertical");
				return new Vector2(x,y).normalized;
			});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
