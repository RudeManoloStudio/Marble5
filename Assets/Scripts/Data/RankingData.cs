using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RankingData : ScriptableObject
{
	public Layer[] layers;

	[System.Serializable]
	public class Layer	{

		public int Stars;
		public string Rank;

	}
}
