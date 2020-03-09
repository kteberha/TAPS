using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UM_AI;
using UModules;
using SensorToolkit;

public class Squid : Agent
{
	const string PLAYERTAG = "Player";
	const string PACKAGETAG = "Package";
	private readonly string[] AllowedTags = {PLAYERTAG,PACKAGETAG};

	public float roamRange = 50f;
	public float seekTime = 30f;
	[Range(0f,1f),Tooltip("Interest in pursuing packages over Player")]
	public float packageInterest = 0.5f;

	[SerializeField,Readonly]
	private Tentacle[] tentacles;

	[SerializeField] protected bool debug = false;

	private SteeringRig2D _steeringRig;
	private SteeringRig2D Steering {get {return this.Get<SteeringRig2D>(ref _steeringRig);} }

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
			Steering.Destination = _targetPos;
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
		SM.AddState(new Wander(this));
		SM.AddState(new Seek(this));
		SM.AddState(new Flee(this));
		SM.DefaultState = SM.GetState(typeof(Wander));
		SM.ChangeState(typeof(Wander));
		
		tentacles = GetComponentsInChildren<Tentacle>();

		RangeSensor.EnableTagFilter = true;
		RangeSensor.AllowedTags = AllowedTags;
		RangeSensor.OnDetected.AddListener(Detection);
		RangeSensor.OnLostDetection.AddListener(LostDetection);
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
		return Physics2D.CircleCastAll(pos,30f,Vector2.zero).Select(x=>x.transform.gameObject).Where(y=>y.tag=="Package");
	}

	public bool ToWander() => !playerDetected && SM.TimeElapsed.Seconds >= seekTime;
	public bool ToSeek() => playerDetected;

	public class Wander : State<Squid>
	{
		public Wander(Agent agent) : base(agent)
		{
			#if UNITY_EDITOR
			color = Color.green;
			#endif
			this.SetTransitional(Agent.ToSeek,nameof(Seek));
		}

		public override void Enter(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Wander));}
			#endif
			Agent.TargetPos = Agent.transform.position.XY().RandomInRadius(Agent.roamRange);
		}

		public override void Update(float deltaTime)
		{
			if (Agent.DistanceToTarget <= Agent.Steering.StoppingDistance) 
			{
				Agent.TargetPos = Agent.transform.position.RandomInRadius(Agent.roamRange);
			}
		}

		public override void Exit(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Wander));}
			#endif
		}
	}

	public class Seek : State<Squid>
	{
		public Seek(Agent agent) : base(agent)
		{
			#if UNITY_EDITOR
			color = Color.yellow;
			#endif
			this.SetTransitional(Agent.ToWander,nameof(Wander));
		}

		public override void Enter(StateArgs args)
		{
			#if UNITY_EDITOR
			if(Agent.debug) Debug.LogFormat("Enter {0}",nameof(Seek));
			#endif
		}

		public override void Update(float deltaTime)
		{
		}

		public override void Exit(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Seek));}
			#endif
		}
	}

	public class Flee : State<Squid>
	{
		public Flee(Agent agent) : base(agent)
		{
			#if UNITY_EDITOR
			color = Color.red;
			#endif
		}

		public override void Enter(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Flee));}
			#endif
			
		}

		public override void Exit(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Flee));}
			#endif
		}
	}

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
