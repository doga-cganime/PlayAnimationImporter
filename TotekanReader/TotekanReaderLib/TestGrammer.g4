grammar TestGrammer;

/*
 * Parser Rules
 */

r  : 'hello' ID ;

/*
 * Lexer Rules
 */

ID : [a-z]+ ;
WS : [ \r\t\n]+ -> skip ;
