using System;

namespace UM_AI
{
	public abstract class StateArgs : EventArgs
	{
		new public static readonly StateArgs Empty;
	}

	public class StateArgs<T0,T1,T2> : StateArgs
	{
		T0 Arg1 { get; set; }
		T1 Arg2 { get; set; }
		T2 Arg3 { get; set; }
	}

	public class StateArgs<T> : StateArgs
	{
		T Arg1 { get; set; }
	}
	
	public class StateArgs<T0,T1> : StateArgs
	{
		T0 Arg1 { get; set; }
		T1 Arg2 { get; set; }
	}
}
