using UnityEngine;
using UM_AI;
using UModules;

public partial class Squid : Agent
{
	protected void SetupStateMachine()
	{
		SM.AddState(new Wander(this));
		SM.AddState(new Seek(this));
		SM.AddState(new Eat(this));
		SM.AddState(new Flee(this));
		SM.SetTransitionEvent(nameof(ToSeek),nameof(Seek),StateArgs.Empty);
		SM.SetTransitionEvent(nameof(ToFlee),nameof(Flee),StateArgs.Empty);
		SM.SetTransition(nameof(Seek),nameof(Wander),StateArgs.Empty,SeekEnough);
		SM.SetTransition(nameof(Flee),nameof(Wander),StateArgs.Empty,FleeEnough);
		SM.SetTransition(nameof(Seek),nameof(Eat),StateArgs.Empty,EatTest);
		SM.SetTransition(nameof(Eat),nameof(Wander),StateArgs.Empty,()=>!EatTest());
		SM.DefaultState = SM.GetState(nameof(Wander));
		SM.ChangeState(nameof(Wander));
		TargetFound.AddListener(ToSeek);
		PackageHit.AddListener(ToFlee);
	}

	#if UNITY_EDITOR
	[DebugGUIPrint]
	#endif
	string currentState => SM.CurrentState.GetType().Name;

	public bool SeekEnough() => (!TargetingPlayer || Target==null) && SM.TimeElapsed.Seconds >= seekTime;
	public bool FleeEnough() => !TargetingPlayer && SM.TimeElapsed.Seconds >= fleeTime;
	public void ToSeek(string tag)
	{
		// don't seek if fleeing
		if (SM.CurrentStateName != nameof(Flee))
		{
			SM.TriggerEvent(nameof(ToSeek));
		}
	}
	public void ToFlee() => SM.TriggerEvent(nameof(ToFlee));
	public bool EatTest() => activeTentacle?.Holding??false;

	public class Wander : State<Squid>
	{
		public Wander(Agent agent) : base(agent)
		{
			// #if UNITY_EDITOR
			// color = Color.green;
			// #endif	
		}

		public override void Enter(StateArgs args)
		{
			// #if UNITY_EDITOR 
			// if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Wander));}
			// #endif
			Agent.Steering.SetFlag(SteeringType.Wander);
		}

		public override void Update(float deltaTime)
		{
		}

		public override void Exit(StateArgs args)
		{
			// #if UNITY_EDITOR 
			// if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Wander));}
			// #endif
			Agent.Steering.UnsetFlag(SteeringType.Wander);
		}
	}

	public class Seek : State<Squid>
	{
		public Seek(Agent agent) : base(agent)
		{
			// #if UNITY_EDITOR
			// color = Color.red;
			// #endif
		}

		public override void Enter(StateArgs args)
		{
			// #if UNITY_EDITOR
			// if(Agent.debug) Debug.LogFormat("Enter {0}",nameof(Seek));
			// #endif
			Agent.Steering.SetFlag(SteeringType.Pursuit);
		}

		public override void Update(float deltaTime)
		{
			Agent.UpdateTentacleTarget();
		}

		public override void Exit(StateArgs args)
		{
			// #if UNITY_EDITOR 
			// if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Seek));}
			// #endif
			Agent.Steering.UnsetFlag(SteeringType.Pursuit);
			// Make sure Target is unset when exiting Seek
			Agent.Target = null;
		}
	}

	public class Eat : State<Squid>
	{
		public Eat(Agent agent) : base(agent)
		{
			// #if UNITY_EDITOR
			// color = Color.blue;
			// #endif
		}

		public override void Enter(StateArgs args)
		{
			// #if UNITY_EDITOR 
			// if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Eat));}
			// #endif
			Agent.Steering.UnsetFlag(SteeringType.Avoidance);
			Agent.activeTentacle.Target = Agent.mouth;
		}

		public override void Update(float deltaTime)
		{
			Debug.Log(distanceToMouth);
			if (distanceToMouth <= Agent.mininumEatDistance)
			{
				EatTarget();
			}
		}

		public void EatTarget()
		{
			// TODO: Eating Animation
			Agent.activeTentacle?.HeldObject?.SendMessage("Eaten",SendMessageOptions.DontRequireReceiver);
			Agent.activeTentacle?.HandDetach();
			Agent.activeTentacle.Target = null;
		}

		float distanceToMouth => Vector2.Distance(Agent.mouth.position,Agent.activeTentacle.HeldObject.position);

		public override void Exit(StateArgs args) 
		{
			// #if UNITY_EDITOR 
			// if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Eat));}
			// #endif
			Agent.Steering.SetFlag(SteeringType.Avoidance);
			Agent.activeTentacle.Target = null;
		}
	}

	public class Flee : State<Squid>
	{
		public Flee(Agent agent) : base(agent)
		{
			// #if UNITY_EDITOR
			// color = Color.yellow;
			// #endif
		}

		public override void Enter(StateArgs args)
		{
			// #if UNITY_EDITOR 
			// if(Agent.debug) {Debug.LogFormat("Enter {0}",nameof(Flee));}
			// #endif
			Agent.Steering.SetFlag(SteeringType.Flee);
			Agent.activeTentacle?.HandDetach();
		}

		public override void Exit(StateArgs args)
		{
			// #if UNITY_EDITOR 
			// if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Flee));}
			// #endif
			Agent.Steering.UnsetFlag(SteeringType.Flee);
		}
	}

}