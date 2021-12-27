using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Sample01.Scripts
{
	// [AddComponentMenu("VFX/Property Binders/GeoVfx/GeoData Binder")]
	[VFXBinder("GeoVfx/GeoData")]
	public class VFXGeoDataBinder : VFXBinderBase
	{
		public GeoData source = null;

		[VFXPropertyBinding("UnityEngine.GraphicsBuffer"), SerializeField]
		private ExposedProperty property = "DataSet";

		public string Property
		{
			get => (string) property;
			set => property = value;
		}

		public override bool IsValid(VisualEffect component)
			=> source != null && component.HasGraphicsBuffer(property);

		public override void UpdateBinding(VisualEffect component)
			=> component.SetGraphicsBuffer(property, source.Buffer);

		public override string ToString()
			=> $"GeoData : '{property}' -> {(source != null ? source.name : "(null)")}";
	}
}