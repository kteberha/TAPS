using System;
using System.Collections.Generic;
using System.Linq;

namespace UM_AI
{
	public class StateMachine
	{
		// /// <summary>
		// /// Agent owning this StateMachine.
		// /// </summary>
		//public Agent Agent {get; private set;}

		/// <summary>
		/// Default State of the StateMachine.
		/// </summary>
		/// <value></value>
		public IState DefaultState {get; set;}

		/// <summary>
		/// Set of States contained in the StateMachine.
		/// </summary>
		public readonly IDictionary<string, IState> States = new Dictionary<string, IState>();

		/// <summary>
		/// Types of States in the StateMachine's State set.
		/// </summary>
		public IEnumerable<Type> StateTypes { get {return States.Select(x=>x.Value.GetType());} }
		//public IEnumerable<Type> StateTypes { get {return States.Select(x=>x.Value.GetType());} }

		/// <summary>
		/// Current active State.
		/// </summary>
		public IState CurrentState { get; private set; }
		//public State CurrentState {get {return StateStack?.Peek();} }
		
		/// <summary>
		/// Time elapsed in seconds since the enter of current State.
		/// </summary>
		public TimeSpan TimeElapsed {get {return DateTime.Now - timeEntered;} }
		private DateTime timeEntered;

		/// <summary>
		/// Update the current State in the StateMachine and check for transitions.
		/// </summary>
		public void UpdateState(float deltaTime)
		{
			CurrentState?.Update(deltaTime);
		}

		public void ChangeState(Type stateType, StateArgs args = null)
		{
			try
			{
				CurrentState = States.Values.Single(x=>x.GetType()==stateType);
				CurrentState?.Enter(args??StateArgs.Empty);
			}
			catch
			{
				throw new ApplicationException(string.Format("Can't find state of type {0}", stateType));
			}
		}

		public void ChangeState(string stateName, StateArgs args = null)
		{
            IState newState;
            if (!States.TryGetValue(stateName, out newState))
            {
				throw new ApplicationException(String.Format("Can't find State of name {0}.", stateName));
            }
			CurrentState = newState;
			CurrentState?.Enter(args??StateArgs.Empty);
		}

		/// <summary>
		/// Add a given State to the StateMachine's set of states.
		/// </summary>
		/// <param name="state">State to add.</param>
		/// <returns>If state addition was successful.</returns>
		public void AddState(IState state)
		{
			States.Add(state.GetType().Name,state);
		}

		/// <summary>
		/// Remove a given State from the StateMachine's set of states.
		/// </summary>
		/// <param name="stateName">name of State to remove.</param>
		/// <returns>If state removal was successful.</returns>
		public bool RemoveState(string stateName)
		{
			return States.Remove(stateName);
		}

		/// <summary>
		/// Retrieve a State of this StateMachine.
		/// </summary>
		/// <param name="stateType">type of requested State.</param>
		/// <returns>The requested State, if found.</returns>
		public IState GetState(Type stateType)
		{
			try
			{
				return States.Values.Single(x=>x.GetType()==stateType);
			}
			catch
			{
				throw new ApplicationException(string.Format("Can't find state of type {0}", stateType));
			}
		}

		/// <summary>
		/// Retrieve a State of this StateMachine.
		/// </summary>
		/// <param name="stateName"></param>
		/// <returns>The State, if found.</returns>
		public IState GetState(string stateName)
		{
			IState stateOut;
			if (States.TryGetValue(stateName, out stateOut))
			{
				return stateOut;
			}
			else
			{
				throw new ApplicationException(string.Format("Can't find state of name {0}", stateName));
			}
		}
	}

	// public class StateMachine<AT> : StateMachine where AT:Agent
	// {
    //     private readonly Agent _agent;
    //     protected AT Agent { get {return _agent as AT;} }

    //     public StateMachine(Agent agent) : base()
    //     {
    //         this._agent = agent;
    //     }
	// }
}
