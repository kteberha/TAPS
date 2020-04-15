using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UM_AI;
using UModules;
using SensorToolkit;

[System.Serializable]
public class TargetingEvent : UnityEvent<string> {};

public partial class Squid : Agent
{
	const string PLAYERTAG = "Player";
	const string PACKAGETAG = "Package";
	const string MOUTH = "Squid/Squid/Root/Mouth/TopMouth1";
	readonly string[] AllowedTags = {PLAYERTAG,PACKAGETAG};

	public float roamRange = 50f;
	public float seekTime = 30f;
	public float fleeTime = 15f;
	[Range(0f,1f),Tooltip("Interest in pursuing packages over Player.")]
	public float packageInterest = 0.5f;
	[Range(0.1f,100f),Tooltip("Mininum magnitude^2 of the velocity from package to detect hit.")]
	public float mininumPackageSpeed = 5f;
	[Range(0.1f,10f)]
	public float mininumEatDistance = 1f;

	[Header("Events")]
	public UnityEvent PackageHit = new UnityEvent();
	public TargetingEvent TargetFound = new TargetingEvent();
	public TargetingEvent TargetLost = new TargetingEvent();

	#if UNITY_EDITOR
	[SerializeField]
	protected bool debug = false;
	#endif

	Tentacle[] tentacles;
	#if UNITY_EDITOR
	[DebugGUIPrint]
	#endif
	Tentacle activeTentacle;
	Transform mouth;

	UM_AI.SteeringRig _steeringRig;
	UM_AI.SteeringRig Steering => this.Get<UM_AI.SteeringRig>(ref _steeringRig);

	RangeSensor2D _rangeSensor;
	RangeSensor2D RangeSensor => this.Get<RangeSensor2D>(ref _rangeSensor);

	Collider2D _collider;
	Collider2D Collider => this.Get<Collider2D>(ref _collider);

	private GameObject _target;
	#if UNITY_EDITOR
	[DebugGUIPrint]
	#endif
	public GameObject Target
	{
		get
		{
			return _target;
		}
		protected set
		{
			_target = value;
			TargetPos = _target?.transform.position.XY()??Vector2.zero;
			Steering.DestinationTransform = _target?.transform;
			UpdateTentacleTarget();
		}
	}

	private Vector2 _targetPos;
	#if UNITY_EDITOR
	[DebugGUIPrint]
	#endif
	public Vector2 TargetPos
	{
		get
		{
			return _targetPos;
		}
		protected set
		{
			_targetPos = value;
			if (Target==null) Steering.Destination = _targetPos;
		}
	}

	public float DistanceToTarget => Vector3.Distance(transform.position,TargetPos);
	private bool TargetingPlayer => Target?.tag == PLAYERTAG;

	public override void Initialize()
	{
		base.Initialize();
		tentacles = GetComponentsInChildren<Tentacle>();
		mouth = transform.Find(MOUTH);
		RangeSensor.EnableTagFilter = true;
		RangeSensor.AllowedTags = AllowedTags;
		RangeSensor.OnDetected.AddListener(DetectedHandler);
		RangeSensor.OnLostDetection.AddListener(LostDetectionHandler);
		Steering.SetFlag(SteeringType.Avoidance);
		SetupStateMachine();
		#if UNITY_EDITOR
		this.gameObject.AddComponent<DebugGUI>();
		#endif
	}

	protected override void UpdateAgent()
	{
		base.UpdateAgent();
		RangeSensor.Pulse();
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.collider.tag == PACKAGETAG && coll.rigidbody.velocity.sqrMagnitude >= mininumPackageSpeed)
		{
			TargetPos = coll.collider.gameObject.transform.position;
			PackageHit.Invoke();
		}
	}

	public void DetectedHandler(GameObject go, Sensor sensor)
	{
		switch (go.tag)
		{
			case PLAYERTAG:
				Target = go;
				TargetFound.Invoke(PLAYERTAG);
			break;
			case PACKAGETAG:
				if (Random.value <= packageInterest)
				{
					Target = go;
					TargetFound.Invoke(PACKAGETAG);
				}
			break;
		}
	}

	public void LostDetectionHandler(GameObject go, Sensor sensor)
	{
		switch (go.tag)
		{
			case PLAYERTAG:
				if (Target==go)
				{
					Target = null;
					TargetLost.Invoke(PACKAGETAG);
				}
			break;
			case PACKAGETAG:
				if (Target==go)
				{
					Target = null;
					TargetLost.Invoke(PACKAGETAG);
				}
			break;
		}
	}

	void UpdateTentacleTarget()
	{
		if (Target != null)
		{
			if (activeTentacle != ClosestTentacle || Target != activeTentacle.Target)
			{
				activeTentacle = ClosestTentacle;
				activeTentacle.Target = Target.transform;
				activeTentacle.dynamicBone.enabled = false;
				foreach (Tentacle ten in tentacles)
				{
					if (ten != activeTentacle)
					{
						ten.Target = null;
						ten.dynamicBone.enabled = true;
					}
				}
			}
		}
		else if (!activeTentacle?.Holding??true)
		{
			activeTentacle = null;
			foreach (Tentacle ten in tentacles)
			{
				ten.Target = null;
				ten.dynamicBone.enabled = true;
			}
		}
	}

	bool LeftSide(Vector3 pos) => Vector3.Dot(transform.position-pos,transform.up) <= 0f;
	Tentacle ClosestTentacle => tentacles.Aggregate((x,y)=>
								Vector3.Distance(x.transform.position,Target.transform.position)<
								Vector3.Distance(y.transform.position,Target.transform.position)?
								x:y);

	protected IEnumerable<GameObject> PackageCast(Vector2 pos)
	{
		return Physics2D.CircleCastAll(pos,30f,Vector2.zero).Select(x=>x.transform.gameObject).Where(y=>y.tag==PACKAGETAG);
	}
}
