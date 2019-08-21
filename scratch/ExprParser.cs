public class ExprParser : Pck.LL1TableParser {
    public const int expr = 0;
    public const int term = 1;
    public const int factor = 2;
    public const int exprrightassoc = 3;
    public const int termrightassoc = 4;
    public const int lparen = 5;
    public const int rparen = 6;
    public const int @int = 7;
    public const int add = 8;
    public const int mul = 9;
    public const int _EOS = 10;
    public const int _ERROR = 11;
    static string[] _Symbols = new string[] {
            "expr",
            "term",
            "factor",
            "exprrightassoc",
            "termrightassoc",
            "lparen",
            "rparen",
            "int",
            "add",
            "mul",
            "#EOS",
            "#ERROR"};
    static int[][][] _ParseTable = new int[][][] {
            new int[][] {
                    new int[] {
                            1,
                            3},
                    null,
                    new int[] {
                            1,
                            3},
                    null,
                    null,
                    null,
                    null},
            new int[][] {
                    new int[] {
                            2,
                            4},
                    null,
                    new int[] {
                            2,
                            4},
                    null,
                    null,
                    null,
                    null},
            new int[][] {
                    new int[] {
                            5,
                            0,
                            6},
                    null,
                    new int[] {
                            7},
                    null,
                    null,
                    null,
                    null},
            new int[][] {
                    null,
                    new int[0],
                    null,
                    new int[] {
                            8,
                            1,
                            3},
                    null,
                    new int[0],
                    null},
            new int[][] {
                    null,
                    new int[0],
                    null,
                    new int[0],
                    new int[] {
                            9,
                            2,
                            4},
                    new int[0],
                    null}};
    static int[] _InitCfg = new int[] {
            0,
            5};
    static int[] _NodeFlags = new int[] {
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
            0};
    static int[] _Substitutions = new int[] {
            -1,
            -1,
            -1,
            0,
            1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1};
    static System.Collections.Generic.KeyValuePair<string, object>[][] _AttributeSets = new System.Collections.Generic.KeyValuePair<string, object>[][] {
            null,
            null,
            null,
            new System.Collections.Generic.KeyValuePair<string, object>[] {
                    new System.Collections.Generic.KeyValuePair<string, object>("substitute", "expr")},
            new System.Collections.Generic.KeyValuePair<string, object>[] {
                    new System.Collections.Generic.KeyValuePair<string, object>("substitute", "term")},
            null,
            null,
            null,
            null,
            null,
            null,
            null};
    public ExprParser(Pck.ITokenizer tokenizer) : 
            base(ExprParser._ParseTable, ExprParser._InitCfg, ExprParser._Symbols, ExprParser._NodeFlags, ExprParser._Substitutions, ExprParser._AttributeSets, tokenizer) {
    }
    public ExprParser() : 
            this(null) {
    }
}
