using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Sample08.Scripts
{
	public class VFXText : MonoBehaviour
	{
		private struct Shape
		{
			public List<Vector2> startPoints;
			public List<Vector2> endPoints;
			public float width;

			public void Init()
			{
				startPoints = new List<Vector2>();
				endPoints = new List<Vector2>();
				width = 0;
			}
		}

		public CHRFont font;
		public string[] worldList;
		public float initialDelay;
		public float delay = 1f;
		public bool automatic = true;
		public bool morphable = true;

		private int currentWorldIndex;
		private VisualEffect vfx;
		private Texture2D texture;

		private string lastWord;
		
		private void Start()
		{
			vfx = gameObject.GetComponent<VisualEffect>();
		}

		private void Update()
		{
			if (automatic && !IsInvoking(nameof(DrawNextWord)))
			{
				InvokeRepeating(nameof(DrawNextWord), initialDelay, delay);
			}
			else if (!automatic && IsInvoking(nameof(DrawNextWord)))
			{
				CancelInvoke();
			}
		}

		private Shape BuildLines(int ch)
		{
			var shape = new Shape();
			shape.Init();
			if (ch < font.Outlines.Count)
			{
				var letter = font.Outlines[ch];
				shape.width = font.Widths[ch];

				foreach (var stroke in letter)
				{
					for (var j = 0; j < stroke.Count - 1; j++)
					{
						shape.startPoints.Add(stroke[j]);
						shape.endPoints.Add(stroke[j + 1]);
					}
				}
			}

			return shape;
		}

		private Shape ShiftRight(Shape shape, float amount)
		{
			for (var i = 0; i < shape.startPoints.Count; i++)
			{
				var p = shape.startPoints[i];
				p.x += amount;
				shape.startPoints[i] = p;
			}

			for (var i = 0; i < shape.endPoints.Count; i++)
			{
				var p = shape.endPoints[i];
				p.x += amount;
				shape.endPoints[i] = p;
			}

			return shape;
		}

		private Shape ConcatShape(Shape shape1, Shape shape2)
		{
			var shiftedShape2 = ShiftRight(shape2, shape1.width);
			return new Shape()
			{
				startPoints = shape1.startPoints.Concat(shiftedShape2.startPoints).ToList(),
				endPoints = shape1.endPoints.Concat(shiftedShape2.endPoints).ToList(),
				width = shape1.width + shape2.width
			};
		}

		private Shape AlignShapeCenter(Shape shape)
		{
			var maxX = Mathf.Max(shape.startPoints.Select(p => p.x).Max(),
				shape.endPoints.Select(p => p.x).Max());
			var amount = maxX / 2;
			for (var i = 0; i < shape.startPoints.Count; i++)
			{
				var p = shape.startPoints[i];
				p.x -= amount;
				shape.startPoints[i] = p;
			}

			for (var i = 0; i < shape.endPoints.Count; i++)
			{
				var p = shape.endPoints[i];
				p.x -= amount;
				shape.endPoints[i] = p;
			}

			return shape;
		}

		public void DrawNextWord()
		{
			if (currentWorldIndex >= worldList.Length)
			{
				currentWorldIndex = 0;
			}

			DrawWord(worldList[currentWorldIndex++]);
		}

		private Shape BuildWord(string word)
		{
			var shape = new Shape();
			shape.Init();

			foreach (var ch in word)
			{
				var newShape = BuildLines(ch);
				shape = ConcatShape(shape, newShape);
			}

			return shape;
		}

		private Color[] BuildColorArray(Shape shape)
		{
			//create a 2px high texture from start and end pairs
			//row 0 is start points, row 1 isi end points
			var count = shape.startPoints.Count;
			var pixelData = new Color[count * 2];
			for (var i = 0; i < count; i++)
			{
				pixelData[i] = new Color(shape.startPoints[i].x, shape.startPoints[i].y, 0, 1);
				pixelData[count + i] = new Color(shape.endPoints[i].x, shape.endPoints[i].y, 0, 1);
			}

			return pixelData;
		}

		private void DrawWord(string word)
		{
			if (word == lastWord)
			{
				return;
			}
			
			var shape = BuildWord(word);
			shape = AlignShapeCenter(shape);
			var colorArray = BuildColorArray(shape);

			if (texture == null)
			{
				texture = new Texture2D(colorArray.Length / 2, 2, TextureFormat.RGFloat, false)
				{
					wrapMode = TextureWrapMode.Mirror,
					filterMode = morphable ? FilterMode.Trilinear : FilterMode.Point,
				};
			}
			else
			{
				texture.Reinitialize(colorArray.Length / 2, 2);
				texture.filterMode = morphable ? FilterMode.Trilinear : FilterMode.Point;
			}

			texture.SetPixels(colorArray);
			texture.Apply();
			vfx.SetTexture("Positions", texture);
			vfx.SetUInt("Count", (uint) texture.width);
			lastWord = word;
		}
	}
}