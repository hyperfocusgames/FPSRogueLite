using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RNGUtil {

	public static T Choose<T>(this System.Random rng, T[] list) {
		if (list.Length == 0) {
			return default(T);
		}
		else {
			return list[rng.Next(list.Length)];
		}
	}
	public static T Choose<T>(this System.Random rng, List<T> list) {
		if (list.Count == 0) {
			return default(T);
		}
		else {
			return list[rng.Next(list.Count)];
		}
	}

	public static T ChooseWeighted<T>(this System.Random rng, WeightedElement<T>[] list) {
		float totalWeight = 0;
		foreach (WeightedElement<T> element in list) {
			totalWeight += element.weight;
		}
		totalWeight *= (float) rng.NextDouble();
		foreach (WeightedElement<T> element in list) {
			totalWeight -= element.weight;
			if (totalWeight <= 0) {
				return element.value;
			}
		}
		return default(T);
	}

}

[System.Serializable]
public class WeightedElement<T> {
	public float weight;
	public T value;
}
