using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public enum EdgeType {
		None,
		Normal,
		CellLook,
		Canceller
	}

    [Serializable]
	public class DoGAAtrOption
	{
		// 'emittion' glowradius=NUMBER glowpower=NUMBER 
		public float? glowradius = null;
		public float? glowpower = null;
		public bool? castShadow = null;
		public bool? selfShadow = null;
		public bool? receiveShadow = null;
		public int? raytracelevel = null;

		//	| 'draw' color 'rate'? NUMBER
		public Rgb drawcolor = null;
		public float? drawsize = null; // rage があると -1 倍した値を代入

		public float? emphasis = null;

		// 'edge'
		// ('celllookedge' | 'cellookedge' )
		// ('edgecancel' | 'edgecanceler' )
		public EdgeType? edgetype = null;

		// ( 'celllookspecular' | 'cellookspecular' ) min=NUMBER max=NUMBER
		public float? minCellLookSpecular = null;
		public float? maxCellLookSpecular = null;

		//	| 'shader' gradationcount=NUMBER gradationrange+
		List<GradationRange> rangesGradation = new List<GradationRange>();

		//	| 'castdecal' NUMBER
		public float? castDecal = null;

		//	| 'noreceivedecal'
		//	| 'receivedecal'
		public bool? reciveDecal = null;
	
		public DoGAAtrOption()
		{
		}

		public void addGradation(GradationRange range)
		{
			this.rangesGradation.Add(range);
		}

		public void clear()
		{
			this.glowradius = null;
			this.glowpower = null;
			this.castShadow = null;
			this.selfShadow = null;
			this.receiveShadow = null;
			this.raytracelevel = null;
			this.drawsize = null;
			this.emphasis = null;
			this.edgetype = null;
			this.minCellLookSpecular = null;
			this.maxCellLookSpecular = null;
			this.castDecal = null;
			this.reciveDecal = null;

			this.rangesGradation.Clear();
		}

		public void merge(DoGAAtrOption atr)
		{
			if (this.glowradius == null) this.glowradius = atr.glowradius;
			if (this.glowpower == null) this.glowpower = atr.glowpower;
			if (this.castShadow == null) this.castShadow = atr.castShadow;
			if (this.selfShadow == null) this.selfShadow = atr.selfShadow;
			if (this.receiveShadow == null) this.receiveShadow = atr.receiveShadow;
			if (this.raytracelevel == null) this.raytracelevel = atr.raytracelevel;
			if (this.drawsize == null) this.drawsize = atr.drawsize;
			if (this.emphasis == null) this.emphasis = atr.emphasis;
			if (this.edgetype == null) this.edgetype = atr.edgetype;
			if (this.minCellLookSpecular == null) this.minCellLookSpecular = atr.minCellLookSpecular;
			if (this.maxCellLookSpecular == null) this.maxCellLookSpecular = atr.maxCellLookSpecular;
			if (this.castDecal == null) this.castDecal = atr.castDecal;
			if (this.reciveDecal == null) this.reciveDecal = atr.reciveDecal;
		}

		public void setDefaultToEmpty()
		{
			if (this.glowradius == null) this.glowradius = 0.0f;
			if (this.glowpower == null ) this.glowpower = 0.0f;
			if (this.castShadow == null) this.castShadow = false;
			if (this.selfShadow == null) this.selfShadow = false;
			if (this.receiveShadow == null) this.receiveShadow = false;
			if (this.raytracelevel == null) this.raytracelevel = 0;
			if (this.drawsize == null) this.drawsize = 0.0f;
			if (this.emphasis == null) this.emphasis = 0.0f;
			if (this.edgetype == null) this.edgetype = EdgeType.None;
			if (this.minCellLookSpecular == null) this.minCellLookSpecular = 0.0f;
			if (this.maxCellLookSpecular == null) this.maxCellLookSpecular = 0.0f;
			if (this.castDecal == null) this.castDecal = 0.0f;
			if (this.reciveDecal == null) this.reciveDecal = false;
		}
	}
}
