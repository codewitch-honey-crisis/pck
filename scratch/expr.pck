E-> T E`
E`-> add T E`
E`->
T-> F T`
T`-> mul F T`
T`->
F-> lparen E rparen
F-> int
add='\+'
mul='\*'
lparen='\('
rparen='\)'
int='[0-9]+'

