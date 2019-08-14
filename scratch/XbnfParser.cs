namespace Pck {
    
    public class XbnfParser : Pck.LL1TableParser {
        public const int grammar = 0;
        public const int expression = 1;
        public const int symbol = 2;
        public const int attrvalue = 3;
        public const int expressionlisttail = 4;
        public const int symbollist = 5;
        public const int attributelisttail = 6;
        public const int expressionlisttailrightassoc = 7;
        public const int symbollistrightassoc = 8;
        public const int attributelisttailrightassoc = 9;
        public const int productions = 10;
        public const int productionspart = 11;
        public const int production = 12;
        public const int productionpart = 13;
        public const int expressions = 14;
        public const int expressionspart = 15;
        public const int symbolpart = 16;
        public const int attributes = 17;
        public const int attributespart = 18;
        public const int attribute = 19;
        public const int attributepart = 20;
        public const int literal = 21;
        public const int regex = 22;
        public const int identifier = 23;
        public const int lparen = 24;
        public const int rparen = 25;
        public const int lbracket = 26;
        public const int rbracket = 27;
        public const int integer = 28;
        public const int or = 29;
        public const int comma = 30;
        public const int lt = 31;
        public const int gt = 32;
        public const int eq = 33;
        public const int semi = 34;
        public const int lbrace = 35;
        public const int rbrace = 36;
        public const int rbracePlus = 37;
        public const int whitespace = 38;
        public const int lineComment = 39;
        public const int blockComment = 40;
        public const int _EOS = 41;
        public const int _ERROR = 42;
        static string[] _Symbols = new string[] {
                "grammar",
                "expression",
                "symbol",
                "attrvalue",
                "expressionlisttail",
                "symbollist",
                "attributelisttail",
                "expressionlisttailrightassoc",
                "symbollistrightassoc",
                "attributelisttailrightassoc",
                "productions",
                "productionspart",
                "production",
                "productionpart",
                "expressions",
                "expressionspart",
                "symbolpart",
                "attributes",
                "attributespart",
                "attribute",
                "attributepart",
                "literal",
                "regex",
                "identifier",
                "lparen",
                "rparen",
                "lbracket",
                "rbracket",
                "integer",
                "or",
                "comma",
                "lt",
                "gt",
                "eq",
                "semi",
                "lbrace",
                "rbrace",
                "rbracePlus",
                "whitespace",
                "lineComment",
                "blockComment",
                "#EOS",
                "#ERROR"};
        static int[][][] _ParseTable = new int[][][] {
                new int[][] {
                        null,
                        null,
                        new int[] {
                                10},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                5},
                        new int[] {
                                5},
                        new int[] {
                                5},
                        new int[] {
                                5},
                        new int[0],
                        new int[] {
                                5},
                        new int[0],
                        null,
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        new int[] {
                                5},
                        new int[0],
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                21},
                        new int[] {
                                22},
                        new int[] {
                                23},
                        new int[] {
                                24,
                                14,
                                25},
                        null,
                        new int[] {
                                26,
                                14,
                                27},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                35,
                                14,
                                16},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                21},
                        null,
                        new int[] {
                                23},
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                28},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                29,
                                1,
                                7},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                2,
                                8},
                        new int[] {
                                2,
                                8},
                        new int[] {
                                2,
                                8},
                        new int[] {
                                2,
                                8},
                        null,
                        new int[] {
                                2,
                                8},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                2,
                                8},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                30,
                                19,
                                9},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null,
                        new int[0],
                        null,
                        new int[] {
                                29,
                                1,
                                7},
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null,
                        new int[0],
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                2,
                                8},
                        new int[] {
                                2,
                                8},
                        new int[] {
                                2,
                                8},
                        new int[] {
                                2,
                                8},
                        new int[0],
                        new int[] {
                                2,
                                8},
                        new int[0],
                        null,
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        new int[] {
                                2,
                                8},
                        new int[0],
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                30,
                                19,
                                9},
                        null,
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        new int[] {
                                12,
                                11},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        new int[] {
                                10},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null},
                new int[][] {
                        null,
                        null,
                        new int[] {
                                23,
                                13},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                31,
                                17,
                                32,
                                33,
                                14,
                                34},
                        null,
                        new int[] {
                                33,
                                14,
                                34},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        new int[] {
                                1,
                                15},
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null,
                        new int[0],
                        null,
                        new int[] {
                                4},
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null,
                        new int[0],
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                36},
                        new int[] {
                                37},
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        new int[] {
                                19,
                                18},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                6},
                        null,
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        new int[] {
                                23,
                                20},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null,
                        new int[0],
                        new int[] {
                                33,
                                3},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null}};
        static int[] _InitCfg = new int[] {
                0,
                21};
        static int[] _NodeFlags = new int[] {
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                0,
                1,
                1,
                1,
                1,
                1,
                1,
                0,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                2,
                2,
                2,
                0,
                0};
        static System.Collections.Generic.KeyValuePair<string, object>[][] _AttributeSets = new System.Collections.Generic.KeyValuePair<string, object>[][] {
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("start", true)},
                null,
                null,
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("color", "orange")},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                null,
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("color", "red")},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("collapsed", true)},
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("hidden", true),
                        new System.Collections.Generic.KeyValuePair<string, object>("terminal", true)},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("hidden", true),
                        new System.Collections.Generic.KeyValuePair<string, object>("color", "green")},
                new System.Collections.Generic.KeyValuePair<string, object>[] {
                        new System.Collections.Generic.KeyValuePair<string, object>("hidden", true),
                        new System.Collections.Generic.KeyValuePair<string, object>("blockEnd", "*/"),
                        new System.Collections.Generic.KeyValuePair<string, object>("color", "green")},
                null,
                null};
        public XbnfParser(System.Collections.Generic.IEnumerable<Pck.Token> tokenizer) : 
                base(XbnfParser._ParseTable, XbnfParser._InitCfg, XbnfParser._Symbols, XbnfParser._NodeFlags,null, XbnfParser._AttributeSets, tokenizer) {
        }
        public XbnfParser() : 
                this(null) {
        }
    }
}
