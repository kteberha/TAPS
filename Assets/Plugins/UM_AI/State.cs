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
        // /// <summary>
        // /// Parent state, or null if this is the root level state.
        // /// </summary>
        // IState Parent { get; set; }

		StateMachine StateMachine { get; set; }

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
        /// Trigger an event on this state or one of its children.
        /// </summary>
        /// <param name="name">Name of the event to trigger</param>
        void TriggerEvent(string name);

        /// <summary>
        /// Triggered when and event occurs. Executes the event's action if the 
        /// current state is at the top of the stack, otherwise triggers it on 
        /// the next state down.
        /// </summary>
        /// <param name="name">Name of the event to trigger</param>
        /// <param name="eventArgs">Arguments to send to the event</param>
        void TriggerEvent(string name, EventArgs eventArgs);

        /// <summary>
        /// Check conditions 
        /// </summary>
        void UpdateConditionals();

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="cond"></param>
        void SetConditional(IConditional cond);

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="act"></param>
        void SetConditional(Func<bool> predicate, Action act);

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transition"></param>
        void SetTransitional(Func<bool> predicate, Transition transition);

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        void SetTransitional(Func<bool> predicate, IState toState);

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        void SetTransitional(Func<bool> predicate, string toState);

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        void SetTransitional(Func<bool> predicate, Type toState);
    }

	public abstract class State : IState
	{

        public StateMachine StateMachine { get; set; }
        
		/// <summary>
        /// Dictionary of all event actions associated with this state.
        /// </summary>
        private readonly IDictionary<string, Action<EventArgs>> events = new Dictionary<string, Action<EventArgs>>();

        /// <summary>
        /// List of all conditions associated with this state.
        /// </summary>
        /// <typeparam name="Conditional"></typeparam>
		private readonly IList<IConditional> conditionals = new List<Conditional>() as IList<IConditional>;
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
        /// Check conditions 
        /// </summary>
        public void UpdateConditionals()
        {
            for (int i=0; i<conditionals.Count; i++)
            {
                if (conditionals[i].Predicate()) conditionals[i].Act();
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
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="act"></param>
        public void SetConditional(Func<bool> predicate, Action act)
        {
            conditionals.Add(new Conditional(predicate, act));
        }

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transition"></param>
        public void SetTransitional(Func<bool> predicate, Transition transition)
        {
            conditionals.Add(new Transitional(transition, predicate));
        }

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public void SetTransitional(Func<bool> predicate, IState toState)
        {
            SetTransitional(predicate, nameof(toState));
        }

        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public void SetTransitional(Func<bool> predicate, string toState)
        {
            try
            {
                conditionals.Add(new Transitional(this.StateMachine.GetState(toState), predicate));
            }
            catch
            {
                throw new ApplicationException("Could not find state " + toState);
            }
        }


        /// <summary>
        /// Set conditional
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public void SetTransitional(Func<bool> predicate, Type toState)
        {
            try
            {
                conditionals.Add(new Transitional(this.StateMachine.GetState(toState), predicate));
            }
            catch
            {
                throw new ApplicationException("Could not find state " + toState);
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
        /// Trigger an event.
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
	}
}
