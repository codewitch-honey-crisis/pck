# PCK: The Parser Construction Kit

PCK is designed to let you work with different parser generator tools but is a parser generator tool in its own right.

It can generate FA based lexers and PDA based parsers on the LL(1) algorithm, with an eye toward supporting LL(1) conflict resolution in the future.

It has a rich "XBNF" grammar format that can be used to construct grammars to pass to parser generators, including YACC, with an extensible translation feature that allows for creation of future grammar and lexer file transformations.

 Pckw/Pckp breaks up the different parser / lexer generator tasks into console accessible operations
 
 They're both the same executable, although Pckw is the windows version, and Pckp is the .NET core version. Functionally they are identical.
 
 I've broken up the different parser / lexer generator tasks into console accessible operations

so - if you want to take an XBNF document

like this

```grammar<start>= productions;

productions<collapsed> = production productions | production;
production= identifier [ "<" attributes ">" ] "=" expressions ";";

expressions<collapsed>= expression { "|" expression }; 
expression= { symbol };
symbol= literal | regex | identifier | 
	"(" expressions ")" | 
	"[" expressions "]" |
	"{" expressions ("}"|"}+");
...
```

and turn into a pck spec (which you have to do before you can do much else with it)

```grammar:start
productions:collapsed
expressions:collapsed
symbollist:collapsed
attributelisttail:collapsed
grammar-> productions
productions-> production productions
productions-> production
production-> identifier lt attributes gt eq expressions semi
production-> identifier eq expressions semi
expressions-> expression expressionlisttail
expressions-> expression
expression-> symbollist
expression->
symbol-> literal
symbol-> regex
symbol-> identifier
symbol-> lparen expressions rparen
symbol-> lbracket expressions rbracket
symbol-> lbrace expressions rbrace
symbol-> lbrace expressions rbracePlus
...
```

you'd use the command line like

`pckw xlt xbnf.xbnf xbnf.pck`


now, before you can use it with an LL(1) parser (including mine, or say Coco/R) you have to "factor" it

from the command line

`pckw ll1 xbnf.pck xbnf.ll1.pck`


you can then generate the code for it or export it whatever

`pckw fagen xbnf.ll1.pck XbnfTokenizer.cs`

`pckw ll1gen xbnf.ll1.pck XbnfParser.cs`


or you can pipe these operations. Like, turn an xbnf grammar into a parser:

`pckw xlt xbnf.xbnf /transform xbnfToPck | pckw ll1 | pckw ll1gen /class XbnfParser > XbnfParser.cs`

