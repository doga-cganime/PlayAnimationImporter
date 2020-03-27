using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Tree;

namespace TotekanReaderLib
{

	public class DoGAE1PVisitor : DoGAE1PBaseVisitor<object>
	{
		public Dictionary<string, DoGAAtr> mapAtrsPalette = new Dictionary<string, DoGAAtr>();
		public List<DoGASufInfo> listSufObjs = new List<DoGASufInfo>();
		private Stack<List<TransformElement>> stackTransforms = new Stack<List<TransformElement>>();

		private DoGAAtr atr;
		private string path;

		public DoGAE1PVisitor(string path)
		{
			this.stackTransforms.Push(new List<TransformElement>());
			this.path = path;
		}

		public override object VisitBlock(DoGAE1PParser.BlockContext context)
		{
			var hoge = this.stackTransforms.Peek();
			
			this.stackTransforms.Push(this.stackTransforms.Peek().CloneDeep());
			var result = base.VisitBlock(context); // children
			this.stackTransforms.Pop();

			return result;
		}

		public override object VisitOnTransform(DoGAE1PParser.OnTransformContext context)
		{
			var typeTransform = context.typeTransform.Text.ToLower();
			var v = (Vec3)this.VisitVector3(context.vector3());

			var transform = this.stackTransforms.Pop();
//				matrixTrans = Matrix4x4.TRS(vec, Quaternion.identity, vecUnit);				
			transform.Add(new TransformElement(typeTransform, v));
			this.stackTransforms.Push(transform);

			return base.VisitOnTransform(context);
		}

		public override object VisitOnRotate(DoGAE1PParser.OnRotateContext context)
		{
			var typeRotation = context.typeRotation.Text.ToLower();
			var angle = float.Parse(context.angle.Text);

			var transform = this.stackTransforms.Pop();
			transform.Add(new TransformElement(typeRotation, new float[] { angle }));
//				matrixRot = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(angle, new Vector3(1.0f, 0.0f, 0.0f)), vecUnit);
			this.stackTransforms.Push(transform);
	
			return base.VisitOnRotate(context);
		}

/*		public string stripQuote(string str)
		{
			var regExpStr = new Regex("\"([^\"]+)\""); 
			var matched = regExpStr.Match(str);
			if (matched.Success) {
				str = matched.Groups[1].Captures[0].ToString();
			}
			return str;
		}
*/
		public override object VisitOnObj(DoGAE1PParser.OnObjContext context)
		{
			var nameObj = StringUtil.stripQuote(context.nameObj.Text.ToLower());

			var pathObj = StringUtil.stripQuote(context.pathObj.path.Text.ToLower());
			string nameAtr = null;
			string nameTexture = null;
			string nameMaterial = null;
			if (context.pathObj.NAME() != null) {
				var paramsAtr = context.pathObj.NAME();
				if (paramsAtr.Length == 3) { // e1p
					nameAtr = StringUtil.stripQuote(paramsAtr[0].ToString().ToLower());
					nameTexture = StringUtil.stripQuote(paramsAtr[1].ToString().ToLower());
					nameMaterial = StringUtil.stripQuote(paramsAtr[2].ToString().ToLower());
				} else if (context.pathObj.name != null) {
					nameAtr = StringUtil.stripQuote(context.pathObj.name.Text.ToLower());
				}
			}

			var transform = this.stackTransforms.Peek().CloneDeep();
			this.listSufObjs.Add(new DoGASufInfo(transform, nameObj, pathObj, nameAtr, nameTexture, nameMaterial));

			return base.VisitOnObj(context);
		}

		public override object VisitOnAtr(DoGAE1PParser.OnAtrContext context)
		{
			var nameAtr = StringUtil.stripQuote(context.name.Text.ToLower());
			this.atr = new DoGAAtr(nameAtr, this.path);
			var result = base.VisitOnAtr(context);
			this.mapAtrsPalette.Add(nameAtr, this.atr);
			this.atr = null;

			return result;
		}

		public override object VisitOnAtrParam(DoGAE1PParser.OnAtrParamContext context)
		{
			var typeParam = context.typeParam.Text;
			var color = (Rgb)this.VisitColor(context.col);
			this.atr.setColor(typeParam, color);

			return base.VisitOnAtrParam(context);
		}

		public override object VisitOnAtrSpc(DoGAE1PParser.OnAtrSpcContext context)
		{
			var spc = (Rgb)this.VisitColor(context.spc);
			var size = (Rgb)this.VisitColor(context.size);
			var h = float.Parse(context.h.Text);
			this.atr.setColor("spc", spc);
			this.atr.setColor("spcsize", size);
			this.atr.spcH = h;

			return base.VisitOnAtrSpc(context);
		}

		public override object VisitOnAtrMap(DoGAE1PParser.OnAtrMapContext context)
		{
			var typeMap = context.typeMap.Text;
			var pathTexture = context.file.Text;
			float min = 0.0f;
			float max = 1.0f;
			if (context.min != null) min = float.Parse(context.min.Text);
			if (context.max != null) max = float.Parse(context.max.Text);

			var infoTexture = new TextureInfo(typeMap, pathTexture, min, max);
			this.atr.addInfoTexture(typeMap, infoTexture);

			return base.VisitOnAtrMap(context);
		}

		public override object VisitOnAtrMapSize(DoGAE1PParser.OnAtrMapSizeContext context)
		{
			var lower = (Vec2)this.VisitVector2(context.lower);
			var upper = (Vec2)this.VisitVector2(context.upper);
			this.atr.setRangeMap("mapsize", new Range<Vec2>(lower, upper));
			return base.VisitOnAtrMapSize(context);
		}

		public override object VisitOnAtrMapWind(DoGAE1PParser.OnAtrMapWindContext context)
		{
			var lower = (Vec2)this.VisitVector2(context.lower);
			var upper = (Vec2)this.VisitVector2(context.upper);
			this.atr.setRangeMap("mapwind", new Range<Vec2>(lower, upper));
			return base.VisitOnAtrMapWind(context);
		}

		public override object VisitOnAtrMapView(DoGAE1PParser.OnAtrMapViewContext context)
		{
			var lower = (Vec2)this.VisitVector2(context.lower);
			var upper = (Vec2)this.VisitVector2(context.upper);
			this.atr.setRangeMap("mapview", new Range<Vec2>(lower, upper));
			return base.VisitOnAtrMapView(context);
		}

		public override object VisitOnAtrOpt(DoGAE1PParser.OnAtrOptContext context)
		{
			return base.VisitOnAtrOpt(context);
		}

		public override object VisitOnAtrOptEmission(DoGAE1PParser.OnAtrOptEmissionContext context)
		{
			this.atr.option.glowpower = float.Parse(context.glowpower.Text);
			this.atr.option.glowradius = float.Parse(context.glowradius.Text);
			return base.VisitOnAtrOptEmission(context);
		}

		public override object VisitOnAtrOptCastShadow(DoGAE1PParser.OnAtrOptCastShadowContext context)
		{
			var typeCastShadow = context.typeCastShadow.Text;
			this.atr.option.castShadow = (typeCastShadow == "castshadow") ? true : false;

			return base.VisitOnAtrOptCastShadow(context);
		}

		public override object VisitOnAtrOptReceiveShadow(DoGAE1PParser.OnAtrOptReceiveShadowContext context)
		{
			var typeReceiveShadow = context.typeReceiveShadow.Text;
			this.atr.option.receiveShadow = (typeReceiveShadow == "receiveshadow") ? true : false;
			return base.VisitOnAtrOptReceiveShadow(context);
		}

		public override object VisitOnAtrOptSelfShadow(DoGAE1PParser.OnAtrOptSelfShadowContext context)
		{
			var typeSelfShadow = context.typeSelfShadow.Text;
			this.atr.option.selfShadow = (typeSelfShadow == "selfshadow") ? true : false;
			return base.VisitOnAtrOptSelfShadow(context);
		}

		public override object VisitOnAtrOptRayTraceLevel(DoGAE1PParser.OnAtrOptRayTraceLevelContext context)
		{
			this.atr.option.raytracelevel = int.Parse(context.level.Text);
			return base.VisitOnAtrOptRayTraceLevel(context);
		}

		public override object VisitOnAtrOptDraw(DoGAE1PParser.OnAtrOptDrawContext context)
		{
			var colorDraw = (Rgb)this.VisitColor(context.col);
			var isRate = (context.isRate != null) ? true : false;
			var value = float.Parse(context.val.Text);
			if (isRate) value = -value;
			this.atr.option.drawcolor = colorDraw;
			this.atr.option.drawsize = value;
			return base.VisitOnAtrOptDraw(context);
		}

		public override object VisitOnAtrOptEmphasis(DoGAE1PParser.OnAtrOptEmphasisContext context)
		{
			this.atr.option.emphasis = float.Parse(context.val.Text);
			return base.VisitOnAtrOptEmphasis(context);
		}

		public override object VisitOnAtrOptEdge(DoGAE1PParser.OnAtrOptEdgeContext context)
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

		public override object VisitOnAtrOptCellLookSpecular(DoGAE1PParser.OnAtrOptCellLookSpecularContext context)
		{
			this.atr.option.minCellLookSpecular = float.Parse(context.min.Text);
			this.atr.option.maxCellLookSpecular = float.Parse(context.max.Text);
			return base.VisitOnAtrOptCellLookSpecular(context);
		}

		public override object VisitOnAtrOptShader(DoGAE1PParser.OnAtrOptShaderContext context)
		{
			var numGradations = int.Parse(context.gradationcount.Text);
			for (int index = 0; index < numGradations; index++) {
				var rangeGradation = (GradationRange)this.VisitGradationrange(context.gradationrange(index));
				if (rangeGradation != null) this.atr.option.addGradation(rangeGradation);
			}
			return base.VisitOnAtrOptShader(context);
		}

		public override object VisitOnAtrOptCastDecal(DoGAE1PParser.OnAtrOptCastDecalContext context)
		{
			var castDecal = float.Parse(context.val.Text);
			this.atr.option.castDecal = castDecal;
			return base.VisitOnAtrOptCastDecal(context);
		}

		public override object VisitOnAtrOptReceiveDecal(DoGAE1PParser.OnAtrOptReceiveDecalContext context)
		{
			var typeReceiveDecal = context.typeReceiveDecal.Text;
			this.atr.option.reciveDecal = (typeReceiveDecal == "receivedecal") ? true : false;
			return base.VisitOnAtrOptReceiveDecal(context);
		}

		public override object VisitGradationrange(DoGAE1PParser.GradationrangeContext context)
		{
			var min = float.Parse(context.min.Text);
			var max = float.Parse(context.max.Text);
			var color = (Rgb)this.VisitColor(context.col);
			string pathTexture = "";
			if (context.texture != null) pathTexture = context.texture.Text;
			var rangeGradation = new GradationRange(min, max, color, pathTexture);
			return rangeGradation;
		}

		public override object VisitColor(DoGAE1PParser.ColorContext context)
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


		public override object VisitVector3(DoGAE1PParser.Vector3Context context)
		{
			return new Vec3(float.Parse(context.x.Text),
				float.Parse(context.y.Text), float.Parse(context.z.Text));
		}

		public override object VisitVector2(DoGAE1PParser.Vector2Context context)
		{
			return new Vec2(float.Parse(context.x.Text), float.Parse(context.y.Text));
		}
	}
}
