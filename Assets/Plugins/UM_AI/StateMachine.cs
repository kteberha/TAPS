/*
	Peter Francis
	available to use according to UM_AI/LICENSE
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace UM_AI
{
	public class StateMachine
	{
		/// <summary>
		/// Default State of the StateMachine.
		/// </summary>
		/// <value></value>
		public IState DefaultState {get; set;}

		/// <summary>
		/// States contained in the StateMachine.
		/// </summary>
		private readonly IDictionary<string,IState> states = new Dictionary<string,IState>();

		/// <summary>
		/// Various actions associated with this state.
		/// </summary>
		private readonly IDictionary<string,Action<EventArgs>> events = new Dictionary<string,Action<EventArgs>>();

		/// <summary>
		/// Types of States in the StateMachine's State set.
		/// </summary>
		public IEnumerable<Type> StateTypes { get {return states.Select(x=>x.Value.GetType());} }

		/// <summary>
		/// Current active State.
		/// </summary>
		public IState CurrentState { get; private set; }

		/// <summary>
		/// Name of current active State.
		/// </summary>
		public string CurrentStateName => states.FirstOrDefault(x=>x.Value==CurrentState).Key;
		
		/// <summary>
		/// Time elapsed in seconds since the enter of current State.
		/// </summary>
		public TimeSpan TimeElapsed {get {return DateTime.Now - timeEntered;} }
		// FIXME: use more efficient method...
		private DateTime timeEntered;

		/// <summary>
		/// Update the current State in the StateMachine and check for transitions.
		/// </summary>
		public void UpdateState(float deltaTime)
		{
			CurrentState?.Update(deltaTime);
			CurrentState?.UpdateConditionals();
		}

		public void ChangeState(in IState state, StateArgs args = null)
		{
			if (CurrentState != state)
			{
				CurrentState?.Exit(args??StateArgs.Empty);
				CurrentState = state;
				CurrentState?.Enter(args??StateArgs.Empty);
				timeEntered = DateTime.Now;
			}
		}

		public void ChangeState(Type stateType, StateArgs args = null)
		{
			IState state;
			try
			{
				state = states.Values.Single(x=>x.GetType()==stateType);
				ChangeState(state,args);
			}
			catch
			{
				throw new ApplicationException(string.Format("Can't find state of type {0}", stateType));
			}
		}

		public void ChangeState(string stateName, StateArgs args = null)
		{
			IState state;
			if (!states.TryGetValue(stateName, out state))
			{
				throw new ApplicationException(String.Format("Can't find State of name {0}.", stateName));
			}
			ChangeState(state,args);
		}

		/// <summary>
		/// Add a given State to the StateMachine's set of states.
		/// </summary>
		/// <param name="state">State to add.</param>
		/// <returns>If state addition was successful.</returns>
		public void AddState(IState state)
		{
			states.Add(state.GetType().Name,state);
		}

		/// <summary>
		/// Remove a given State from the StateMachine's set of states.
		/// </summary>
		/// <param name="stateName">name of State to remove.</param>
		/// <returns>If state removal was successful.</returns>
		public bool RemoveState(string stateName)
		{
			return states.Remove(stateName);
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
				return states.Values.Single(x=>x.GetType()==stateType);
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
			if (states.TryGetValue(stateName, out stateOut))
			{
				return stateOut;
			}
			else
			{
				throw new ApplicationException(string.Format("Can't find state of name {0}", stateName));
			}
		}

		/// <summary>
		/// Sets an action to be associated with an identifier that can later be used
		/// to trigger it.
		/// Convenience method that uses default event args intended for events that 
		/// don't need any arguments.
		/// </summary>
		public void SetEvent(string identifier, Action<EventArgs> eventTriggeredAction)
		{
			SetEvent<EventArgs>(identifier, eventTriggeredAction);
		}

		/// <summary>
		/// Sets an action to be associated with an identifier that can later be used
		/// to trigger it.
		/// </summary>
		public void SetEvent<TEvent>(string identifier, Action<TEvent> eventTriggeredAction)
			where TEvent : EventArgs
		{
			events.Add(identifier, args => eventTriggeredAction(CheckEventArgs<TEvent>(identifier, args)));
		}

		/// <summary>
		/// Cast the specified EventArgs to a specified type, throwing a descriptive exception if this fails.
		/// </summary>
		private static TEvent CheckEventArgs<TEvent>(string identifier, EventArgs args) 
			where TEvent : EventArgs
		{
			try
			{
				return (TEvent)args;
			}
			catch (InvalidCastException ex)
			{
				throw new ApplicationException("Could not invoke event \"" + identifier + "\" with argument of type " +
					args.GetType().Name + ". Expected " + typeof(TEvent).Name, ex);
			}
		}

		/// <summary>
		/// Triggered when and event occurs. Executes the event's action if the 
		/// current state is at the top of the stack, otherwise triggers it on 
		/// the next state down.
		/// </summary>
		/// <param name="name">Name of the event to trigger</param>
		public void TriggerEvent(string name)
		{
			TriggerEvent(name, EventArgs.Empty);
		}

		/// <summary>
		/// Triggered when and event occurs. Executes the event's action.
		/// </summary>
		/// <param name="name">Name of the event to trigger</param>
		/// <param name="eventArgs">Arguments to send to the event</param>
		public void TriggerEvent(string name, EventArgs eventArgs)
		{
			Action<EventArgs> myEvent;
			if (events.TryGetValue(name, out myEvent))
			{
				myEvent(eventArgs);
			}
		}

		/// <summary>
		/// Set an implicit transition that can be triggered from any State to a given State in this StateMachine.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="toState"></param>
		/// <param name="args"></param>
		public void SetTransitionEvent(string name, string toState, StateArgs args)
		{
			IState targetState;
			try
			{
				targetState = GetState(toState);
				// FIXME: discarding x?
				events.Add(name,(x)=>ChangeState(in targetState,args));
			}
			catch
			{
				throw new ApplicationException(string.Format("Can't assign transition event to NULL state!"));
			}
		}

		// /// <summary>
		// /// Set an explicit transition that can be triggered from and to given States in this StateMachine.
		// /// </summary>
		// /// <param name="name"></param>
		// /// <param name="fromState"></param>
		// /// <param name="toState"></param>
		// /// <param name="args"></param>
		// public void SetTransitionEvent(string name, string fromState, string toState, StateArgs args)
		// {
		// 	IState targetState, sourceState;
		// 	try
		// 	{
		// 		targetState = GetState(toState);
		// 		sourceState = GetState(fromState);
		// 		// FIXME: discarding x?
		// 		events.Add(name,(x)=>{if (CurrentState==sourceState) ChangeState(in targetState,args);});
		// 	}
		// 	catch
		// 	{
		// 		throw new ApplicationException(string.Format("Can't assign transition event of NULL state!"));
		// 	}
		// }

		/// <summary>
		/// Set transition for a State contained in this StateMachine.
		/// </summary>
		/// <param name="fromState"></param>
		/// <param name="toState"></param>
		/// <param name="args"></param>
		/// <param name="predicate"></param>
		public void SetTransition(string fromState, string toState, StateArgs args, Func<bool> predicate)
		{
			IState targetState;
			try
			{
				targetState = GetState(toState);
				GetState(fromState).SetTransition(new Transitional(in targetState,args,predicate,ChangeState));
			}
			catch
			{
				throw new ApplicationException(string.Format("Can't assign transitions to NULL state!"));
			}  
		}
	}
}
