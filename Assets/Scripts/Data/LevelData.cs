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

		public Vector2Int GridSize;

		public int FirstStarScore;
		public int SecondStarScore;
		public int ThirdStarScore;

		public GameObject Bille;
		public GameObject Plomb;
		public GameObject Quinte;

		public Sprite BackgroundTexture;

		public MotifData Motif;

		public int Handicap;

		public int Difficulte;

	}
}
