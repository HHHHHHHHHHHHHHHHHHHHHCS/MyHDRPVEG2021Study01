using System.IO;
using Sample08.Scripts;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sample08.Editor
{
	[ScriptedImporter(0, "chr")]
	public class CHRImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext ctx)
		{
			var font = ScriptableObject.CreateInstance<CHRFont>();
			font.dataRaw = File.ReadAllText(ctx.assetPath);
			ctx.AddObjectToAsset("font", font);
			ctx.SetMainObject(font);
		}
	}
}