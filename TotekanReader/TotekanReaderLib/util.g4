/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

grammar util;

vector3
	: x=NUMBER y=NUMBER z=NUMBER
	;

vector2
	: x=NUMBER y=NUMBER
	;

FILE_PATH
	: NAME ( [:\\\.] NAME )+
	;

NAME
	: [a-zA-Z_] [a-zA-Z0-9_\-]+
	| [0-9]+ [a-zA-Z] [a-zA-Z0-9_\-]+
	;

NUMBER
	:	'-'? INT '.' DECIMAL
	|	'-'? INT
    |	'-'? '.' DECIMAL
	;

STRING
	: '"' ~'"'* '"'
	;

INT
	:	'0' | [1-9] [0-9]* ;

DECIMAL
	:	[0-9]+ ;

WS
	:	[ \t\n\r]+	->	skip;

CTRLZ
	:	[\z] -> skip;
