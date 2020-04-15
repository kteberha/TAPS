/*
	Peter Francis
	available to use according to UM_AI/LICENSE
*/

using System;

namespace UM_AI
{
	public abstract class StateArgs : EventArgs
	{
		new public static readonly StateArgs Empty;
	}

	public class StateArgs<T> : StateArgs
	{
		public T Arg1 { get; set; }

		public StateArgs(T arg1)
		{
			this.Arg1 = arg1;
		}
	}
	
	public class StateArgs<T1,T2> : StateArgs
	{
		public T1 Arg1 { get; set; }
		public T2 Arg2 { get; set; }

		public StateArgs(T1 arg1, T2 arg2)
		{
			this.Arg1 = arg1;
			this.Arg2 = arg2;
		}
	}

	public class StateArgs<T1,T2,T3> : StateArgs
	{
		public T1 Arg1 { get; set; }
		public T2 Arg2 { get; set; }
		public T3 Arg3 { get; set; }

		public StateArgs(T1 arg1, T2 arg2, T3 arg3)
		{
			this.Arg1 = arg1;
			this.Arg2 = arg2;
			this.Arg3 = arg3;
		}
	}
}
