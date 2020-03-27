grammar atr;
import util;

atr
	:	'atr' name=(STRING | NAME) atr_block # onAtr
	;

atr_block
	: '{' (atr_statement)* '}'?                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
	;

atr_statement
	:	typeParam=('col' | 'amb' | 'dif' | 'tra' | 'ref' | 'rfr' | 'size') '(' col=color ')'	# onAtrParam
	|	'spc' '(' spc=color size=color h=NUMBER ')'	# onAtrSpc
	|	typeMap=('colormap' | 'bumpmap' | 'ambmap' | 'difmap' | 'tramap' | 'spcmap' | 'sizemap' | 'refmap' | 'rfrmap' | 'hmap' | 'glowpowermap' | 'glowradiusmap') '(' file=(STRING | FILE_PATH) (min=NUMBER max=NUMBER)? ')'	# onAtrMap
	|	'mapsize' '(' lower=vector2 upper=vector2 ')'								# onAtrMapSize
	|	'mapview' '(' lower=vector2 upper=vector2 ')'								# onAtrMapView
	|	'mapwind' '(' lower=vector2 upper=vector2 ')'								# onAtrMapWind
	|	'opt' '(' option* ')'												# onAtrOpt
	;

option
	: 'emittion' glowradius=NUMBER glowpower=NUMBER				# onAtrOptEmission
	| typeCastShadow=('nocastshadow' | 'castshadow')			# onAtrOptCastShadow
	| typeReceiveShadow=('noreceiveshadow' | 'receiveshadow' )	# onAtrOptReceiveShadow
	| typeSelfShadow=('noselfshadow' | 'selfshadow' )			# onAtrOptSelfShadow
	| 'raytracelevel' level=NUMBER								# onAtrOptRayTraceLevel
	| 'draw' col=color isRate='rate'? val=NUMBER					# onAtrOptDraw
	| 'emphasis' val=NUMBER										# onAtrOptEmphasis
	| typeEdge=('edge' | 'celllookedge' | 'cellookedge' | 'edgecancel' | 'edgecanceler' ) # onAtrOptEdge
	| ( 'celllookspecular' | 'cellookspecular' ) min=NUMBER max=NUMBER	# onAtrOptCellLookSpecular
	| 'shader' gradationcount=NUMBER gradationrange+			# onAtrOptShader
	| 'castdecal' val=NUMBER									# onAtrOptCastDecal
	| typeReceiveDecal=('noreceivedecal' | 'receivedecal')				# onAtrOptReceiveDecal
	;

gradationrange
	:	'(' min=NUMBER max=NUMBER col=color texture=STRING? ')'
	;

color
	:  'rgb' '(' vec=vector3 ')'
	|  val=NUMBER
	;
