#if ODIN_INSPECTOR
#endif

using System.Collections.Generic;

namespace Waker.StatSystem
{
	public interface IStatModifier<T> where T : System.IComparable
	{
		T Property { get; }
		int Order { get; }

		void Modify(ref double currentValue, double baseValue);
	}

	public interface IStatEnhancer<T> where T : System.IComparable
	{
		IEnumerable<IStatModifier<T>> Modifiers { get; }
	}
}
