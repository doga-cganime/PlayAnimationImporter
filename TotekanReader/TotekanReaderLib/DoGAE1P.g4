grammar DoGAE1P;
import atr;

frame
	:	comment 'fram' block
	;

block
	: '{' (statement)* '}'
	;

statement
	:	block									# onBlock									
	|	'eye' 'deg' '(' angle=NUMBER ')'				# onEyeDeg
	|	'target'								# onTarget
	|	'light' 'pal' '(' colorLight=color orientLight=vector3 ')'	# onLight
	|	typeTransform=('mov' | 'scal') '(' vec=vector3 ')'			# onTransform
	|	typeRotation=('rotx' | 'roty' | 'rotz') '(' angle=NUMBER ')'		# onRotate
	|	'obj' nameObj=(NAME | STRING | 'light') pathObj=obj_path						# onObj
	;

comment
	: '/*' comment_title comment_version ( comment_parts | comment_infos | comment_palettes | comment_light | comment_background | comment_motion_preview)* '*/' 
	;

comment_title
	: 'Parts' 'Assembler'
	;

comment_version
	: 'Version' ( version_number | NUMBER ) ( '(:' | '\u03b2')?
	;

comment_parts
	: 'Parts:' part=comment_part*
	| 'Parts:' part0='C:\\Program' part1=comment_part*
	;

comment_part
	: '(' STRING+ ')'
	| FILE_PATH
	;

comment_light
	: 'Light:' '(' vector3 ')' '(' vector3 ')'
	;

comment_background
	: 'BackGround:' STRING ( NUMBER? | 'UseStage' )
	;

comment_motion_preview
	: 'MotionPreview:' NUMBER NUMBER '(' vector3 ')' '(' vector3 ')'
	;

comment_infos
	: 'Info:' comment_info*
	;

comment_info
	: 'DisplayOffset:' '(' vector3 ')' 
	| ( 'DisplayScale:' | 'MeshFlag:' | 'MeshSpacing:' | 'ShowStatus:' | 'ShowAttr:' | 'GridFlag:' | 'GridSpacing:' | 'RotSpacing:' | 'SelectedOnly:' | 'DispRelative:' ) NUMBER
	;

comment_palettes
	: 'Palette:' palette=comment_palette*
	;

comment_palette
	: ( INDEX atr )						# onAtrPalette
	;

obj_path
	: '/*' path=STRING 'atr' name=STRING '*/'
	| '/*' path=FILE_PATH 'atr' name=STRING '*/'
	| '/*' path=STRING atrObj=NAME* '*/'
	| '/*' path=FILE_PATH atrObj=NAME* '*/'
	;

version_number
	: NUMBER NUMBER+
	;

INDEX
	: INT ':'
	;
