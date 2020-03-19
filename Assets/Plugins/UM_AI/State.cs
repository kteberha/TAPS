using System;
using System.Collections.Generic;
using System.Linq;

namespace UM_AI
{
	/// <summary>
	/// Non-generic state interface.
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// Update this state and its children with a specified delta time.
		/// </summary>
		void Update(float deltaTime);

		/// <summary>
		/// Triggered when we enter the state.
		/// </summary>
		void Enter(StateArgs args);

		/// <summary>
		/// Triggered when we exit the state.
		/// </summary>
		void Exit(StateArgs args);

		/// <summary>
		/// Check conditions.
		/// </summary>
		void UpdateConditionals();

		/// <summary>
		/// Set conditional.
		/// </summary>
		/// <param name="cond"></param>
		void SetConditional(IConditional cond);

		/// <summary>
		/// Set conditional.
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="act"></param>
		void SetConditional(Func<bool> predicate, params Action[] acts);

		/// <summary>
		/// Set Transitional.
		/// </summary>
		/// <param name="transition"></param>
		void SetTransition(Transitional transition);

		/// <summary>
		/// Set Transitional.
		/// </summary>
		/// <param name="targetState"></param>
		/// <param name="predicate"></param>
		/// <param name="acts"></param>
		void SetTransition(in IState targetState, Func<bool> predicate, Transition transition);

		/// <summary>
		/// Set Transitional.
		/// </summary>
		/// <param name="targetState"></param>
		/// <param name="args"></param>
		/// <param name="predicate"></param>
		/// <param name="acts"></param>
		void SetTransition(in IState targetState, StateArgs args, Func<bool> predicate, Transition transition);
	}

	public abstract class State : IState
	{
		/// <summary>
		/// List of all conditions associated with this state.
		/// </summary>
		/// <typeparam name="Conditional"></typeparam>
		protected readonly IList<IConditional> conditionals = new List<IConditional>();
		private IEnumerable<Transitional> Transitions
		{
			get
			{
				return conditionals.OfType<Transitional>();
			}
		}

		/// <summary>
		/// Triggered when we enter the state.
		/// </summary>
		public virtual void Enter(StateArgs args) {}
		
		/// <summary>
		/// Update this state and its children with a specified delta time.
		/// </summary>
		public virtual void Update(float deltaTime) {}

		/// <summary>
		/// Triggered when we exit the state.
		/// </summary>
		public virtual void Exit(StateArgs args) {}


		/// <summary>
		/// Check conditions.
		/// </summary>
		public void UpdateConditionals()
		{
			for (int i=0; i<conditionals.Count; i++)
			{
				if (conditionals[i].Triggered)
				{
					foreach (Action act in conditionals[i].Actions) act();
				}
			}
		}

		/// <summary>
		/// Set conditional
		/// </summary>
		/// <param name="cond"></param>
		public void SetConditional(IConditional cond)
		{
			conditionals.Add(cond);
		}

		/// <summary>
		/// Set conditional.
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="act"></param>
		public void SetConditional(Func<bool> predicate, params Action[] acts)
		{
			conditionals.Add(new Conditional(predicate, acts));
		}

		/// <summary>
		/// Set Transitional.
		/// </summary>
		/// <param name="transition"></param>
		public void SetTransition(Transitional transition)
		{
			conditionals.Add(transition);
		}

		/// <summary>
		/// Set Transition.
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="transition"></param>
		public void SetTransition(in IState targetState, Func<bool> predicate, Transition transition)
		{
			conditionals.Add(new Transitional(in targetState, StateArgs.Empty, predicate, transition));

		}

		/// <summary>
		/// Set Transitional.
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="transition"></param>
		public void SetTransition(in IState targetState, StateArgs args, Func<bool> predicate, Transition transition)
		{
			conditionals.Add(new Transitional(in targetState, args, predicate, transition));

		}

	}
}
