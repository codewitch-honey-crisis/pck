expr-> expr add term
expr-> term
term-> term mul factor
term-> factor
factor-> lparen expr rparen
factor-> int

add= '\+'
mul= '\*'
lparen= '\('
rparen= '\)'
int= '([0-9]+)'
