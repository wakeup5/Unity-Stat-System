#if ODIN_INSPECTOR
#endif

namespace Waker.StatSystem
{
	public interface IStat<T> where T : System.IComparable
	{
		string Name { get; }
		T Property { get; }
		double BaseValue { get; set; }
		double Value { get; }

		void AddModifier(IStatModifier<T> modifier);
		bool RemoveModifier(IStatModifier<T> modifier);
	}
}
