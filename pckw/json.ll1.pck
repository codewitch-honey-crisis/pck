json:start
fields:collapsed
values:collapsed
lbracket:collapsed
rbracket:collapsed
lbrace:collapsed
rbrace:collapsed
colon:collapsed
comma:collapsed
whitespace:hidden
objectpart:collapsed
fieldspart:collapsed
arraypart:collapsed
valuespart:collapsed
json -> object
json -> array
field -> string colon value
value -> string
value -> number
value -> object
value -> array
value -> boolean
value -> null
boolean -> true
boolean -> false
object -> lbrace objectpart
objectpart -> rbrace
objectpart -> fields rbrace
fields -> field fieldspart
fieldspart ->
fieldspart -> comma fields
array -> lbracket arraypart
arraypart -> rbracket
arraypart -> values rbracket
values -> value valuespart
valuespart ->
valuespart -> comma values

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
