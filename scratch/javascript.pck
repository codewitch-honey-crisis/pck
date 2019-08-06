stringLiteral:terminal
whitespace:hidden
blockComment:blockEnd= "*/", hidden
lineComment:hidden
program:start
literal-> nullLiteral
literal-> booleanLiteral
literal-> numericLiteral
literal-> stringLiteral
numericLiteral-> DecimalLiteral
numericLiteral-> HexIntegerLiteral
primaryExpression-> implicit
primaryExpression-> identifier
primaryExpression-> literal
primaryExpression-> arrayLiteral
primaryExpression-> objectLiteral
primaryExpression-> implicit2 expression implicit3
arrayLiteral-> implicit4 implicit5
arrayLiteral-> implicit4 elision implicit5
arrayLiteral-> implicit4 elementList implicit5
arrayLiteral-> implicit4 elementList implicit6 elision implicit5
elision-> implicit6
elision-> elision implicit6
elementList-> elision assignmentExpression
elementList-> elementList implicit6 elision assignmentExpression
elementList-> elementList implicit6 assignmentExpression
elementList-> assignmentExpression
objectLiteral-> implicit7 propertyNameAndValueList implicit8
propertyNameAndValueList-> propertyName implicit9 assignmentExpression
propertyNameAndValueList-> propertyNameAndValueList implicit6 propertyName implicit9 assignmentExpression
propertyName-> identifier
propertyName-> stringLiteral
propertyName-> numericLiteral
memberExpression-> primaryExpression
memberExpression-> functionExpression
memberExpression-> memberExpression implicit4 expression implicit5
memberExpression-> memberExpression implicit10 identifier
memberExpression-> implicit11 memberExpression arguments
newExpression-> memberExpression
newExpression-> implicit11 newExpression
callExpression-> memberExpression arguments
callExpression-> callExpression arguments
callExpression-> callExpression implicit4 expression implicit5
callExpression-> callExpression implicit10 identifier
arguments-> implicit2 implicit3
arguments-> implicit2 argumentList implicit3
argumentList-> assignmentExpression
argumentList-> argumentList implicit6 assignmentExpression
leftHandSideExpression-> newExpression
leftHandSideExpression-> callExpression
postfixExpression-> leftHandSideExpression
postfixExpression-> postfixExpression implicit12
postfixExpression-> postfixExpression implicit13
unaryExpression-> postfixExpression
unaryExpression-> implicit14 unaryExpression
unaryExpression-> implicit15 unaryExpression
unaryExpression-> implicit16 unaryExpression
unaryExpression-> implicit12 unaryExpression
unaryExpression-> implicit13 unaryExpression
unaryExpression-> implicit17 unaryExpression
unaryExpression-> implicit18 unaryExpression
unaryExpression-> implicit19 unaryExpression
unaryExpression-> implicit20 unaryExpression
multiplicativeExpression-> unaryExpression
multiplicativeExpression-> unaryExpression implicit21 multiplicativeExpression
multiplicativeExpression-> unaryExpression implicit22 multiplicativeExpression
multiplicativeExpression-> unaryExpression implicit23 multiplicativeExpression
additiveExpression-> additiveExpression implicit17 multiplicativeExpression
additiveExpression-> additiveExpression implicit18 multiplicativeExpression
additiveExpression-> multiplicativeExpression
shiftExpression-> shiftExpression implicit24 additiveExpression
shiftExpression-> shiftExpression implicit25 additiveExpression
shiftExpression-> shiftExpression implicit26 additiveExpression
shiftExpression-> additiveExpression
relationalExpression-> shiftExpression
relationalExpression-> relationalExpression implicit27 shiftExpression
relationalExpression-> relationalExpression implicit28 shiftExpression
relationalExpression-> relationalExpression implicit29 shiftExpression
relationalExpression-> relationalExpression implicit30 shiftExpression
relationalExpression-> relationalExpression implicit31 shiftExpression
equalityExpression-> relationalExpression
equalityExpression-> equalityExpression implicit32 relationalExpression
equalityExpression-> equalityExpression implicit33 relationalExpression
equalityExpression-> equalityExpression implicit34 relationalExpression
equalityExpression-> equalityExpression implicit35 relationalExpression
bitwiseAndExpression-> equalityExpression
bitwiseAndExpression-> bitwiseAndExpression implicit36 equalityExpression
bitwiseXorExpression-> bitwiseAndExpression
bitwiseXorExpression-> bitwiseXorExpression implicit37 bitwiseAndExpression
bitwiseOrExpression-> bitwiseXorExpression
bitwiseOrExpression-> bitwiseOrExpression implicit38 bitwiseXorExpression
logicalAndExpression-> bitwiseOrExpression
logicalAndExpression-> logicalAndExpression implicit39 bitwiseOrExpression
logicalOrExpression-> logicalAndExpression
logicalOrExpression-> logicalOrExpression implicit40 logicalAndExpression
conditionalExpression-> logicalOrExpression
conditionalExpression-> logicalOrExpression implicit41 assignmentExpression implicit9 assignmentExpression
assignmentExpression-> conditionalExpression
assignmentExpression-> leftHandSideExpression assignmentOperator assignmentExpression
expression-> assignmentExpression
expression-> expression implicit6 assignmentExpression
statement-> block
statement-> variableStatement
statement-> emptyStatement
statement-> ifStatement
statement-> ifElseStatement
statement-> iterationStatement
statement-> continueStatement
statement-> breakStatement
statement-> returnStatement
statement-> withStatement
statement-> labelledStatement
statement-> throwStatement
statement-> tryStatement
statement-> expression
block-> implicit7 implicit8
block-> implicit7 statementList implicit8
statementList-> statement
statementList-> statementList statement
variableStatement-> implicit42 variableDeclarationList emptyStatement
variableDeclarationList-> variableDeclaration
variableDeclarationList-> variableDeclarationList implicit6 variableDeclaration
variableDeclaration-> identifier
variableDeclaration-> identifier initializer
initializer-> implicit43 assignmentExpression
ifStatement-> implicit44 implicit2 expression implicit3 statement
ifElseStatement-> implicit44 implicit2 expression implicit3 statement implicit45 statement
iterationStatement-> implicit46 statement implicit47 implicit2 expression implicit3 emptyStatement
iterationStatement-> implicit47 implicit2 expression implicit3 statement
iterationStatement-> implicit48 implicit2 expression emptyStatement expression emptyStatement expression implicit3 statement
iterationStatement-> implicit48 implicit2 implicit42 variableDeclarationList emptyStatement expression emptyStatement expression implicit3 statement
iterationStatement-> implicit48 implicit2 leftHandSideExpression implicit49 expression implicit3 statement
iterationStatement-> implicit48 implicit2 implicit42 variableDeclaration implicit49 expression implicit3 statement
continueStatement-> implicit50 emptyStatement
continueStatement-> implicit50 identifier emptyStatement
breakStatement-> implicit51 emptyStatement
breakStatement-> implicit51 identifier emptyStatement
returnStatement-> implicit52 emptyStatement
returnStatement-> implicit52 expression emptyStatement
withStatement-> implicit53 implicit2 expression implicit3 statement emptyStatement
labelledStatement-> identifier implicit9 statement
throwStatement-> implicit54 expression
tryStatement-> implicit55 block catch
tryStatement-> implicit55 block finally
tryStatement-> implicit55 block catch finally
catch-> implicit56 implicit2 identifier implicit3 block
finally-> implicit57 block
functionDeclaration-> implicit58 identifier implicit2 formalParameterList implicit3 implicit7 functionBody implicit8
functionDeclaration-> implicit58 identifier implicit2 implicit3 implicit7 functionBody implicit8
functionExpression-> implicit58 implicit2 implicit3 implicit7 functionBody implicit8
functionExpression-> implicit58 implicit2 formalParameterList implicit3 implicit7 functionBody implicit8
formalParameterList-> identifier
formalParameterList-> formalParameterList implicit6 identifier
functionBody-> sourceElements
functionBody->
program-> sourceElements
sourceElements-> sourceElement
sourceElements-> sourceElements sourceElement
sourceElement-> statement
sourceElement-> functionDeclaration

identifier= '([A-Za-z_][0-9A-Z_a-z]*)'
stringLiteral= '(("([^"\\]|\\.)*")|(\'([^\'\\]|\\.)*\'))'
HexIntegerLiteral= '(0x[0-9A-Fa-f]+)'
DecimalLiteral= '(\-?(0|[1-9][0-9]*)(\.[0-9]+)?([Ee][\+\-]?[0-9]+)?)'
whitespace= '([ \t\v\f\r\n]+)'
blockComment= '/\*'
lineComment= '(//[^\n]*[\n])'
nullLiteral= 'null'
booleanLiteral= '(true|false)'
assignmentOperator= '(=|(\*=|(/=|(%=|(\+=|(\-=|(\<\<=|(\>\>=|(\>\>\>=|(&=|(^=|\|=)))))))))))'
emptyStatement= '\;'
implicit= 'this'
implicit2= '\('
implicit3= '\)'
implicit4= '\['
implicit5= '\]'
implicit6= ','
implicit7= '\{'
implicit8= '\}'
implicit9= ':'
implicit10= '\.'
implicit11= 'new'
implicit12= '\+\+'
implicit13= '\-\-'
implicit14= 'delete'
implicit15= 'void'
implicit16= 'typeof'
implicit17= '\+'
implicit18= '\-'
implicit19= '~'
implicit20= '!'
implicit21= '\*'
implicit22= '/'
implicit23= '%'
implicit24= '\<\<'
implicit25= '\>\>'
implicit26= '\>\>\>'
implicit27= '\<'
implicit28= '\>'
implicit29= '\<='
implicit30= '\>='
implicit31= 'instanceof'
implicit32= '=='
implicit33= '!='
implicit34= '==='
implicit35= '!=='
implicit36= '&'
implicit37= '^'
implicit38= '\|'
implicit39= '&&'
implicit40= '\|\|'
implicit41= '\?'
implicit42= 'var'
implicit43= '='
implicit44= 'if'
implicit45= 'else'
implicit46= 'do'
implicit47= 'while'
implicit48= 'for'
implicit49= 'in'
implicit50= 'continue'
implicit51= 'break'
implicit52= 'return'
implicit53= 'with'
implicit54= 'throw'
implicit55= 'try'
implicit56= 'catch'
implicit57= 'finally'
implicit58= 'function'
