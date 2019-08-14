grammar:start
productions:collapsed
expressions:collapsed
attributes:collapsed
attribute:color= "red"
attrvalue:color= "orange"
whitespace:hidden
lineComment:hidden, color= "green"
blockComment:hidden, blockEnd= "*/", color= "green"
expressionlisttail:collapsed
symbollist:collapsed
attributelisttail:collapsed
expressionlisttailrightassoc:collapsed
symbollistrightassoc:collapsed
attributelisttailrightassoc:collapsed
productionspart:collapsed
productionpart:collapsed
expressionspart:collapsed
symbolpart:collapsed
attributespart:collapsed
attributepart:collapsed
grammar -> productions
expression -> symbollist
expression ->
symbol -> literal
symbol -> regex
symbol -> identifier
symbol -> lparen expressions rparen
symbol -> lbracket expressions rbracket
attrvalue -> literal
attrvalue -> integer
attrvalue -> identifier
expressionlisttail -> or expression expressionlisttailrightassoc
symbollist -> symbol symbollistrightassoc
attributelisttail -> comma attribute attributelisttailrightassoc
expressionlisttailrightassoc -> or expression expressionlisttailrightassoc
expressionlisttailrightassoc ->
symbollistrightassoc -> symbol symbollistrightassoc
symbollistrightassoc ->
attributelisttailrightassoc -> comma attribute attributelisttailrightassoc
attributelisttailrightassoc ->
productions -> production productionspart
productionspart -> productions
productionspart ->
production -> identifier productionpart
productionpart -> lt attributes gt eq expressions semi
productionpart -> eq expressions semi
expressions -> expression expressionspart
expressionspart -> expressionlisttail
expressionspart ->
symbol -> lbrace expressions symbolpart
symbolpart -> rbrace
symbolpart -> rbracePlus
attributes -> attribute attributespart
attributespart -> attributelisttail
attributespart ->
attribute -> identifier attributepart
attributepart -> eq attrvalue
attributepart ->

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
= '\}\+'
