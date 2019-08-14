exprrightassoc:substitute= "expr"
termrightassoc:substitute= "term"
expr -> term exprrightassoc
term -> factor termrightassoc
factor -> lparen expr rparen
factor -> int
exprrightassoc -> add term exprrightassoc
exprrightassoc ->
termrightassoc -> mul factor termrightassoc
termrightassoc ->

add= '\+'
mul= '\*'
lparen= '\('
rparen= '\)'
int= '([0-9]+)'
