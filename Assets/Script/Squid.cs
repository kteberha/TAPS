using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UM_AI;
using UModules;

public class Squid : Agent
{
	private readonly string[] AllowedTags = {"Player","Crate"};

	public override void Initialize()
	{
		base.Initialize();
		this.SM.AddState(new Wander(this.SM));
		this.SM.AddState(new Seek(this.SM));
		this.SM.AddState(new Attack(this.SM));
		this.SM.StartState = this.SM.GetState(typeof(Wander));
		this.SM.Setup();
	}

	protected override void UpdateAgent()
	{
		base.UpdateAgent();
	}

	public class Seek : State<Squid>
	{
		public Seek(StateMachine sm) : base(sm)
		{ 
			StateColor = Color.yellow;
			this.Transitions = new Transition[] {};
		}

		public override void Enter()
		{
		}

		public override void Update()
		{
		}

		public override void Exit() 
		{
		}
	};

	public class Wander : State<Squid>
	{
		public Wander(StateMachine sm) : base(sm)
		{
			StateColor = Color.green;
			this.Transitions = new Transition[] {};
		}

		public override void Enter()
		{
		}

		public override void Update()
		{
		}

		public override void Exit()
		{
		}
	}

	public class Attack : State<Squid>
	{
		public Attack(StateMachine sm) : base(sm)
		{
			StateColor = Color.red;
			this.Transitions = new Transition[] {};
		}
	}
}
