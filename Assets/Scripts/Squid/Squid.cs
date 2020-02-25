using System;
using System.Linq;
using UnityEngine;
using UM_AI;
using UModules;
using SensorToolkit;

public class Squid : Agent
{
	private readonly string[] AllowedTags = {"Player","Crate"};

	public float roamRange = 50f;
	public float seekTime = 30f;

	[SerializeField,Readonly]
	private Tentacle[] tentacles;

	[SerializeField] protected bool debug = false;

	public GameObject Target {get; protected set;}

	private Vector2 targetPos;

	private SteeringRig2D _steeringRig;
	private SteeringRig2D SteeringRig {get {return this.Get<SteeringRig2D>(ref _steeringRig);} }

	private RangeSensor2D _rangeSensor;
	private RangeSensor2D RangeSensor {get {return this.Get<RangeSensor2D>(ref _rangeSensor);} }

	public override void Initialize()
	{
		base.Initialize();
		SM.AddState(new Wander(this));
		SM.AddState(new Seek(this));
		SM.AddState(new Grab(this));
		
		tentacles = GetComponentsInChildren<Tentacle>();

		RangeSensor.EnableTagFilter = true;
		RangeSensor.AllowedTags = AllowedTags;
		RangeSensor.OnDetected.AddListener((x)=>
		{
			Target = x;
			foreach (Tentacle t in tentacles) t.Target = x.transform;
		});
		RangeSensor.OnLostDetection.AddListener((x)=>Target=null);
	}

	protected override void UpdateAgent()
	{
		base.UpdateAgent();
		RangeSensor.Pulse();
	}

	public class Wander : State<Squid>
	{
		public Wander(Agent agent) : base(agent)
		{
			#if UNITY_EDITOR
			color = Color.green;
			#endif
		}

		public override void Enter(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Wander));}
			#endif
			Agent.targetPos = Agent.transform.position.RandomInRadius(Agent.roamRange);
		}

		public override void Update(float deltaTime)
		{
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

	public class Grab : State<Squid>
	{
		public Grab(Agent agent) : base(agent)
		{
			#if UNITY_EDITOR
			color = Color.red;
			#endif
		}

		public override void Enter(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Grab));}
			#endif
		}

		public override void Exit(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Grab));}
			#endif
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (debug)
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(targetPos,Vector3.one*3f);
			}
		}
	}
	#endif
}
