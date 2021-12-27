using System;
using UnityEngine;

namespace Sample01.Scripts
{
	//让ScriptableObject 二进制化
	[PreferBinarySerialization]
	public class GeoData : ScriptableObject
	{
		[SerializeField] private Vector3[] pointArray;

		private GraphicsBuffer buffer;

		public GraphicsBuffer Buffer => buffer ??= CreateBuffer();

		public static GeoData CreateAsset(Vector3[] source)
		{
			var asset = CreateInstance<GeoData>();
			asset.pointArray = source;
			return asset;
		}

		private GraphicsBuffer CreateBuffer()
		{
			var bf = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
				pointArray.Length, sizeof(float) * 3);
			bf.SetData(pointArray);
			return buffer;
		}

		private void OnDisable()
		{
			buffer?.Dispose();
			buffer = null;
		}
	}
}