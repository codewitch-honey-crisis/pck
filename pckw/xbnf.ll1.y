%token literal
%token regex
%token identifier
%token lparen
%token rparen
%token lbracket
%token rbracket
%token integer
%token or
%token comma
%token lt
%token gt
%token eq
%token semi
%token lbrace
%token rbrace
%token rbracePlus
%token whitespace
%token lineComment
%token blockComment
%%
grammar: productions;
expression: symbollist
	|;
symbol: literal
	| regex
	| identifier
	| lparen expressions rparen
	| lbracket expressions rbracket
	| lbrace expressions symbolpart;
attrvalue: literal
	| integer
	| identifier;
expressionlisttail: or expression expressionlisttailrightassoc;
symbollist: symbol symbollistrightassoc;
attributelisttail: comma attribute attributelisttailrightassoc;
expressionlisttailrightassoc: or expression expressionlisttailrightassoc
	|;
symbollistrightassoc: symbol symbollistrightassoc
	|;
attributelisttailrightassoc: comma attribute attributelisttailrightassoc
	|;
productions: production productionspart;
productionspart: productions
	|;
production: identifier productionpart;
productionpart: lt attributes gt eq expressions semi
	| eq expressions semi;
expressions: expression expressionspart;
expressionspart: expressionlisttail
	|;
symbolpart: rbrace
	| rbracePlus;
attributes: attribute attributespart;
attributespart: attributelisttail
	|;
attribute: identifier attributepart;
attributepart: eq attrvalue
	|;
