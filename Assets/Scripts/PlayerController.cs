using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public abstract class PlayerSignal : MonoBehaviour {
	public abstract float Stride {get;}
	public abstract IObservable<Vector3> Walked {get;}
	public abstract IObservable<Unit> Landed {get;}
	public abstract IObservable<Unit> Jumped {get;}
	public abstract IObservable<Unit> Stepped {get;}
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : PlayerSignal {

	private float _WalkSpeed = 5f;
	private float _RunSpeed = 10f;
	public float jumpSpeed = 2f;
	public float stickToGround = 5f;
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

	private Subject<Unit> landed;
	public override IObservable<Unit> Landed {
		get {
			return landed;
		}
	}

	private Subject<Unit> jumped;
	public override IObservable<Unit> Jumped {
		get {
			return jumped;
		}
	}

	private Subject<Unit> stepped;
	public override IObservable<Unit> Stepped {
		get {
			return stepped;
		}
	}

	public AudioSource audioSource;
	public AudioClip jump;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		_Character = GetComponent<CharacterController>();
		walked = new Subject<Vector3> ().AddTo(this);
		jumped = new Subject<Unit> ().AddTo (this);
		landed = new Subject<Unit> ().AddTo (this);
		stepped = new Subject<Unit> ().AddTo(this);
	}
	// Use this for initialization
	void Start () {
		var inputs = Inputs.Instance;
		_Character.Move (-stickToGround * transform.up);
		inputs.MoveInputs.Subscribe (i => {
			var wasGrounded = _Character.isGrounded;
			var verticalVelocity = 0f;
			if (i.jump && wasGrounded) {
				verticalVelocity = jumpSpeed;
				jumped.OnNext(Unit.Default);
			} else if (!wasGrounded) {
				verticalVelocity = _Character.velocity.y + (Physics.gravity.y * Time.fixedDeltaTime);
			} else {
				verticalVelocity = -Mathf.Abs (stickToGround);
			}
			var horizontalVelocity = i.movement * (inputs.Run.Value? _RunSpeed : _WalkSpeed);
			var playerVelocity = transform.TransformVector (new Vector3 (horizontalVelocity.x, verticalVelocity, horizontalVelocity.y));
			var distance = playerVelocity * Time.fixedDeltaTime;
			_Character.Move (distance);
			if (wasGrounded && _Character.isGrounded) {
				walked.OnNext (_Character.velocity * Time.fixedDeltaTime);
			}
			if (!wasGrounded && _Character.isGrounded) {
				landed.OnNext (Unit.Default);
			}
		}).AddTo(this);

		var stepDistance = 0f;
		Walked.Subscribe(w => {
			stepDistance += w.magnitude;
			if (stepDistance > Stride) {
				stepped.OnNext (Unit.Default);
			}
			stepDistance %= Stride;
		});

		Jumped.Subscribe ( _ => {
			audioSource.PlayOneShot (jump);
		}).AddTo (this);
	}
}
