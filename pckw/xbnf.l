%%
("([^"\\]|\\.)*")	return literal;
(\'([^\'\\]|\\.)*\')	return regex;
([A-Z_a-z][\-0-9A-Z_a-z]*)	return identifier;
(\-?[0-9]+)	return integer;
(( |(\v|(\f|(\t|(\r|\n))))))+	;
(//[^\n]*[\n])	;
/\*	; /* TODO: implement blockend */
\|	return or;
\<	return lt;
\>	return gt;
=	return eq;
\;	return semi;
,	return comma;
\(	return lparen;
\)	return rparen;
\[	return lbracket;
\]	return rbracket;
\{	return lbrace;
\}	return rbrace;
\}\+	return rbracePlus;
