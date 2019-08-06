namespace Pck {
    
    public class JSTokenizer : Pck.TableTokenizer {
        public const int identifier = 56;
        public const int stringLiteral = 57;
        public const int HexIntegerLiteral = 58;
        public const int DecimalLiteral = 59;
        public const int whitespace = 122;
        public const int blockComment = 123;
        public const int lineComment = 124;
        public const int nullLiteral = 60;
        public const int booleanLiteral = 61;
        public const int assignmentOperator = 62;
        public const int emptyStatement = 63;
        public const int @implicit = 64;
        public const int implicit2 = 65;
        public const int implicit3 = 66;
        public const int implicit4 = 67;
        public const int implicit5 = 68;
        public const int implicit6 = 69;
        public const int implicit7 = 70;
        public const int implicit8 = 71;
        public const int implicit9 = 72;
        public const int implicit10 = 73;
        public const int implicit11 = 74;
        public const int implicit12 = 75;
        public const int implicit13 = 76;
        public const int implicit14 = 77;
        public const int implicit15 = 78;
        public const int implicit16 = 79;
        public const int implicit17 = 80;
        public const int implicit18 = 81;
        public const int implicit19 = 82;
        public const int implicit20 = 83;
        public const int implicit21 = 84;
        public const int implicit22 = 85;
        public const int implicit23 = 86;
        public const int implicit24 = 87;
        public const int implicit25 = 88;
        public const int implicit26 = 89;
        public const int implicit27 = 90;
        public const int implicit28 = 91;
        public const int implicit29 = 92;
        public const int implicit30 = 93;
        public const int implicit31 = 94;
        public const int implicit32 = 95;
        public const int implicit33 = 96;
        public const int implicit34 = 97;
        public const int implicit35 = 98;
        public const int implicit36 = 99;
        public const int implicit37 = 100;
        public const int implicit38 = 101;
        public const int implicit39 = 102;
        public const int implicit40 = 103;
        public const int implicit41 = 104;
        public const int implicit42 = 105;
        public const int implicit43 = 106;
        public const int implicit44 = 107;
        public const int implicit45 = 108;
        public const int implicit46 = 109;
        public const int implicit47 = 110;
        public const int implicit48 = 111;
        public const int implicit49 = 112;
        public const int implicit50 = 113;
        public const int implicit51 = 114;
        public const int implicit52 = 115;
        public const int implicit53 = 116;
        public const int implicit54 = 117;
        public const int implicit55 = 118;
        public const int implicit56 = 119;
        public const int implicit57 = 120;
        public const int implicit58 = 121;
        public const int _EOS = 125;
        public const int _ERROR = 126;
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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
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
                "stringLiteral",
                "HexIntegerLiteral",
                "DecimalLiteral",
                "nullLiteral",
                "booleanLiteral",
                "assignmentOperator",
                "emptyStatement",
                "implicit",
                "implicit2",
                "implicit3",
                "implicit4",
                "implicit5",
                "implicit6",
                "implicit7",
                "implicit8",
                "implicit9",
                "implicit10",
                "implicit11",
                "implicit12",
                "implicit13",
                "implicit14",
                "implicit15",
                "implicit16",
                "implicit17",
                "implicit18",
                "implicit19",
                "implicit20",
                "implicit21",
                "implicit22",
                "implicit23",
                "implicit24",
                "implicit25",
                "implicit26",
                "implicit27",
                "implicit28",
                "implicit29",
                "implicit30",
                "implicit31",
                "implicit32",
                "implicit33",
                "implicit34",
                "implicit35",
                "implicit36",
                "implicit37",
                "implicit38",
                "implicit39",
                "implicit40",
                "implicit41",
                "implicit42",
                "implicit43",
                "implicit44",
                "implicit45",
                "implicit46",
                "implicit47",
                "implicit48",
                "implicit49",
                "implicit50",
                "implicit51",
                "implicit52",
                "implicit53",
                "implicit54",
                "implicit55",
                "implicit56",
                "implicit57",
                "implicit58",
                "whitespace",
                "blockComment",
                "lineComment",
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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
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
                null,
                null};
        static System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>[] _DfaTable = new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>[] {
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("AZ__aaghjmoqssuuxz", 1),
                            new System.Collections.Generic.KeyValuePair<string, int>("bb", 3),
                            new System.Collections.Generic.KeyValuePair<string, int>("cc", 8),
                            new System.Collections.Generic.KeyValuePair<string, int>("dd", 20),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 27),
                            new System.Collections.Generic.KeyValuePair<string, int>("ff", 31),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 51),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 62),
                            new System.Collections.Generic.KeyValuePair<string, int>("rr", 68),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 74),
                            new System.Collections.Generic.KeyValuePair<string, int>("vv", 90),
                            new System.Collections.Generic.KeyValuePair<string, int>("ww", 96),
                            new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 104),
                            new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 109),
                            new System.Collections.Generic.KeyValuePair<string, int>("00", 114),
                            new System.Collections.Generic.KeyValuePair<string, int>("--", 123),
                            new System.Collections.Generic.KeyValuePair<string, int>("19", 125),
                            new System.Collections.Generic.KeyValuePair<string, int>("\t\t\n\n\r  ", 129),
                            new System.Collections.Generic.KeyValuePair<string, int>("//", 130),
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 136),
                            new System.Collections.Generic.KeyValuePair<string, int>("**", 139),
                            new System.Collections.Generic.KeyValuePair<string, int>("%%", 141),
                            new System.Collections.Generic.KeyValuePair<string, int>("++", 143),
                            new System.Collections.Generic.KeyValuePair<string, int>("<<", 146),
                            new System.Collections.Generic.KeyValuePair<string, int>(">>", 150),
                            new System.Collections.Generic.KeyValuePair<string, int>("&&", 156),
                            new System.Collections.Generic.KeyValuePair<string, int>("^^", 159),
                            new System.Collections.Generic.KeyValuePair<string, int>("||", 161),
                            new System.Collections.Generic.KeyValuePair<string, int>(";;", 164),
                            new System.Collections.Generic.KeyValuePair<string, int>("((", 165),
                            new System.Collections.Generic.KeyValuePair<string, int>("))", 166),
                            new System.Collections.Generic.KeyValuePair<string, int>("[[", 167),
                            new System.Collections.Generic.KeyValuePair<string, int>("]]", 168),
                            new System.Collections.Generic.KeyValuePair<string, int>(",,", 169),
                            new System.Collections.Generic.KeyValuePair<string, int>("{{", 170),
                            new System.Collections.Generic.KeyValuePair<string, int>("}}", 171),
                            new System.Collections.Generic.KeyValuePair<string, int>("::", 172),
                            new System.Collections.Generic.KeyValuePair<string, int>("..", 173),
                            new System.Collections.Generic.KeyValuePair<string, int>("~~", 174),
                            new System.Collections.Generic.KeyValuePair<string, int>("!!", 175),
                            new System.Collections.Generic.KeyValuePair<string, int>("??", 178)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aqsz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("rr", 4)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 5)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__bz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("aa", 6)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__ajlz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("kk", 7)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__bnpz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("aa", 9),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 13)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__asuz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 10)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__abdz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("cc", 11)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__agiz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("hh", 12)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__amoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 14)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__asuz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 15)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__ahjz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 16)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__amoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 17)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__atvz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("uu", 18)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 19)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfnpz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 21),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 26)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 22)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 23)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__asuz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 24)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 25)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 28)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__artz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ss", 29)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 30)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__bhjnptvz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("aa", 32),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 36),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 42),
                            new System.Collections.Generic.KeyValuePair<string, int>("uu", 44)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 33)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__artz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ss", 34)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 35)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__amoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 37)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__bz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("aa", 38)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 39)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 40)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__axzz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("yy", 41)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aqsz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("rr", 43)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__amoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 45)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__abdz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("cc", 46)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__asuz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 47)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__ahjz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 48)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__anpz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 49)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__amoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 50)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aegmoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ff", 52),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 53)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__artz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ss", 54)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__asuz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 55)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__bz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("aa", 56)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__amoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 57)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__abdz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("cc", 58)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 59)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__anpz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 60)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aegz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ff", 61)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adftvz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 63),
                            new System.Collections.Generic.KeyValuePair<string, int>("uu", 65)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__avxz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ww", 64)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 66)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 67)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 69)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__asuz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 70)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__atvz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("uu", 71)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aqsz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("rr", 72)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__amoz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("nn", 73)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__agiqsxzz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("hh", 75),
                            new System.Collections.Generic.KeyValuePair<string, int>("rr", 81),
                            new System.Collections.Generic.KeyValuePair<string, int>("yy", 85)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__ahjqsz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 76),
                            new System.Collections.Generic.KeyValuePair<string, int>("rr", 78)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__artz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ss", 77)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__anpz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 79)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__avxz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ww", 80)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__atvxzz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("uu", 82),
                            new System.Collections.Generic.KeyValuePair<string, int>("yy", 84)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 83)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aoqz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("pp", 86)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 87)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__anpz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 88)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aegz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ff", 89)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__bnpz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("aa", 91),
                            new System.Collections.Generic.KeyValuePair<string, int>("oo", 93)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__aqsz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("rr", 92)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__ahjz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 94)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__acez", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("dd", 95)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__agjz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("hh", 97),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 101)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__ahjz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ii", 98)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__akmz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ll", 99)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__adfz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("ee", 100)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__asuz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("tt", 102)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__agiz", 2),
                            new System.Collections.Generic.KeyValuePair<string, int>("hh", 103)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(56, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AZ__az", 2)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0!#[]￿", 105),
                            new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 106),
                            new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 108)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0!#[]￿", 105),
                            new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 106),
                            new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 108)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0￿", 107)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0!#[]￿", 105),
                            new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 106),
                            new System.Collections.Generic.KeyValuePair<string, int>("\"\"", 108)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(57, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0&([]￿", 110),
                            new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 111),
                            new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 113)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0&([]￿", 110),
                            new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 111),
                            new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 113)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0￿", 112)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0&([]￿", 110),
                            new System.Collections.Generic.KeyValuePair<string, int>("\\\\", 111),
                            new System.Collections.Generic.KeyValuePair<string, int>("\'\'", 113)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(57, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(59, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("xx", 115),
                            new System.Collections.Generic.KeyValuePair<string, int>("..", 117),
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 119),
                            new System.Collections.Generic.KeyValuePair<string, int>("EEee", 120)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AFaf", 116)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(58, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09AFaf", 116)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 118)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(59, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 119),
                            new System.Collections.Generic.KeyValuePair<string, int>("EEee", 120)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(59, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 119),
                            new System.Collections.Generic.KeyValuePair<string, int>("EEee", 120)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("++--", 121),
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 122)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 122)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(59, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 122)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(81, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("00", 124),
                            new System.Collections.Generic.KeyValuePair<string, int>("19", 125),
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 127),
                            new System.Collections.Generic.KeyValuePair<string, int>("--", 128)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(59, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("..", 117),
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 119),
                            new System.Collections.Generic.KeyValuePair<string, int>("EEee", 120)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(59, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 126),
                            new System.Collections.Generic.KeyValuePair<string, int>("..", 117),
                            new System.Collections.Generic.KeyValuePair<string, int>("EEee", 120)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(59, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("09", 126),
                            new System.Collections.Generic.KeyValuePair<string, int>("..", 117),
                            new System.Collections.Generic.KeyValuePair<string, int>("EEee", 120)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(76, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(122, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\t\t\n\n\r  ", 129)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(85, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("**", 131),
                            new System.Collections.Generic.KeyValuePair<string, int>("//", 132),
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 135)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(123, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0\t￿", 133),
                            new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 134)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(-1, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("\0\t￿", 133),
                            new System.Collections.Generic.KeyValuePair<string, int>("\n\n", 134)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(124, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 137)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(95, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 138)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(97, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(84, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 140)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(86, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 142)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(80, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 144),
                            new System.Collections.Generic.KeyValuePair<string, int>("++", 145)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(75, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(90, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("<<", 147),
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 149)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(87, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 148)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(92, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(91, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>(">>", 151),
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 155)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(88, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 152),
                            new System.Collections.Generic.KeyValuePair<string, int>(">>", 153)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(89, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 154)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(93, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(99, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 157),
                            new System.Collections.Generic.KeyValuePair<string, int>("&&", 158)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(102, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(100, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 160)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(101, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 162),
                            new System.Collections.Generic.KeyValuePair<string, int>("||", 163)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(62, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(103, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(63, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(65, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(66, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(67, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(68, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(69, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(70, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(71, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(72, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(73, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(82, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(83, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 176)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(96, new System.Collections.Generic.KeyValuePair<string, int>[] {
                            new System.Collections.Generic.KeyValuePair<string, int>("==", 177)}),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(98, new System.Collections.Generic.KeyValuePair<string, int>[0]),
                new System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<string, int>[]>(104, new System.Collections.Generic.KeyValuePair<string, int>[0])};
        public JSTokenizer(System.Collections.Generic.IEnumerable<char> input) : 
                base(JSTokenizer._DfaTable, JSTokenizer._Symbols, JSTokenizer._BlockEnds, input) {
        }
    }
}
