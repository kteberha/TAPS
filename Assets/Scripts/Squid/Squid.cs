using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UM_AI;
using UModules;
using SensorToolkit;

public partial class Squid : Agent
{
	const string PLAYERTAG = "Player";
	const string PACKAGETAG = "Package";
	private readonly string[] AllowedTags = {PLAYERTAG,PACKAGETAG};

	public float roamRange = 50f;
	public float seekTime = 30f;
	[Range(0f,1f),Tooltip("Interest in pursuing packages over Player.")]
	public float packageInterest = 0.5f;

	[SerializeField,Readonly]
	private Tentacle[] tentacles;

	[SerializeField] protected bool debug = false;

	private UM_AI.SteeringRig _steeringRig;
	private UM_AI.SteeringRig Steering {get {return this.Get<UM_AI.SteeringRig>(ref _steeringRig);} }

	private RangeSensor2D _rangeSensor;
	private RangeSensor2D RangeSensor {get {return this.Get<RangeSensor2D>(ref _rangeSensor);} }

	private GameObject _target;
	public GameObject Target
	{
		get
		{
			return _target;
		}
		protected set
		{
			_target = value;
			TargetPos = _target.transform.position;
			Steering.destinationTransform = gameObject.transform;
			foreach (var t in tentacles) t.Target = _target.transform;
		}
	}
	private Vector2 _targetPos;
	public Vector2 TargetPos
	{
		get
		{
			return _targetPos;
		}
		protected set
		{
			_targetPos = value;
			//Steering.Destination = _targetPos;
			#if UNITY_EDITOR
			if (debug) Debug.LogFormat("TargetPos: {0}",_targetPos);
			#endif
		}
	}

	public float DistanceToTarget
	{
		get
		{
			return Vector3.Distance(transform.position,TargetPos);
		}
	}

	//public bool PlayerDetected => RangeSensor.DetectedObjects.Any(x => x.CompareTag(PLAYERTAG));
	protected bool playerDetected = false;

	public override void Initialize()
	{
		base.Initialize();
		tentacles = GetComponentsInChildren<Tentacle>();
		RangeSensor.EnableTagFilter = true;
		RangeSensor.AllowedTags = AllowedTags;
		RangeSensor.OnDetected.AddListener(Detection);
		RangeSensor.OnLostDetection.AddListener(LostDetection);
		Steering.SetFlag(SteeringType.Avoidance);
		SetupStateMachine();
	}


	protected override void UpdateAgent()
	{
		base.UpdateAgent();
		RangeSensor.Pulse();
	}

	public void Detection(GameObject go, Sensor sensor)
	{
		switch (go.tag)
		{
			case PLAYERTAG:
				playerDetected = true;
				Target = go;
			break;
			case PACKAGETAG:
				if (Random.value <= packageInterest) Target = go;
			break;
		}
	}

	public void LostDetection(GameObject go, Sensor sensor)
	{
		switch (go.tag)
		{
			case PLAYERTAG:
				playerDetected = false;
				Target = null;
			break;
			case PACKAGETAG:

			break;
		}
	}

	public void Sprayed() => SM.ChangeState(typeof(Flee));

	protected IEnumerable<GameObject> PackageCast(Vector2 pos)
	{
		return Physics2D.CircleCastAll(pos,30f,Vector2.zero).Select(x=>x.transform.gameObject).Where(y=>y.tag==PACKAGETAG);
	}

	public bool ToWander() => !playerDetected && SM.TimeElapsed.Seconds >= seekTime;
	public bool ToSeek() => playerDetected;

	#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (debug && Application.isPlaying)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube(TargetPos,Vector3.one*3f);
		}
	}
	#endif
}
