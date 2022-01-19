using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sample08.Scripts
{
	public class CHRFont : ScriptableObject
	{
		public string dataRaw;

		private Dictionary<int, List<List<Vector2>>> outlines;

		private Dictionary<int, float> widths;

		public Dictionary<int, List<List<Vector2>>> Outlines => outlines;

		public Dictionary<int, float> Widths => widths;

		private void OnEnable()
		{
			if (dataRaw == null)
			{
				return;
			}
			
			var fontDataLines = dataRaw.Split('\n');
			outlines = new();
			widths = new();

			foreach (var line in fontDataLines)
			{
				if (line.Trim().Length < 1)
				{
					continue;
				}

				var item = line.Split(new[] {';'}, 2);
				var lineHeader = item[0];
				var lineHeaderData = lineHeader.Split(' ');
				var asciiCode = int.Parse(lineHeaderData[0].Split('_')[1], System.Globalization.NumberStyles.HexNumber);
				var width = float.Parse(lineHeaderData[1]) / 20.0f;
				var lineData = item[1];
				if (lineData.Trim().Length < 1)
				{
					continue;
				}

				var strokeStrings = lineData.Trim().Split(';');
				var strokes = new List<List<Vector2>>();

				foreach (var stroke in strokeStrings)
				{
					var pointsString = stroke.Trim().Split(' ');
					if (pointsString.Length < 1)
					{
						continue;
					}

					var points = new List<Vector2>();

					foreach (var point in pointsString)
					{
						var coordsString = point.Trim().Split(',');
						if (coordsString.Length < 1)
						{
							continue;
						}

						float xCoord = float.Parse(coordsString[0]);
						float yCoord = float.Parse(coordsString[1]);

						var coord = new Vector2(xCoord / 20.0f, yCoord / 20.0f);
						points.Add(coord);
					}

					strokes.Add(points);
				}
				
				outlines[asciiCode] = strokes;
				widths[asciiCode] = width;
			}
			
			
		}
	}
}