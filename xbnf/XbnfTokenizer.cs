namespace Pck {
    
    public class XbnfTokenizer : Pck.TableTokenizer {
        public const int literal = 24;
        public const int regex = 25;
        public const int identifier = 26;
        public const int integer = 33;
        public const int whitespace = 41;
        public const int lineComment = 42;
        public const int blockComment = 43;
        public const int or = 37;
        public const int lt = 39;
        public const int gt = 40;
        public const int eq = 35;
        public const int semi = 36;
        public const int comma = 34;
        public const int lparen = 27;
        public const int rparen = 28;
        public const int lbracket = 29;
        public const int rbracket = 30;
        public const int lbrace = 38;
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
        static Pck.CharDfaEntry[] _DfaTable = new Pck.CharDfaEntry[] {
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\"',
                                        '\"'}, 1),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\'',
                                        '\''}, 6),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        'A',
                                        'Z',
                                        '_',
                                        '_',
                                        'a',
                                        'z'}, 11),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '-',
                                        '-'}, 13),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '0',
                                        '9'}, 14),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 16),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 17),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 20),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '/',
                                        '/'}, 21),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '|',
                                        '|'}, 26),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '<',
                                        '<'}, 27),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '>',
                                        '>'}, 28),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '=',
                                        '='}, 29),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ';',
                                        ';'}, 30),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ',',
                                        ','}, 31),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '(',
                                        '('}, 32),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ')',
                                        ')'}, 33),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '[',
                                        '['}, 34),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ']',
                                        ']'}, 35),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '{',
                                        '{'}, 36),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '}',
                                        '}'}, 37)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '!',
                                        '#',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 2),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\\',
                                        '\\'}, 3),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\"',
                                        '\"'}, 5)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '!',
                                        '#',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 2),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\\',
                                        '\\'}, 3),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\"',
                                        '\"'}, 5)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        ((char)(65535))}, 4)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '!',
                                        '#',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 2),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\\',
                                        '\\'}, 3),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\"',
                                        '\"'}, 5)}),
                new Pck.CharDfaEntry(24, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '&',
                                        '(',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 7),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\\',
                                        '\\'}, 8),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\'',
                                        '\''}, 10)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '&',
                                        '(',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 7),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\\',
                                        '\\'}, 8),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\'',
                                        '\''}, 10)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        ((char)(65535))}, 9)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '&',
                                        '(',
                                        '[',
                                        ']',
                                        ((char)(65535))}, 7),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\\',
                                        '\\'}, 8),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\'',
                                        '\''}, 10)}),
                new Pck.CharDfaEntry(25, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(26, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
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
                new Pck.CharDfaEntry(26, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
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
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '0',
                                        '9'}, 14)}),
                new Pck.CharDfaEntry(33, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '0',
                                        '9'}, 14)}),
                new Pck.CharDfaEntry(41, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 16),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 17),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new Pck.CharDfaEntry(41, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 16),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 17),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new Pck.CharDfaEntry(41, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 16),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 17),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new Pck.CharDfaEntry(41, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 16),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 17),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new Pck.CharDfaEntry(41, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 16),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 17),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new Pck.CharDfaEntry(41, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        ' ',
                                        ' '}, 15),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 16),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '',
                                        ''}, 17),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\t',
                                        '\t'}, 18),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\r',
                                        '\r'}, 19),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 20)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '/',
                                        '/'}, 22),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '*',
                                        '*'}, 25)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '\t',
                                        '',
                                        ((char)(65535))}, 23),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 24)}),
                new Pck.CharDfaEntry(-1, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\0',
                                        '\t',
                                        '',
                                        ((char)(65535))}, 23),
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '\n',
                                        '\n'}, 24)}),
                new Pck.CharDfaEntry(42, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(43, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(37, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(39, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(40, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(35, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(36, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(34, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(27, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(28, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(29, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(30, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(38, new Pck.CharDfaTransitionEntry[0]),
                new Pck.CharDfaEntry(31, new Pck.CharDfaTransitionEntry[] {
                            new Pck.CharDfaTransitionEntry(new char[] {
                                        '+',
                                        '+'}, 38)}),
                new Pck.CharDfaEntry(32, new Pck.CharDfaTransitionEntry[0])};
        public XbnfTokenizer(System.Collections.Generic.IEnumerable<char> input) : 
                base(XbnfTokenizer._DfaTable, XbnfTokenizer._Symbols, XbnfTokenizer._BlockEnds, input) {
        }
    }
}
