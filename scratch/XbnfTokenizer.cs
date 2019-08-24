namespace Pck {
    
    public class XbnfTokenizer : Pck.TableTokenizer {
        public const int literal = 24;
        public const int regex = 25;
        public const int identifier = 26;
        public const int integer = 33;
        public const int whitespace = 41;
        public const int lineComment = 42;
        public const int blockComment = 43;
        public const int or = 39;
        public const int lt = 35;
        public const int gt = 36;
        public const int eq = 37;
        public const int semi = 38;
        public const int comma = 34;
        public const int lparen = 27;
        public const int rparen = 28;
        public const int lbracket = 29;
        public const int rbracket = 30;
        public const int lbrace = 40;
        public const int rbrace = 31;
        public const int rbracePlus = 32;
        public const int _EOS = 44;
        public const int _ERROR = 45;
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
                "lt",
                "gt",
                "eq",
                "semi",
                "or",
                "lbrace",
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
        static System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>[] _DfaTable = new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>[] {
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\"',
                                        '\"'}, 1),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\'',
                                        '\''}, 6),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        'A',
                                        'Z',
                                        '_',
                                        '_',
                                        'a',
                                        'z'}, 11),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '-',
                                        '-'}, 13),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '0',
                                        '9'}, 14),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 16),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 17),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 20),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '/',
                                        '/'}, 21),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '|',
                                        '|'}, 26),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '<',
                                        '<'}, 27),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '>',
                                        '>'}, 28),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '=',
                                        '='}, 29),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ';',
                                        ';'}, 30),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ',',
                                        ','}, 31),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '(',
                                        '('}, 32),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ')',
                                        ')'}, 33),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '[',
                                        '['}, 34),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ']',
                                        ']'}, 35),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '{',
                                        '{'}, 36),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '}',
                                        '}'}, 37)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '!',
                                        '#',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 2),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\\',
                                        '\\'}, 3),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\"',
                                        '\"'}, 5)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '!',
                                        '#',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 2),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\\',
                                        '\\'}, 3),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\"',
                                        '\"'}, 5)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        ((char)(65535))}, 4)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '!',
                                        '#',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 2),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\\',
                                        '\\'}, 3),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\"',
                                        '\"'}, 5)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(24, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '&',
                                        '(',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 7),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\\',
                                        '\\'}, 8),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\'',
                                        '\''}, 10)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '&',
                                        '(',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 7),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\\',
                                        '\\'}, 8),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\'',
                                        '\''}, 10)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        ((char)(65535))}, 9)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '&',
                                        '(',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 7),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\\',
                                        '\\'}, 8),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\'',
                                        '\''}, 10)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(25, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(26, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '-',
                                        '-',
                                        '0',
                                        '9',
                                        'A',
                                        'Z',
                                        '_',
                                        '_',
                                        'a',
                                        'z'}, 12)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(26, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '-',
                                        '-',
                                        '0',
                                        '9',
                                        'A',
                                        'Z',
                                        '_',
                                        '_',
                                        'a',
                                        'z'}, 12)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '0',
                                        '9'}, 14)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(33, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '0',
                                        '9'}, 14)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(41, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 16),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 17),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(41, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 16),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 17),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(41, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 16),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 17),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(41, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 16),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 17),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(41, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 16),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 17),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(41, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 16),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '',
                                        ''}, 17),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '/',
                                        '/'}, 22),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '*',
                                        '*'}, 25)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '\t',
                                        '',
                                        ((char)(65535))}, 23),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 24)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(-1, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\0',
                                        '\t',
                                        '',
                                        ((char)(65535))}, 23),
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '\n',
                                        '\n'}, 24)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(42, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(43, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(39, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(35, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(36, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(37, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(38, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(34, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(27, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(28, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(29, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(30, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(40, new System.Collections.Generic.KeyValuePair<char[], int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(31, new System.Collections.Generic.KeyValuePair<char[], int>[] {
                            new System.Collections.Generic.KeyValuePair<char[], int>(new char[] {
                                        '+',
                                        '+'}, 38)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<char[], int>[]>(32, new System.Collections.Generic.KeyValuePair<char[], int>[0])};
        public XbnfTokenizer(System.Collections.Generic.IEnumerable<char> input) : 
                base(XbnfTokenizer._DfaTable, XbnfTokenizer._Symbols, XbnfTokenizer._BlockEnds, input) {
        }
    }
}
