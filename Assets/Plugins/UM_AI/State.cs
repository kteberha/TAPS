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

		//StateMachine StateMachine { get; set; }

		//Transition[] Transitions { get; }

        // /// <summary>
        // /// Change to the state with the specified name.
        // /// </summary>
        // void ChangeState(string stateName);

        // /// <summary>
        // /// Push another state above the current one, so that popping it will return to the
        // /// current state.
        // /// </summary>
        // void PushState(string stateName);

        // /// <summary>
        // /// Exit out of the current state and enter whatever state is below it in the stack.
        // /// </summary>
        // void PopState();

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
    }

	public abstract class State : IState
	{

        protected StateMachine StateMachine { get; set; }
        
		/// <summary>
        /// Dictionary of all event actions associated with this state.
        /// </summary>
        private readonly IDictionary<string, Action<EventArgs>> events = new Dictionary<string, Action<EventArgs>>();

        /// <summary>
        /// List of all conditions associated with this state.
        /// </summary>
        /// <typeparam name="Condition"></typeparam>
        /// <returns></returns>
		private readonly IList<ICondition> conditions = new List<Condition>() as IList<ICondition>;
        private IEnumerable<TransitionCondition> Transitions
        {
            get
            {
                return conditions.OfType<TransitionCondition>();
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
        public void UpdateConditions()
        {
            for (int i=0; i<conditions.Count; i++)
            {
                if (conditions[i].Predicate()) conditions[i].Act();
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
