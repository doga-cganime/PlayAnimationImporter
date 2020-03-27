grammar DoGAAtr;
import atr;

atrs
	:	atr*	# onAtrs
	;

BLOCK_COMMENT
	: '/*' .*? '*/' -> skip
	;
