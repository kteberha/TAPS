/*
	Peter Francis
	available to use according to UM_AI/LICENSE
*/

using System;

namespace UM_AI
{

	public delegate void Transition(in IState targetState, StateArgs args);

	public interface IConditional
	{
		Func<bool> Predicate { get; }
		Action[] Actions { get; }
		bool Triggered { get; }
	}

	public readonly struct Conditional : IConditional
	{
		public Func<bool> Predicate { get; }
		public Action[] Actions { get; }
		public bool Triggered => Predicate();
		public Conditional(Func<bool> predicate, params Action[] actions)
		{
			this.Predicate = predicate;
			this.Actions = actions;
		}
	}

	public readonly struct Transitional : IConditional
	{ 
		public Func<bool> Predicate { get; }
		public Action[] Actions { get; }
		public bool Triggered => Predicate();
		public Transition Transition { get; }
		public IState TargetState { get; }
		public StateArgs StateArgs { get; }
		public void TransitionAct() => Transition(TargetState,StateArgs);

		public Transitional(in IState targetState, StateArgs args, Func<bool> predicate, Transition transition) : this()
		{
			this.Predicate = predicate;
			this.Transition = transition;
			this.TargetState = targetState;
			this.StateArgs = args;
			this.Actions = new Action[] {TransitionAct};
		}
	}
}
