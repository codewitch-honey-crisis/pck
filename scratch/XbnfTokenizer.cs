public class XbnfTokenizer : Pck.TableTokenizer {
    public const int literal = 17;
    public const int regex = 18;
    public const int identifier = 12;
    public const int integer = 26;
    public const int whitespace = 29;
    public const int lineComment = 30;
    public const int blockComment = 31;
    public const int or = 27;
    public const int lt = 13;
    public const int gt = 14;
    public const int eq = 15;
    public const int semi = 16;
    public const int comma = 28;
    public const int lparen = 19;
    public const int rparen = 20;
    public const int lbracket = 21;
    public const int rbracket = 22;
    public const int lbrace = 23;
    public const int rbrace = 24;
    public const int rbracePlus = 25;
    public const int _EOS = 32;
    public const int _ERROR = 33;
    static string[] _Symbols = new string[] {
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
            "identifier",
            "lt",
            "gt",
            "eq",
            "semi",
            "literal",
            "regex",
            "lparen",
            "rparen",
            "lbracket",
            "rbracket",
            "lbrace",
            "rbrace",
            "rbracePlus",
            "integer",
            "or",
            "comma",
            "whitespace",
            "lineComment",
            "blockComment",
            "#EOS",
            "#ERROR"};
    static string[] _BlockEnds = new string[] {
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
            "*/",
            null,
            null};
    static System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>[] _DfaTable = new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>[] {
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 1),
                        new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 6),
                        new System.Collections.Generic.KeyValuePair<string, int>("AZ__az", 11),
                        new System.Collections.Generic.KeyValuePair<string, int>("--", 13),
                        new System.Collections.Generic.KeyValuePair<string, int>("09", 14),
                        new System.Collections.Generic.KeyValuePair<string, int>("  ", 15),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 16),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 17),
                        new System.Collections.Generic.KeyValuePair<string, int>("\t\t", 18),
                        new System.Collections.Generic.KeyValuePair<string, int>("\r\r", 19),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 20),
                        new System.Collections.Generic.KeyValuePair<string, int>("//", 21),
                        new System.Collections.Generic.KeyValuePair<string, int>("||", 26),
                        new System.Collections.Generic.KeyValuePair<string, int>("<<", 27),
                        new System.Collections.Generic.KeyValuePair<string, int>(">>", 28),
                        new System.Collections.Generic.KeyValuePair<string, int>("==", 29),
                        new System.Collections.Generic.KeyValuePair<string, int>(";;", 30),
                        new System.Collections.Generic.KeyValuePair<string, int>(",,", 31),
                        new System.Collections.Generic.KeyValuePair<string, int>("((", 32),
                        new System.Collections.Generic.KeyValuePair<string, int>("))", 33),
                        new System.Collections.Generic.KeyValuePair<string, int>("[[", 34),
                        new System.Collections.Generic.KeyValuePair<string, int>("]]", 35),
                        new System.Collections.Generic.KeyValuePair<string, int>("{{", 36),
                        new System.Collections.Generic.KeyValuePair<string, int>("}}", 37)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0!#[]￿", 2),
                        new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 3),
                        new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 5)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0!#[]￿", 2),
                        new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 3),
                        new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 5)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0￿", 4)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0!#[]￿", 2),
                        new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 3),
                        new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 5)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(17, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0&([]￿", 7),
                        new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 8),
                        new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 10)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0&([]￿", 7),
                        new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 8),
                        new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 10)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0￿", 9)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0&([]￿", 7),
                        new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 8),
                        new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 10)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(18, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(12, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("--09AZ__az", 12)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(12, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("--09AZ__az", 12)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("09", 14)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(26, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("09", 14)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(29, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("  ", 15),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 16),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 17),
                        new System.Collections.Generic.KeyValuePair<string, int>("\t\t", 18),
                        new System.Collections.Generic.KeyValuePair<string, int>("\r\r", 19),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 20)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(29, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("  ", 15),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 16),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 17),
                        new System.Collections.Generic.KeyValuePair<string, int>("\t\t", 18),
                        new System.Collections.Generic.KeyValuePair<string, int>("\r\r", 19),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 20)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(29, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("  ", 15),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 16),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 17),
                        new System.Collections.Generic.KeyValuePair<string, int>("\t\t", 18),
                        new System.Collections.Generic.KeyValuePair<string, int>("\r\r", 19),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 20)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(29, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("  ", 15),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 16),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 17),
                        new System.Collections.Generic.KeyValuePair<string, int>("\t\t", 18),
                        new System.Collections.Generic.KeyValuePair<string, int>("\r\r", 19),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 20)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(29, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("  ", 15),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 16),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 17),
                        new System.Collections.Generic.KeyValuePair<string, int>("\t\t", 18),
                        new System.Collections.Generic.KeyValuePair<string, int>("\r\r", 19),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 20)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(29, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("  ", 15),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 16),
                        new System.Collections.Generic.KeyValuePair<string, int>("", 17),
                        new System.Collections.Generic.KeyValuePair<string, int>("\t\t", 18),
                        new System.Collections.Generic.KeyValuePair<string, int>("\r\r", 19),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 20)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("//", 22),
                        new System.Collections.Generic.KeyValuePair<string, int>("**", 25)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0\t￿", 23),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 24)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("\0\t￿", 23),
                        new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 24)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(30, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(31, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(27, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(13, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(14, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(15, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(16, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(28, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(19, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(20, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(21, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(22, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(23, new System.Collections.Generic.KeyValuePair<string, int>[0]),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(24, new System.Collections.Generic.KeyValuePair<string, int>[] {
                        new System.Collections.Generic.KeyValuePair<string, int>("++", 38)}),
            new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(25, new System.Collections.Generic.KeyValuePair<string, int>[0])};
    public XbnfTokenizer(System.Collections.Generic.IEnumerable<char> input) : 
            base(XbnfTokenizer._DfaTable, XbnfTokenizer._Symbols, XbnfTokenizer._BlockEnds, input) {
    }
}
