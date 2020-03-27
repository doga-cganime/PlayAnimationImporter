using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class AtrVisitor
	{

		public override object VisitOnAtr(DoGAAtrParser.OnAtrContext context)
		{
			var nameAtr = context.name.Text.ToLower();
			this.atr = new DoGAAtr(nameAtr);
			var result = base.VisitOnAtr(context);
			this.atrs.Add(this.atr);
			this.atr = null;

			return result;
		}

		public override object VisitOnAtrParam(DoGAAtrParser.OnAtrParamContext context)
		{
			var typeParam = context.typeParam.Text;
			var color = (Rgb)this.VisitColor(context.col);
			this.atr.setColor(typeParam, color);

			return base.VisitOnAtrParam(context);
		}

		public override object VisitOnAtrSpc(DoGAAtrParser.OnAtrSpcContext context)
		{
			var spc = (Rgb)this.VisitColor(context.spc);
			var size = (Rgb)this.VisitColor(context.size);
			var h = float.Parse(context.h.Text);
			this.atr.setColor("spc", spc);
			this.atr.setColor("spcsize", size);
			this.atr.spcH = h;

			return base.VisitOnAtrSpc(context);
		}

		public override object VisitOnAtrMap(DoGAAtrParser.OnAtrMapContext context)
		{
			var typeMap = context.typeMap.Text;
			var pathTexture = context.file.Text;
			float min = 0.0f;
			float max = 1.0f;
			if (context.min != null) min = float.Parse(context.min.Text);
			if (context.max != null) max = float.Parse(context.max.Text);

			var infoTexture = new TextureInfo(typeMap, pathTexture, min, max);
			this.atr.setInfoTexture(typeMap, infoTexture);

			return base.VisitOnAtrMap(context);
		}

		public override object VisitOnAtrMapSize(DoGAAtrParser.OnAtrMapSizeContext context)
		{
			var lower = (Vec2)this.VisitVector2(context.lower);
			var upper = (Vec2)this.VisitVector2(context.upper);
			this.atr.setRangeMap("mapsize", new Range<Vec2>(lower, upper));
			return base.VisitOnAtrMapSize(context);
		}

		public override object VisitOnAtrMapWind(DoGAAtrParser.OnAtrMapWindContext context)
		{
			var lower = (Vec2)this.VisitVector2(context.lower);
			var upper = (Vec2)this.VisitVector2(context.upper);
			this.atr.setRangeMap("mapwind", new Range<Vec2>(lower, upper));
			return base.VisitOnAtrMapWind(context);
		}

		public override object VisitOnAtrMapView(DoGAAtrParser.OnAtrMapViewContext context)
		{
			var lower = (Vec2)this.VisitVector2(context.lower);
			var upper = (Vec2)this.VisitVector2(context.upper);
			this.atr.setRangeMap("mapview", new Range<Vec2>(lower, upper));
			return base.VisitOnAtrMapView(context);
		}

		public override object VisitOnAtrOpt(DoGAAtrParser.OnAtrOptContext context)
		{
			return base.VisitOnAtrOpt(context);
		}

		public override object VisitOnAtrOptEmission(DoGAAtrParser.OnAtrOptEmissionContext context)
		{
			this.atr.option.glowpower = float.Parse(context.glowpower.Text);
			this.atr.option.glowradius = float.Parse(context.glowradius.Text);
			return base.VisitOnAtrOptEmission(context);
		}

		public override object VisitOnAtrOptCastShadow(DoGAAtrParser.OnAtrOptCastShadowContext context)
		{
			var typeCastShadow = context.typeCastShadow.Text;
			this.atr.option.castShadow = (typeCastShadow == "castshadow") ? true : false;

			return base.VisitOnAtrOptCastShadow(context);
		}

		public override object VisitOnAtrOptReceiveShadow(DoGAAtrParser.OnAtrOptReceiveShadowContext context)
		{
			var typeReceiveShadow = context.typeReceiveShadow.Text;
			this.atr.option.receiveShadow = (typeReceiveShadow == "receiveshadow") ? true : false;
			return base.VisitOnAtrOptReceiveShadow(context);
		}

		public override object VisitOnAtrOptSelfShadow(DoGAAtrParser.OnAtrOptSelfShadowContext context)
		{
			var typeSelfShadow = context.typeSelfShadow.Text;
			this.atr.option.selfShadow = (typeSelfShadow == "selfshadow") ? true : false;
			return base.VisitOnAtrOptSelfShadow(context);
		}

		public override object VisitOnAtrOptRayTraceLevel(DoGAAtrParser.OnAtrOptRayTraceLevelContext context)
		{
			this.atr.option.raytracelevel = int.Parse(context.level.Text);
			return base.VisitOnAtrOptRayTraceLevel(context);
		}

		public override object VisitOnAtrOptDraw(DoGAAtrParser.OnAtrOptDrawContext context)
		{
			var colorDraw = (Rgb)this.VisitColor(context.col);
			var isRate = (context.isRate != null) ? true : false;
			var value = float.Parse(context.val.Text);
			if (isRate) value = -value;
			this.atr.option.drawcolor = colorDraw;
			this.atr.option.drawsize = value;
			return base.VisitOnAtrOptDraw(context);
		}

		public override object VisitOnAtrOptEmphasis(DoGAAtrParser.OnAtrOptEmphasisContext context)
		{
			this.atr.option.emphasis = float.Parse(context.val.Text);
			return base.VisitOnAtrOptEmphasis(context);
		}

		public override object VisitOnAtrOptEdge(DoGAAtrParser.OnAtrOptEdgeContext context)
		{
			var typeEdge = context.typeEdge.Text;
			switch (typeEdge) {
				case "edge":
					this.atr.option.edgetype = EdgeType.Normal;
					break;
				case "celllookedge":
				case "cellookedge":
					this.atr.option.edgetype = EdgeType.CellLook;
					break;
				case "edgecancel":
				case "edgecanceler":
					this.atr.option.edgetype = EdgeType.Canceller;
					break;
				default:
					this.atr.option.edgetype = EdgeType.None;
					break;
			}
			return base.VisitOnAtrOptEdge(context);
		}

		public override object VisitOnAtrOptCellLookSpecular(DoGAAtrParser.OnAtrOptCellLookSpecularContext context)
		{
			this.atr.option.minCellLookSpecular = float.Parse(context.min.Text);
			this.atr.option.maxCellLookSpecular = float.Parse(context.max.Text);
			return base.VisitOnAtrOptCellLookSpecular(context);
		}

		public override object VisitOnAtrOptShader(DoGAAtrParser.OnAtrOptShaderContext context)
		{
			var numGradations = int.Parse(context.gradationcount.Text);
			for (int index = 0; index < numGradations; index++) {
				var rangeGradation = (GradationRange)this.VisitGradationrange(context.gradationrange(index));
				if (rangeGradation != null) this.atr.option.addGradation(rangeGradation);
			}
			return base.VisitOnAtrOptShader(context);
		}

		public override object VisitOnAtrOptCastDecal(DoGAAtrParser.OnAtrOptCastDecalContext context)
		{
			var castDecal = float.Parse(context.val.Text);
			this.atr.option.castDecal = castDecal;
			return base.VisitOnAtrOptCastDecal(context);
		}

		public override object VisitOnAtrOptReceiveDecal(DoGAAtrParser.OnAtrOptReceiveDecalContext context)
		{
			var typeReceiveDecal = context.typeReceiveDecal.Text;
			this.atr.option.reciveDecal = (typeReceiveDecal == "receivedecal") ? true : false;
			return base.VisitOnAtrOptReceiveDecal(context);
		}

		public override object VisitGradationrange(DoGAAtrParser.GradationrangeContext context)
		{
			var min = float.Parse(context.min.Text);
			var max = float.Parse(context.max.Text);
			var color = (Rgb)this.VisitColor(context.col);
			string pathTexture = "";
			if (context.texture != null) pathTexture = context.texture.Text;
			var rangeGradation = new GradationRange(min, max, color, pathTexture);
			return rangeGradation;
		}

		public override object VisitColor(DoGAAtrParser.ColorContext context)
		{
			Rgb color = null;
			if (context.vec != null) {
				color = new Rgb((Vec3)this.VisitVector3(context.vec));
			} else {
				var value = float.Parse(context.val.Text);
				color = new Rgb(value, value, value);
			}
			return color;
		}


		public override object VisitVector3(DoGAAtrParser.Vector3Context context)
		{
			return new Vec3(float.Parse(context.x.Text),
				float.Parse(context.y.Text), float.Parse(context.z.Text));
		}

		public override object VisitVector2(DoGAAtrParser.Vector2Context context)
		{
			return new Vec2(float.Parse(context.x.Text), float.Parse(context.y.Text));
		}
	}
}
