%token identifier
%token lt
%token gt
%token eq
%token semi
%token literal
%token regex
%token lparen
%token rparen
%token lbracket
%token rbracket
%token lbrace
%token rbrace
%token rbracePlus
%token integer
%token or
%token comma
%%
grammar: productions;
productions: production productions
	| production;
production: identifier lt attributes gt eq expressions semi
	| identifier eq expressions semi;
expressions: expression expressionlisttail
	| expression;
expression: symbollist
	|;
symbol: literal
	| regex
	| identifier
	| lparen expressions rparen
	| lbracket expressions rbracket
	| lbrace expressions rbrace
	| lbrace expressions rbracePlus;
attributes: attribute attributelisttail
	| attribute;
attribute: identifier eq attrvalue
	| identifier;
attrvalue: literal
	| integer
	| identifier;
expressionlisttail: expressionlisttail or expression
	| or expression;
symbollist: symbollist symbol
	| symbol;
attributelisttail: attributelisttail comma attribute
	| comma attribute;
