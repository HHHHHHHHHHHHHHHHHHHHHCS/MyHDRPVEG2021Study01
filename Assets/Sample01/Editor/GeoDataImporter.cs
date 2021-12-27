using System;
using System.IO;
using Sample01.Scripts;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sample01.Editor
{
	//当后缀为 geodata 导入的时候触发
	[ScriptedImporter(1, "geodata")]
	public class GeoDataImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext context)
		{
			var data = ImportGeoData(context.assetPath);
			if (data == null)
			{
				return;
			}

			context.AddObjectToAsset("data", data);
			context.SetMainObject(data);
		}

		private GeoData ImportGeoData(string path)
		{
			try
			{
				using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
				using var reader = new BinaryReader(stream);

				// original map dimensions 
				var dims = (x: reader.ReadUInt16(), y: reader.ReadUInt16());
				var scale = (x: 1.0f / dims.x, y: 1.0f / dims.y);

				// element count
				var count = reader.ReadUInt32();

				// element readout
				var array = new Vector3[count];
				for (int i = 0; i < count; i++)
				{
					var px = reader.ReadUInt16() * scale.x;
					var py = reader.ReadUInt16() * scale.y;
					var data = reader.ReadSingle();
					array[i] = new Vector3(px, py, data);
				}
				
				Debug.Log($"ImportGeoData : {count}");
				return GeoData.CreateAsset(array);
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed importing {path}. {e.Message}");
				return null;
			}
		}
	}
}