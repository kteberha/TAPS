using System;

namespace UM_AI
{
	// public interface ITransition
	// {	
	// 	IState State { get; set; }
	// 	void ToTransition();
	// }

	// public struct Transition : ITransition
	// {
	// 	public IState State { get; set; }
	// 	public void ToTransition() => State.StateMachine.ChangeState(nameof(State));
	// 	public Transition(IState state)
	// 	{
	// 		this.State = state;
	// 	}
	// }

	public delegate void Transition(TransitionArgs toState);

	public class TransitionArgs : EventArgs
	{
		public readonly string ToState;
		public readonly StateArgs StateArgs;

		public TransitionArgs(IState state, StateArgs args)
		{
			this.ToState = nameof(state);
			this.StateArgs = args;
		}

		public TransitionArgs(string stateName, StateArgs args)
		{
			this.ToState = stateName;
			this.StateArgs = args;
		}

		public TransitionArgs(Type stateType, StateArgs args)
		{
			this.ToState = nameof(stateType);
			this.StateArgs = args;
		}
	}

	public interface ICondition
	{
		Func<bool> Predicate { get; }
		Action Act { get; }
	}

	public struct Condition : ICondition
	{
		public Func<bool> Predicate { get; }
		public Action Act { get; }
		public Condition(Func<bool> predicate, Action action)
		{
			this.Predicate = predicate;
			this.Act = action;
		}
	}

	//public struct TransitionCond : ITransition, ICondition
	public struct TransitionCondition : ICondition
	{
		public Func<bool> Predicate { get; }
		public Action Act { get; }
		private Transition transition;
		private TransitionArgs args;
		private IState state;
		private void ToTransition() => transition(args);

		public TransitionCondition(IState state, TransitionArgs args, Func<bool> predicate) : this()
		{
			this.args = args;
			this.state = state;
			this.Act = ToTransition;
			this.Predicate = predicate;
		}

		public TransitionCondition(Transition transition, Func<bool> predicate) : this()
		{
			this.transition = transition;
			this.Act = ToTransition;
			this.Predicate = predicate;
		}
	}
}
