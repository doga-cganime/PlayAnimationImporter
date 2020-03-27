grammar DoGASuf;
import util;

objs
	:	(obj)*		# onObjs
	;

obj
	: 'obj' 'suf' name=NAME block	# onObjSuf
	;

block
	: '{' (statement)* '}'
	;

statement
	:	'atr' name=(STRING | NAME)												# onAtr
	|	'prim' 'poly' '(' position=vector3+ ')'									# onPrimPoly									
	|	'prim' 'shade' '(' (position=vector3 normal=vector3)+ ')'				# onPrimShade									
	|	'prim' 'uvpoly' '(' (position=vector3 uv=vector2)+ ')'					# onPrimUvPoly
	|	'prim' 'uvshade' '(' (position=vector3 normal=vector3 uv=vector2)+ ')'	# onPrimUvShade									
	;
