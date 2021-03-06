namespace Pck {
    
    public class XbnfParser : Pck.LL1TableParser {
        public const int grammar = 0;
        public const int concatExpression = 1;
        public const int symbol = 2;
        public const int subexpression = 3;
        public const int optionalExpression = 4;
        public const int repeatZero = 5;
        public const int repeatOne = 6;
        public const int attrvalue = 7;
        public const int symbollist = 8;
        public const int attributelisttail = 9;
        public const int symbollistrightassoc = 10;
        public const int attributelisttailrightassoc = 11;
        public const int productions = 12;
        public const int productionspart = 13;
        public const int production = 14;
        public const int productionpart = 15;
        public const int orExpression = 16;
        public const int orExpressionpart = 17;
        public const int repeatExpression = 18;
        public const int repeatExpressionpart = 19;
        public const int attributes = 20;
        public const int attributespart = 21;
        public const int attribute = 22;
        public const int attributepart = 23;
        public const int literal = 24;
        public const int regex = 25;
        public const int identifier = 26;
        public const int lparen = 27;
        public const int rparen = 28;
        public const int lbracket = 29;
        public const int rbracket = 30;
        public const int rbrace = 31;
        public const int rbracePlus = 32;
        public const int integer = 33;
        public const int comma = 34;
        public const int eq = 35;
        public const int semi = 36;
        public const int or = 37;
        public const int lbrace = 38;
        public const int lt = 39;
        public const int gt = 40;
        public const int whitespace = 41;
        public const int lineComment = 42;
        public const int blockComment = 43;
        public const int _EOS = 44;
        public const int _ERROR = 45;
        static string[] _Symbols = new string[] {
                "grammar",
                "concatExpression",
                "symbol",
                "subexpression",
                "optionalExpression",
                "repeatZero",
                "repeatOne",
                "attrvalue",
                "symbollist",
                "attributelisttail",
                "symbollistrightassoc",
                "attributelisttailrightassoc",
                "productions",
                "productionspart",
                "production",
                "productionpart",
                "orExpression",
                "orExpressionpart",
                "repeatExpression",
                "repeatExpressionpart",
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
                "rbrace",
                "rbracePlus",
                "integer",
                "comma",
                "eq",
                "semi",
                "or",
                "lbrace",
                "lt",
                "gt",
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
                                12},
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
                                8},
                        new int[] {
                                8},
                        new int[] {
                                8},
                        new int[] {
                                8},
                        new int[0],
                        new int[] {
                                8},
                        new int[0],
                        new int[0],
                        new int[0],
                        null,
                        null,
                        null,
                        new int[0],
                        new int[0],
                        new int[] {
                                8},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                24},
                        new int[] {
                                25},
                        new int[] {
                                26},
                        new int[] {
                                3},
                        null,
                        new int[] {
                                4},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                18},
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
                        new int[] {
                                27,
                                16,
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
                        new int[] {
                                29,
                                16,
                                30},
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
                        new int[] {
                                31},
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
                                32},
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
                                24},
                        null,
                        new int[] {
                                26},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                33},
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
                                10},
                        new int[] {
                                2,
                                10},
                        new int[] {
                                2,
                                10},
                        new int[] {
                                2,
                                10},
                        null,
                        new int[] {
                                2,
                                10},
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
                                10},
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
                                34,
                                22,
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
                        null},
                new int[][] {
                        new int[] {
                                2,
                                10},
                        new int[] {
                                2,
                                10},
                        new int[] {
                                2,
                                10},
                        new int[] {
                                2,
                                10},
                        new int[0],
                        new int[] {
                                2,
                                10},
                        new int[0],
                        new int[0],
                        new int[0],
                        null,
                        null,
                        null,
                        new int[0],
                        new int[0],
                        new int[] {
                                2,
                                10},
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
                                34,
                                22,
                                11},
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        new int[] {
                                14,
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
                        new int[] {
                                12},
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
                                26,
                                15},
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
                        null,
                        new int[] {
                                35,
                                16,
                                36},
                        null,
                        null,
                        null,
                        new int[] {
                                20,
                                35,
                                16,
                                36},
                        null,
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        new int[] {
                                1,
                                17},
                        null,
                        null,
                        null,
                        new int[] {
                                1,
                                17},
                        null,
                        new int[] {
                                1,
                                17},
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
                        new int[0],
                        new int[0],
                        null,
                        null,
                        null,
                        new int[0],
                        new int[] {
                                37,
                                16},
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
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                38,
                                16,
                                19},
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
                        new int[] {
                                5},
                        new int[] {
                                6},
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
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                39,
                                22,
                                21},
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
                                9,
                                40},
                        null,
                        null,
                        null,
                        null,
                        null,
                        new int[] {
                                40},
                        null,
                        null,
                        null,
                        null,
                        null},
                new int[][] {
                        null,
                        null,
                        new int[] {
                                26,
                                23},
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
                        new int[0],
                        new int[] {
                                35,
                                7},
                        null,
                        null,
                        null,
                        null,
                        new int[0],
                        null,
                        null,
                        null,
                        null,
                        null}};
        static int[] _InitCfg = new int[] {
                0,
                24};
        static int[] _NodeFlags = new int[] {
                0,
                0,
                1,
                1,
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
                0,
                1,
                0,
                1,
                0,
                1,
                0,
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
        static int[] _Substitutions = new int[] {
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1};
        static Pck.ParseAttribute[][] _AttributeSets = new Pck.ParseAttribute[][] {
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("start", true)},
                null,
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                null,
                null,
                null,
                null,
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                null,
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                null,
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                null,
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                null,
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
                null,
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("collapsed", true)},
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
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("hidden", true),
                        new Pck.ParseAttribute("terminal", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("hidden", true)},
                new Pck.ParseAttribute[] {
                        new Pck.ParseAttribute("hidden", true),
                        new Pck.ParseAttribute("blockEnd", "*/")},
                null,
                null};
        public XbnfParser(Pck.ITokenizer tokenizer) : 
                base(XbnfParser._ParseTable, XbnfParser._InitCfg, XbnfParser._Symbols, XbnfParser._NodeFlags, XbnfParser._Substitutions, XbnfParser._AttributeSets, tokenizer) {
        }
        public XbnfParser() : 
                this(null) {
        }
    }
}
