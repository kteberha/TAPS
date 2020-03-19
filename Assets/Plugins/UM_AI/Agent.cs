using System;
using UnityEngine;
using UModules;

namespace UM_AI
{
	public abstract class Agent : ExtendedMonoBehaviour
	{
		/// <summary>
		/// StateMachine of this Agent.
		/// </summary>
		/// <value></value>
		public StateMachine SM {get; private set;}

		/// <summary>
		/// Interval in seconds to update Agent.
		/// </summary>
		public float UpdateInterval;

		#if UNITY_EDITOR
		[HideInInspector]
		public Color debugColor;
		#endif

		public override void Initialize()
		{
			this.SM = new StateMachine();
			StartUpdate();
		}

		/// <summary>
		/// Start updating the Agent.
		/// </summary>
		public void StartUpdate()
		{
			InvokeRepeating(nameof(UpdateAgent), UpdateInterval, UpdateInterval);
		}

		/// <summary>
		/// Stop updating the Agent.
		/// </summary>
		public void StopUpdate()
		{
			CancelInvoke();
		}

		/// <summary>
		/// Temporarily stop updating, and then start updating the Agent after a given delay.
		/// </summary>
		/// <param name="delay">Delay in seconds.</param>
		public void DelayUpdate(float delay)
		{
			CancelInvoke();
			InvokeRepeating(nameof(UpdateAgent), delay, UpdateInterval);
		}

		/// <summary>
		/// Update Agent.
		/// </summary>
		protected virtual void UpdateAgent()
		{
			SM.UpdateState(UpdateInterval);
		}
	}

	public abstract class State<AT> : State where AT:Agent
    {
        private readonly Agent _agent;
        protected AT Agent { get {return _agent as AT;} }

		#if UNITY_EDITOR
		public Color color;
		public State(Agent agent, Color color) : base()
		{
			this._agent = agent;
			this.color = color;
		}
		#endif

        public State(Agent agent) : base()
        {
            this._agent = agent;
        }
    }
}
