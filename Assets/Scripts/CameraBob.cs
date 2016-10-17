using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Camera))]
public class CameraBob : MonoBehaviour {

	public PlayerSignal _PlayerSignal;
	public float _WalkBobMagnitude = 0.05f;
	public float _RunBobMagnitude = 0.10f;
	public AnimationCurve _Bob = new AnimationCurve(
			new Keyframe(0.00f,  0f),
			new Keyframe(0.25f,  1f),
			new Keyframe(0.50f,  0f),
			new Keyframe(0.75f, -1f),
			new Keyframe(1.00f,  0f));

	private Camera _View;
	private Vector3 _InitialPosition;


	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		_View = GetComponent<Camera> ();
		_InitialPosition = _View.transform.localPosition;
	}

	// Use this for initialization
	void Start () {
		var distance = 0f;
		_PlayerSignal.Walked.Subscribe(w => {
      		distance += w.magnitude;
      		distance %= _PlayerSignal.Stride;
      		var magnitude = Inputs.Instance.Run.Value ? _RunBobMagnitude : _WalkBobMagnitude;
      		var deltaPos = magnitude * _Bob.Evaluate(distance / _PlayerSignal.Stride) * Vector3.up;
     	 	_View.transform.localPosition = _InitialPosition + deltaPos;
   		}).AddTo(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
