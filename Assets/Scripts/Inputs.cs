using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public struct MoveInputs {
	public readonly Vector2 movement;
	public readonly bool jump;

	public MoveInputs (Vector2 m, bool j) {
		this.movement = m;
		this.jump = j;
	}
}

public class Inputs : MonoBehaviour {

	public static Inputs Instance {get; private set;}
	public IObservable<Vector2> Movement {get; private set;}
	public IObservable<Unit> Jump {get; private set;}
	public IObservable<MoveInputs> MoveInputs {get; private set;}
	public ReadOnlyReactiveProperty<bool> Run {get; private set;}

	// Use this for initialization
	void Awake () {
		Instance = this;
		Movement = this.FixedUpdateAsObservable ()
			.Select(_ => {
				var x = Input.GetAxis("Horizontal");
				var y = Input.GetAxis("Vertical");
				return new Vector2(x,y).normalized;
			});
		
		var runValue = false;
		Run = this.UpdateAsObservable ()
			.Where (_=> Input.GetButton ("Fire3"))
			.Do (_ => runValue = !runValue)
			.Select (_ => runValue)	
			.ToReadOnlyReactiveProperty ();
		
		Jump = this.UpdateAsObservable().Where (_ => Input.GetButton("Jump"));
		var jumpLatch = CustomObservables.Latch(this.FixedUpdateAsObservable(), Jump, false);

		MoveInputs = Movement.Zip(jumpLatch, (m, j) => new MoveInputs (m, j));

	}
}
