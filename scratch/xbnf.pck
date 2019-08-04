grammar:start
productions:collapsed
expressions:collapsed
attributes:collapsed
attribute:color= "red"
attrvalue:color= "orange"
whitespace:hidden, terminal
lineComment:hidden, color= "green"
blockComment:hidden, blockEnd= "*/", color= "green"
expressionlisttail:collapsed
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
attributes-> attribute attributelisttail
attributes-> attribute
attribute-> identifier eq attrvalue
attribute-> identifier
attrvalue-> literal
attrvalue-> integer
attrvalue-> identifier
expressionlisttail-> expressionlisttail or expression
expressionlisttail-> or expression
symbollist-> symbollist symbol
symbollist-> symbol
attributelisttail-> attributelisttail comma attribute
attributelisttail-> comma attribute

literal= '("([^"\\]|\\.)*")'
regex= '(\'([^\'\\]|\\.)*\')'
identifier= '([A-Z_a-z][\-0-9A-Z_a-z]*)'
integer= '(\-?[0-9]+)'
whitespace= '(( |(\v|(\f|(\t|(\r|\n))))))+'
lineComment= '(//[^\n]*[\n])'
blockComment= '/\*'
or= '\|'
lt= '\<'
gt= '\>'
eq= '='
semi= '\;'
comma= ','
lparen= '\('
rparen= '\)'
lbracket= '\['
rbracket= '\]'
lbrace= '\{'
rbrace= '\}'
rbracePlus= '\}\+'
