using System;
using System.Linq;
using UnityEngine;
using UM_AI;
using UModules;
using SensorToolkit;

public class Squid : Agent
{
	private readonly string[] AllowedTags = {"Player","Crate"};

	public float RoamRange = 50f;
	public float SeekTime = 30f;

	[SerializeField,Readonly]
	private Tentacle[] tentacles;

	[SerializeField] protected bool debug = false;

	public GameObject Target {get; protected set;}

	private Vector2 TargetPos;

	private SteeringRig2D _steeringRig;
	private SteeringRig2D SteeringRig {get {return this.Get<SteeringRig2D>(ref _steeringRig);} }

	private RangeSensor2D _rangeSensor;
	private RangeSensor2D RangeSensor {get {return this.Get<RangeSensor2D>(ref _rangeSensor);} }

	public override void Initialize()
	{
		base.Initialize();
		SM.AddState(new Wander(SM));
		SM.AddState(new Seek(SM));
		SM.AddState(new Grab(SM));
		SM.StartState = SM.GetState(typeof(Wander));
		SM.Setup();
		
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
		public Wander(StateMachine sm) : base(sm)
		{
			StateColor = Color.green;
			this.Transitions = new Transition[] {ToSeek};
		}

		public bool ToSeek(out Type state, out StackFlag flag)
		{
			state = typeof(Seek);
			flag = StackFlag.PUSH;
			return Agent.Target != null;
		}

		public override void Enter()
		{
			#if UNITY_EDITOR 
				if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Wander));}
			#endif
			Vector2 randPos = VectorExtensions.RandomOnUnitCircle()*Agent.RoamRange;
			Agent.SteeringRig.Destination = Agent.TargetPos = randPos;
		}

		public override void Update()
		{
			if (Vector3.Distance(Agent.transform.position,Agent.TargetPos) <= Agent.SteeringRig.StoppingDistance)
			{
				Vector2 randPos = VectorExtensions.RandomOnUnitCircle()*Agent.RoamRange;
				Agent.TargetPos = randPos;
				Agent.SteeringRig.Destination = Agent.TargetPos;
			}
		}

		public override void Exit()
		{
			#if UNITY_EDITOR
				if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Wander));}
			#endif
		}
	}

	public class Seek : State<Squid>
	{
		public Seek(StateMachine sm) : base(sm)
		{ 
			StateColor = Color.yellow;
			this.Transitions = new Transition[] {ToWander};
		}

		public bool ToWander(out Type state, out StackFlag flag)
		{
			state = typeof(Wander);
			flag = StackFlag.POP;
			return Agent.Target == null || (SM.TimeElapsed > Agent.SeekTime && Agent.Target == null);
		}

		public override void Enter()
		{
			Agent.TargetPos = Agent.Target.transform.position;
			Agent.SteeringRig.DestinationTransform = Agent.Target.transform;
		}

		public override void Update()
		{
			Agent.TargetPos = Agent.Target.transform.position;
		}

		public override void Exit()
		{
			Agent.SteeringRig.DestinationTransform = null;
		}
	};

	public class Grab : State<Squid>
	{
		public Grab(StateMachine sm) : base(sm)
		{
			StateColor = Color.red;
			this.Transitions = new Transition[] {};
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
				Gizmos.DrawCube(TargetPos,Vector3.one*3f);
			}
		}
	}
	#endif
}
