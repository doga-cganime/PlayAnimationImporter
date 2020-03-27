using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

namespace TotekanReaderLib
{

	public class DoGASufVisitor : DoGASufBaseVisitor<object>
	{
		public List<DoGASuf> listSufs = new List<DoGASuf>();
		private DoGASuf suf = null;
		private string nameAtr;

		public DoGASufVisitor()
		{
		}

		public override object VisitOnObjs(DoGASufParser.OnObjsContext context)
		{
			var result = base.VisitOnObjs(context);
			return this.listSufs;
		}

		public override object VisitOnObjSuf(DoGASufParser.OnObjSufContext context)
		{
			this.suf = new DoGASuf(context.name.Text);
			var result = base.VisitOnObjSuf(context);
			this.listSufs.Add(suf);
			this.suf = null;

			return result;
		}

		public override object VisitOnPrimPoly(DoGASufParser.OnPrimPolyContext context)
		{
			var contextVector3 = context.vector3();
			int numVertices = contextVector3.Length;
			var prim = new PrimPoly();
			for (int index = 0; index < numVertices; index++) {
				var position = (Vec3)this.VisitVector3(contextVector3[index]);
				prim.addVertex(position);
			}
			this.suf.addPrim(this.nameAtr, false, prim);
			return prim;
		}

		public override object VisitOnPrimShade(DoGASufParser.OnPrimShadeContext context)
		{
			var contextPosition = context.position;
			var contextVector3 = context.vector3();
			int numVertices = contextVector3.Length / 2;
			var prim = new PrimShade();
			for (int index = 0; index < numVertices; index++) {
				var position = (Vec3)this.VisitVector3(contextVector3[index * 2]);
				var normal = (Vec3)this.VisitVector3(contextVector3[index * 2 + 1]);
				prim.addVertex(position, normal);
			}

			this.suf.addPrim(this.nameAtr, false, prim);
			return prim;
		}

		public override object VisitOnPrimUvPoly(DoGASufParser.OnPrimUvPolyContext context)
		{
			var contextVector3 = context.vector3();
			var contextVector2 = context.vector2();
			int numVertices = contextVector3.Length;
			var prim = new PrimUVPoly();
			for (int index = 0; index < numVertices; index++) {
				var position = (Vec3)this.VisitVector3(contextVector3[index]);
				var uv = (Vec2)this.VisitVector2(contextVector2[index]);
				prim.addVertex(position, uv);
			}
			this.suf.addPrim(this.nameAtr, true, prim);
			return prim;
		}

		public override object VisitOnPrimUvShade(DoGASufParser.OnPrimUvShadeContext context)
		{
			var contextVector3 = context.vector3();
			var contextVector2 = context.vector2();
			int numVertices = contextVector2.Length;
			var prim = new PrimUVShade();
			for (int index = 0; index < numVertices; index++) {
				var position = (Vec3)this.VisitVector3(contextVector3[index * 2]);
				var normal = (Vec3)this.VisitVector3(contextVector3[index * 2 + 1]);
				var uv = (Vec2)this.VisitVector2(contextVector2[index]);
				prim.addVertex(position, normal, uv);
			}
			this.suf.addPrim(this.nameAtr, true, prim);
			return prim;
		}

		public override object VisitOnAtr(DoGASufParser.OnAtrContext context)
		{
			this.nameAtr = StringUtil.stripQuote(context.name.Text.ToLower());
			return base.VisitOnAtr(context);
		}

		public override object VisitVector3(DoGASufParser.Vector3Context context)
		{
			return new Vec3(float.Parse(context.x.Text),
				float.Parse(context.y.Text), float.Parse(context.z.Text));
		}

		public override object VisitVector2(DoGASufParser.Vector2Context context)
		{
			return new Vec2(float.Parse(context.x.Text), float.Parse(context.y.Text));
		}
	}
}
