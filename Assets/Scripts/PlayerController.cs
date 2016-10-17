using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public abstract class PlayerSignal : MonoBehaviour {
	public abstract float Stride {get;}
	public abstract IObservable<Vector3> Walked {get;}
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : PlayerSignal {

	private float _WalkSpeed = 5f;
	private float _RunSpeed = 10f;
	private CharacterController _Character;
	private Subject<Vector3> walked;
	public override IObservable<Vector3> Walked {
		get {
			return walked;
		}
	}

	private float stride = 2.5f;
	public override float Stride {
		get {
			return stride;
		}
	}

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		_Character = GetComponent<CharacterController>();
		walked = new Subject<Vector3> ().AddTo(this);
	}
	// Use this for initialization
	void Start () {
		var inputs = Inputs.Instance;
		inputs.Movement
		.Where (v => v != Vector2.zero)
		.Subscribe (inputMovement => {
			var inputVelocity = inputMovement * (inputs.Run.Value ? _RunSpeed : _WalkSpeed);

			var playerVelocity = inputVelocity.x * transform.right + inputVelocity.y * transform.forward;
			var distance = playerVelocity * Time.fixedDeltaTime;
			_Character.Move(distance); 
			walked.OnNext(_Character.velocity * Time.fixedDeltaTime);
		}).AddTo(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
