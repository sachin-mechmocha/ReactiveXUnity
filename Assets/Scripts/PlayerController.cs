using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

	private float _WalkSpeed = 5f;
	private CharacterController _Character;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		_Character = GetComponent<CharacterController>();	 
	}
	// Use this for initialization
	void Start () {
		var inputs = Inputs.Instance;
		inputs.Movement
		.Where (v => v != Vector2.zero)
		.Subscribe (inputMovement => {
			var inputVelocity = inputMovement * _WalkSpeed;

			var playerVelocity = inputVelocity.x * transform.right + inputVelocity.y * transform.forward;
			var distance = playerVelocity * Time.fixedDeltaTime;
			_Character.Move(distance); 
		}).AddTo(this);


		inputs.Movement
		.Where (v => v != Vector2.zero)
		.Subscribe (inputMovement => {
			Debug.Log ("Move by " + inputMovement * _WalkSpeed);
		}).AddTo (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
