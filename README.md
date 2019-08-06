# PCK: The Parser Construction Kit

PCK is designed to let you work with different parser generator tools but is a parser generator tool in its own right.

It can generate FA based lexers and PDA based parsers on the LL(1) algorithm, with an eye toward supporting LL(1) conflict resolution in the future.

It has a rich "XBNF" grammar format that can be used to construct grammars to pass to parser generators, including YACC, with an extensible translation feature that allows for creation of future grammar and lexer file transformations.

Pckw/Pckp breaks up the different parser / lexer generator tasks into console accessible operations
 
They're both the same executable, although Pckw is the windows version, and Pckp is the .NET core version. Functionally they are identical.
 
```
Usage: pckw <command> [<arguments>]

Commands:

pckw fagen [<specfile> [<outputfile>]] [/class <classname>] [/namespace <namespace>] [/language <language>]

  <specfile>    The pck specification file to use (or stdin)
  <outputfile>  The file to write (or stdout)
  <classname>   The name of the class to generate (or taken from the filename or from the start symbol of the grammar)
  <namespace>   The namespace to generate the code under (or none)
  <language>    The .NET language to generate the code for (or draw from filename or C#)

  Generates an FA tokenizer/lexer in the specified .NET language.

pckw ll1gen [<specfile> [<outputfile>]] [/class <classname>] [/namespace <namespace>] [/language <language>]

  <specfile>    The pck specification file to use (or stdin)
  <outputfile>  The file to write (or stdout)
  <classname>   The name of the class to generate (or taken from the filename or from the start symbol of the grammar)
  <namespace>   The namespace to generate the code under (or none)
  <language>    The .NET language to generate the code for (or draw from filename or C#)

  Generates an LL(1) parser in the specified .NET language.

pckw ll1factor [<specfile> [<outputfile>]]

  <specfile>    The pck specification file to use (or stdin)
  <outputfile>  The file to write (or stdout)

  Factors a pck grammar spec so that it can be used with an LL(1) parser.


pckw ll1tree <specfile> [<inputfile>]

  <specfile>    The pck specification file to use
  <inputfile>   The file to parse (or stdin)

  Prints a tree from the specified input file using the specified pck specification file.

pckw xlt [<inputfile> [<outputfile>]] [/transform <transform>] [/assembly <assembly>]

  <inputfile>   The input file to use (or stdin)
  <outputfile>  The file to write (or stdout)
  <transform>   The name of the transform to use (or taken from the input and/or output filenames)
  <assembly>    The assembly to reference

  Translates an input format to an output format.

  Available transforms include:

   pckToLex     Translates a pck spec to a lex/flex spec
   pckToYacc    Translates a pck spec to a yacc spec
   xbnfToPck    Translates an xbnf grammar to a pck spec.
```

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

`pckw ll1factor xbnf.pck xbnf.ll1.pck`


you can then generate the code for it or export it whatever

`pckw fagen xbnf.ll1.pck XbnfTokenizer.cs`

`pckw ll1gen xbnf.ll1.pck XbnfParser.cs`


or you can pipe these operations. Like, turn an xbnf grammar into a parser:

`pckw xlt xbnf.xbnf /transform xbnfToPck | pckw ll1factor | pckw ll1gen /class XbnfParser > XbnfParser.cs`

I plan on adding more xlt transformations. They're relatively easy to implement, considering. Right now it's just lex and yacc

The lexer transformations won't happen until I can implement arden's theorem in c# (any help?)
