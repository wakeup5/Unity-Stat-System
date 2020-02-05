#if ODIN_INSPECTOR
#endif

namespace Waker.StatSystem
{
	public interface IStatModifier<T> where T : System.IComparable
	{
		T Property { get; }
		int Order { get; }

		void Modify(ref double currentValue, double baseValue);
	}
}
