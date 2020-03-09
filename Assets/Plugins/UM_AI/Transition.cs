using System;

namespace UM_AI
{
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

	public interface IConditional
	{
		Func<bool> Predicate { get; }
		Action Act { get; }
	}

	public struct Conditional : IConditional
	{
		public Func<bool> Predicate { get; }
		public Action Act { get; }
		public Conditional(Func<bool> predicate, Action action)
		{
			this.Predicate = predicate;
			this.Act = action;
		}
	}

	public struct Transitional : IConditional
	{
		public Func<bool> Predicate { get; }
		public Action Act { get; }
		private Transition transition;
		private TransitionArgs args;
		private string stateName;
		private void ToTransition() => transition(args??(TransitionArgs.Empty as TransitionArgs));

		public Transitional(IState toState, Func<bool> predicate) : this()
		{
			this.transition = (x) => toState.StateMachine.ChangeState(nameof(toState), x.StateArgs);
			this.Act = ToTransition;
			this.Predicate = predicate;
		}

		public Transitional(Transition transition, Func<bool> predicate) : this()
		{
			this.transition = transition;
			this.Act = ToTransition;
			this.Predicate = predicate;
		}
	}
}
