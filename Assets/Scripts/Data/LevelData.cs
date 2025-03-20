using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
	public Layer[] layers;

	[System.Serializable]
	public class Layer
	{
		public int Star1score;
		public int Star2Score;
		public int Star3Score;

		public GameObject Bille;
		public GameObject Plomb;
		public GameObject Quinte;

		public Material Background;

		public MotifData Motif;

		public SoundData Sounds;

	}
}
