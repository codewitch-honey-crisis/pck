json:start
fields:collapsed
values:collapsed
whitespace:hidden
json-> object
json-> array
object-> lbrace rbrace
object-> lbrace fields rbrace
fields-> field
fields-> field comma fields
field-> string colon value
array-> lbracket rbracket
array-> lbracket values rbracket
values-> value
values-> value comma values
value-> string
value-> number
value-> object
value-> array
value-> boolean
value-> null
boolean-> true
boolean-> false

number= '(\-?(0|[1-9][0-9]*)(\.[0-9]+)?([Ee][\+\-]?[0-9]+)?)'
string= '("([^"\\]|\\.)*")'
true= 'true'
false= 'false'
null= 'null'
lbracket= '\['
rbracket= '\]'
lbrace= '\{'
rbrace= '\}'
colon= ':'
comma= ','
whitespace= '([ \t\r\n\f\v]+)'
