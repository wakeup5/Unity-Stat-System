using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Waker.StatSystem
{
	[System.Serializable]
	public class StatBase<T> : IStat<T> where T : System.IComparable
	{
		/// <summary>
		/// 스탯을 구분하는 값
		/// </summary>
	#if ODIN_INSPECTOR
		[HideLabel, HorizontalGroup, Required, ReadOnly]
	#endif
		[SerializeField]
		private T property = default;

		/// <summary>
		/// 스탯의 기본 값
		/// </summary>
	#if ODIN_INSPECTOR
		[HideLabel, HorizontalGroup, OnValueChanged(nameof(OnBaseValueChangedOnEditor))]
	#endif
		[SerializeField]
		private double baseValue = 0d;

		/// <summary>
		/// 값이 변경됨을 알리는 값
		/// </summary>
		protected bool isDirty = true;

		/// <summary>
		/// 캐싱된 최종 값
		/// </summary>
		private double value;

		/// <summary>
		/// Modifier Container
		/// </summary>
		private readonly List<IStatModifier<T>> modifiers;
		private readonly List<IStatEnhancer<T>> enhancers;

		public StatBase()
		{
			modifiers = new List<IStatModifier<T>>();
			enhancers = new List<IStatEnhancer<T>>();
		}

		public StatBase(T property, double baseValue) : this()
		{
			this.property = property;
			this.baseValue = baseValue;
		}

		public string Name
		{
			get => property.ToString();
		}

		public T Property
		{
			get => property;
		}

		public double BaseValue
		{
			get => baseValue;
			set
			{
				baseValue = value;

				isDirty = true;
				valueChanged?.Invoke(this);
			}
		}

#if ODIN_INSPECTOR
		[HideLabel, ShowInInspector, HorizontalGroup, ReadOnly]
#endif
		public double Value
		{
			get
			{
				if (isDirty)
				{
					value = CalculateValue();
					isDirty = false;
				}

				return value;
			}
		}

		public IStatModifier<T> BaseModifier { get; set; } = null;

		public event System.Action<IStat<T>> valueChanged;

		public void AddModifier(IStatModifier<T> modifier)
		{
			if (property.CompareTo(modifier.Property) != 0)
			{
				return;
			}

			if (modifiers.Contains(modifier))
			{
				return;
			}

			modifiers.Add(modifier);
			modifiers.Sort(ModifierComparer.Instance);

			isDirty = true;
			valueChanged?.Invoke(this);
		}

		public bool RemoveModifier(IStatModifier<T> modifier)
		{
			if (modifiers.Remove(modifier))
			{
				isDirty = true;
				valueChanged?.Invoke(this);

				return true;
			}

			return false;
		}

		public void ClearModifiers()
		{
			modifiers.Clear();

			isDirty = true;
		}

		private double CalculateValue()
		{
			double modifiedValue = baseValue;

			BaseModifier?.Modify(ref modifiedValue, baseValue);

			foreach (var modifier in modifiers)
			{
				modifier.Modify(ref modifiedValue, baseValue);
			}

			return modifiedValue;//((v + addtive) * multiplyAddtive * multiply) + constance;
		}

#if ODIN_INSPECTOR
		private void OnBaseValueChangedOnEditor()
		{
			baseValue = System.Math.Floor(baseValue * 1000) / 1000;

			isDirty = true;
			valueChanged?.Invoke(this);
		}
#endif

		private class ModifierComparer : IComparer<IStatModifier<T>>
		{
			public static readonly ModifierComparer Instance = new ModifierComparer();
			public int Compare(IStatModifier<T> x, IStatModifier<T> y)
			{
				return x.Order.CompareTo(y.Order);
			}
		}

	}
}
