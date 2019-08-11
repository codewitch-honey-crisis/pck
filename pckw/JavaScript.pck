Literal -> Null_Literal
Literal -> Boolean_Literal
Literal -> Numeric_Literal
Literal -> StringLiteral
Null_Literal -> null
Boolean_Literal -> true
Boolean_Literal -> false
Numeric_Literal -> DecimalLiteral
Numeric_Literal -> HexIntegerLiteral
Regular_Expression_Literal -> RegExp
Primary_Expression -> this
Primary_Expression -> Identifier
Primary_Expression -> Literal
Primary_Expression -> Array_Literal
Primary_Expression -> Object_Literal
Primary_Expression -> implicit11 Expression implicit12
Primary_Expression -> Regular_Expression_Literal
Array_Literal -> implicit19 implicit20
Array_Literal -> implicit19 Elision implicit20
Array_Literal -> implicit19 Element_List implicit20
Array_Literal -> implicit19 Element_List implicit14 Elision implicit20
Elision -> implicit14
Elision -> Elision implicit14
Element_List -> Elision Assignment_Expression
Element_List -> Element_List implicit14 Elision Assignment_Expression
Element_List -> Element_List implicit14 Assignment_Expression
Element_List -> Assignment_Expression
Object_Literal -> implicit22 Property_Name_and_Value_List implicit26
Property_Name_and_Value_List -> Property_Name implicit17 Assignment_Expression
Property_Name_and_Value_List -> Property_Name_and_Value_List implicit14 Property_Name implicit17 Assignment_Expression
Property_Name -> Identifier
Property_Name -> StringLiteral
Property_Name -> Numeric_Literal
Member_Expression -> Primary_Expression
Member_Expression -> Function_Expression
Member_Expression -> Member_Expression implicit19 Expression implicit20
Member_Expression -> Member_Expression . Identifier
Member_Expression -> new Member_Expression Arguments
New_Expression -> Member_Expression
New_Expression -> new New_Expression
Call_Expression -> Member_Expression Arguments
Call_Expression -> Call_Expression Arguments
Call_Expression -> Call_Expression implicit19 Expression implicit20
Call_Expression -> Call_Expression . Identifier
Arguments -> implicit11 implicit12
Arguments -> implicit11 Argument_List implicit12
Argument_List -> Assignment_Expression
Argument_List -> Argument_List implicit14 Assignment_Expression
Left_Hand_Side_Expression -> New_Expression
Left_Hand_Side_Expression -> Call_Expression
Postfix_Expression -> Left_Hand_Side_Expression
Postfix_Expression -> Postfix_Expression ++
Postfix_Expression -> Postfix_Expression implicit6
Unary_Expression -> Postfix_Expression
Unary_Expression -> delete Unary_Expression
Unary_Expression -> void Unary_Expression
Unary_Expression -> typeof Unary_Expression
Unary_Expression -> ++ Unary_Expression
Unary_Expression -> implicit6 Unary_Expression
Unary_Expression -> + Unary_Expression
Unary_Expression -> implicit5 Unary_Expression
Unary_Expression -> ~ Unary_Expression
Unary_Expression -> ! Unary_Expression
Multiplicative_Expression -> Unary_Expression
Multiplicative_Expression -> Unary_Expression * Multiplicative_Expression
Multiplicative_Expression -> Unary_Expression implicit15 Multiplicative_Expression
Multiplicative_Expression -> Unary_Expression % Multiplicative_Expression
Additive_Expression -> Additive_Expression + Multiplicative_Expression
Additive_Expression -> Additive_Expression implicit5 Multiplicative_Expression
Additive_Expression -> Multiplicative_Expression
Shift_Expression -> Shift_Expression implicit29 Additive_Expression
Shift_Expression -> Shift_Expression implicit38 Additive_Expression
Shift_Expression -> Shift_Expression implicit40 Additive_Expression
Shift_Expression -> Additive_Expression
Relational_Expression -> Shift_Expression
Relational_Expression -> Relational_Expression implicit28 Shift_Expression
Relational_Expression -> Relational_Expression implicit36 Shift_Expression
Relational_Expression -> Relational_Expression implicit31 Shift_Expression
Relational_Expression -> Relational_Expression implicit37 Shift_Expression
Relational_Expression -> Relational_Expression instanceof Shift_Expression
Equality_Expression -> Relational_Expression
Equality_Expression -> Equality_Expression implicit34 Relational_Expression
Equality_Expression -> Equality_Expression implicit7 Relational_Expression
Equality_Expression -> Equality_Expression implicit35 Relational_Expression
Equality_Expression -> Equality_Expression implicit8 Relational_Expression
Bitwise_And_Expression -> Equality_Expression
Bitwise_And_Expression -> Bitwise_And_Expression & Equality_Expression
Bitwise_XOr_Expression -> Bitwise_And_Expression
Bitwise_XOr_Expression -> Bitwise_XOr_Expression ^ Bitwise_And_Expression
Bitwise_Or_Expression -> Bitwise_XOr_Expression
Bitwise_Or_Expression -> Bitwise_Or_Expression implicit23 Bitwise_XOr_Expression
Logical_And_Expression -> Bitwise_Or_Expression
Logical_And_Expression -> Logical_And_Expression && Bitwise_Or_Expression
Logical_Or_Expression -> Logical_And_Expression
Logical_Or_Expression -> Logical_Or_Expression implicit24 Logical_And_Expression
Conditional_Expression -> Logical_Or_Expression
Conditional_Expression -> Logical_Or_Expression ? Assignment_Expression implicit17 Assignment_Expression
Assignment_Expression -> Conditional_Expression
Assignment_Expression -> Left_Hand_Side_Expression Assignment_Operator Assignment_Expression
Assignment_Operator -> implicit32
Assignment_Operator -> implicit13
Assignment_Operator -> implicit16
Assignment_Operator -> implicit9
Assignment_Operator -> implicit27
Assignment_Operator -> implicit33
Assignment_Operator -> implicit30
Assignment_Operator -> implicit39
Assignment_Operator -> implicit41
Assignment_Operator -> implicit10
Assignment_Operator -> implicit21
Assignment_Operator -> implicit25
Expression -> Assignment_Expression
Expression -> Expression implicit14 Assignment_Expression
Statement -> Block
Statement -> Variable_Statement
Statement -> Empty_Statement
Statement -> If_Statement
Statement -> If_Else_Statement
Statement -> Iteration_Statement
Statement -> Continue_Statement
Statement -> Break_Statement
Statement -> Return_Statement
Statement -> With_Statement
Statement -> Labelled_Statement
Statement -> Switch_Statement
Statement -> Throw_Statement
Statement -> Try_Statement
Statement -> Expression
Block -> implicit22 implicit26
Block -> implicit22 Statement_List implicit26
Statement_List -> Statement
Statement_List -> Statement_List Statement
Variable_Statement -> var Variable_Declaration_List implicit18
Variable_Declaration_List -> Variable_Declaration
Variable_Declaration_List -> Variable_Declaration_List implicit14 Variable_Declaration
Variable_Declaration -> Identifier
Variable_Declaration -> Identifier Initializer
Initializer -> implicit32 Assignment_Expression
Empty_Statement -> implicit18
If_Statement -> if implicit11 Expression implicit12 Statement
If_Else_Statement -> if implicit11 Expression implicit12 Statement else Statement
Iteration_Statement -> do Statement while implicit11 Expression implicit12 implicit18
Iteration_Statement -> while implicit11 Expression implicit12 Statement
Iteration_Statement -> for implicit11 Expression implicit18 Expression implicit18 Expression implicit12 Statement
Iteration_Statement -> for implicit11 var Variable_Declaration_List implicit18 Expression implicit18 Expression implicit12 Statement
Iteration_Statement -> for implicit11 Left_Hand_Side_Expression in Expression implicit12 Statement
Iteration_Statement -> for implicit11 var Variable_Declaration in Expression implicit12 Statement
Continue_Statement -> continue implicit18
Continue_Statement -> continue Identifier implicit18
Break_Statement -> break implicit18
Break_Statement -> break Identifier implicit18
Return_Statement -> return implicit18
Return_Statement -> return Expression implicit18
With_Statement -> with implicit11 Expression implicit12 Statement implicit18
Switch_Statement -> switch implicit11 Expression implicit12 Case_Block
Case_Block -> implicit22 implicit26
Case_Block -> implicit22 Case_Clauses implicit26
Case_Block -> implicit22 Case_Clauses Default_Clause implicit26
Case_Block -> implicit22 Case_Clauses Default_Clause Case_Clauses implicit26
Case_Block -> implicit22 Default_Clause Case_Clauses implicit26
Case_Block -> implicit22 Default_Clause implicit26
Case_Clauses -> Case_Clause
Case_Clauses -> Case_Clauses Case_Clause
Case_Clause -> case Expression implicit17 Statement_List
Case_Clause -> case Expression implicit17
Default_Clause -> default implicit17
Default_Clause -> default implicit17 Statement_List
Labelled_Statement -> Identifier implicit17 Statement
Throw_Statement -> throw Expression
Try_Statement -> try Block Catch
Try_Statement -> try Block Finally
Try_Statement -> try Block Catch Finally
Catch -> catch implicit11 Identifier implicit12 Block
Finally -> finally Block
Function_Declaration -> function Identifier implicit11 Formal_Parameter_List implicit12 implicit22 Function_Body implicit26
Function_Declaration -> function Identifier implicit11 implicit12 implicit22 Function_Body implicit26
Function_Expression -> function implicit11 implicit12 implicit22 Function_Body implicit26
Function_Expression -> function implicit11 Formal_Parameter_List implicit12 implicit22 Function_Body implicit26
Formal_Parameter_List -> Identifier
Formal_Parameter_List -> Formal_Parameter_List implicit14 Identifier
Function_Body -> Source_Elements
Function_Body ->
Program -> Source_Elements
Source_Elements -> Source_Element
Source_Elements -> Source_Elements Source_Element
Source_Element -> Statement
Source_Element -> Function_Declaration

// TODO: Finish porting the lex output
Whitespace= '[\t-\r  ][\t-\r  ]*'
implicit11= ''
implicit12= ''
implicit14= ','
.= '.'
implicit17= ':'
implicit18= '\;'
?= '\?'
implicit19= ''
implicit20= '\]'
implicit22= '\{'
implicit26= '\}'
~= '~'
Identifier= '[\$A-Z_aghj-mo-qux-z][\$0-9A-Z_a-z][\$0-9A-Z_a-z]*\?|b([\$0-9A-Z_a-qs-z][\$0-9A-Z_a-z]*|r([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e([\$0-9A-Z_b-z][\$0-9A-Z_a-z]*|a([\$0-9A-Z_a-jl-z][\$0-9A-Z_a-z]*|k[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?|c([\$0-9A-Z_b-np-z][\$0-9A-Z_a-z]*|a([\$0-9A-Z_a-ru-z][\$0-9A-Z_a-z]*|s([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|t([\$0-9A-Z_abd-z][\$0-9A-Z_a-z]*|c([\$0-9A-Z_a-gi-z][\$0-9A-Z_a-z]*|h[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?|o([\$0-9A-Z_a-mo-z][\$0-9A-Z_a-z]*|n([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t([\$0-9A-Z_a-hj-z][\$0-9A-Z_a-z]*|i([\$0-9A-Z_a-mo-z][\$0-9A-Z_a-z]*|n([\$0-9A-Z_a-tv-z][\$0-9A-Z_a-z]*|u([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?)?)?)?|d([\$0-9A-Z_a-df-np-z][\$0-9A-Z_a-z]*|e([\$0-9A-Z_a-eg-km-z][\$0-9A-Z_a-z]*|f([\$0-9A-Z_b-z][\$0-9A-Z_a-z]*|a([\$0-9A-Z_a-tv-z][\$0-9A-Z_a-z]*|u([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?|l([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?|o[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|e([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l([\$0-9A-Z_a-rt-z][\$0-9A-Z_a-z]*|s([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?|f([\$0-9A-Z_b-hj-np-tv-z][\$0-9A-Z_a-z]*|a([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l([\$0-9A-Z_a-rt-z][\$0-9A-Z_a-z]*|s([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?|i([\$0-9A-Z_a-mo-z][\$0-9A-Z_a-z]*|n([\$0-9A-Z_b-z][\$0-9A-Z_a-z]*|a([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l([\$0-9A-Z_a-xz][\$0-9A-Z_a-z]*|y[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?)?|o([\$0-9A-Z_a-qs-z][\$0-9A-Z_a-z]*|r[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|u([\$0-9A-Z_a-mo-z][\$0-9A-Z_a-z]*|n([\$0-9A-Z_abd-z][\$0-9A-Z_a-z]*|c([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t([\$0-9A-Z_a-hj-z][\$0-9A-Z_a-z]*|i([\$0-9A-Z_a-np-z][\$0-9A-Z_a-z]*|o([\$0-9A-Z_a-mo-z][\$0-9A-Z_a-z]*|n[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?)?)?)?|i([\$0-9A-Z_a-eg-mo-z][\$0-9A-Z_a-z]*|f[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*|n([\$0-9A-Z_a-rt-z][\$0-9A-Z_a-z]*|s([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t([\$0-9A-Z_b-z][\$0-9A-Z_a-z]*|a([\$0-9A-Z_a-mo-z][\$0-9A-Z_a-z]*|n([\$0-9A-Z_abd-z][\$0-9A-Z_a-z]*|c([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e([\$0-9A-Z_a-np-z][\$0-9A-Z_a-z]*|o([\$0-9A-Z_a-eg-z][\$0-9A-Z_a-z]*|f[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?)?)?)?))?|n([\$0-9A-Z_a-df-tv-z][\$0-9A-Z_a-z]*|e([\$0-9A-Z_a-vx-z][\$0-9A-Z_a-z]*|w[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|u([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?|r([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t([\$0-9A-Z_a-tv-z][\$0-9A-Z_a-z]*|u([\$0-9A-Z_a-qs-z][\$0-9A-Z_a-z]*|r([\$0-9A-Z_a-mo-z][\$0-9A-Z_a-z]*|n[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?)?|s([\$0-9A-Z_a-vx-z][\$0-9A-Z_a-z]*|w([\$0-9A-Z_a-hj-z][\$0-9A-Z_a-z]*|i([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t([\$0-9A-Z_abd-z][\$0-9A-Z_a-z]*|c([\$0-9A-Z_a-gi-z][\$0-9A-Z_a-z]*|h[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?)?|t([\$0-9A-Z_a-gi-qs-xz][\$0-9A-Z_a-z]*|h([\$0-9A-Z_a-hj-qs-z][\$0-9A-Z_a-z]*|i([\$0-9A-Z_a-rt-z][\$0-9A-Z_a-z]*|s[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|r([\$0-9A-Z_a-np-z][\$0-9A-Z_a-z]*|o([\$0-9A-Z_a-vx-z][\$0-9A-Z_a-z]*|w[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?|r([\$0-9A-Z_a-tv-xz][\$0-9A-Z_a-z]*|u([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|y[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|y([\$0-9A-Z_a-oq-z][\$0-9A-Z_a-z]*|p([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e([\$0-9A-Z_a-np-z][\$0-9A-Z_a-z]*|o([\$0-9A-Z_a-eg-z][\$0-9A-Z_a-z]*|f[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?)?)?|v([\$0-9A-Z_b-np-z][\$0-9A-Z_a-z]*|a([\$0-9A-Z_a-qs-z][\$0-9A-Z_a-z]*|r[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?|o([\$0-9A-Z_a-hj-z][\$0-9A-Z_a-z]*|i([\$0-9A-Z_a-ce-z][\$0-9A-Z_a-z]*|d[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?|w([\$0-9A-Z_a-gj-z][\$0-9A-Z_a-z]*|h([\$0-9A-Z_a-hj-z][\$0-9A-Z_a-z]*|i([\$0-9A-Z_a-km-z][\$0-9A-Z_a-z]*|l([\$0-9A-Z_a-df-z][\$0-9A-Z_a-z]*|e[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?|i([\$0-9A-Z_a-su-z][\$0-9A-Z_a-z]*|t([\$0-9A-Z_a-gi-z][\$0-9A-Z_a-z]*|h[\$0-9A-Z_a-z][\$0-9A-Z_a-z]*)?)?)?'
StringLiteral= ''
!= '!'
implicit7= '!='
implicit8= '!=='
%= '%'
implicit9= '%='
&= '&'
&&= '&&'
implicit10= '&='
*= '\*'
implicit2= '\*\/'
implicit13= '\*='
+= '\+'
++= '\++'
implicit27= '\+='
implicit5= '-'
implicit6= '--'
implicit33= '-='
implicit15= '\/'
implicit3= '\/\/'
implicit16= '\/='
RegExp= ''
implicit4= '\/*'
DecimalLiteral= '0.[0-9](e[1-9](0[0-9]*|[1-9]*)|E[1-9](0[0-9]*|[1-9]*)|[0-9]*)?|[1-9](0[0-9]*|.(e[1-9](0[0-9]*|[1-9]*)|E[1-9](0[0-9]*|[1-9]*)|[0-9](e[1-9](0[0-9]*|[1-9]*)|E[1-9](0[0-9]*|[1-9]*)|[0-9]*))?|[1-9]*)'
HexIntegerLiteral= '0x[0-9A-Fa-f][0-9A-Fa-f]*'
implicit28= '\<'
implicit31= '\<='
implicit29= '\<\<'
implicit30= '\<\<='
implicit32= '='
implicit34= '=='
implicit35= '==='
implicit36= '\>'
implicit37= '\>='
implicit38= '\>\>'
implicit39= '\>\>='
implicit40= '\>\>\>'
implicit41= '\>\>\>='
^= '\^'
implicit21= '\^='
break= 'break'
case= 'case'
catch= 'catch'
continue= 'continue'
default= 'default'
delete= 'delete'
do= 'do'
else= 'else'
false= 'false'
finally= 'finally'
for= 'for'
function= 'function'
if= 'if'
in= 'in'
instanceof= 'instanceof'
new= 'new'
null= 'null'
return= 'return'
switch= 'switch'
this= 'this'
throw= 'throw'
true= 'true'
try= 'try'
typeof= 'typeof'
var= 'var'
void= 'void'
while= 'while'
with= 'with'
implicit23= ''
implicit24= ''
implicit25= '|='
