using UnityEngine;
using UM_AI;
using UModules;

public partial class Squid : Agent
{
	protected void SetupStateMachine()
	{
		SM.AddState(new Wander(this));
		SM.AddState(new Seek(this));
		SM.AddState(new Flee(this));
		SM.DefaultState = SM.GetState(nameof(Wander));
		SM.ChangeState(nameof(Wander));
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
			Agent.Steering.SetFlag(SteeringType.Wander);
		}

		public override void Update(float deltaTime)
		{
		}

		public override void Exit(StateArgs args)
		{
			#if UNITY_EDITOR 
			if(Agent.debug) {Debug.LogFormat("Exit {0}",nameof(Wander));}
			#endif
			Agent.Steering.UnsetFlag(SteeringType.Wander);
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


}