using System.Linq;
using Irony.Parsing;
using Visor.LanguageService.ReservedWords;

// ReSharper disable InconsistentNaming

namespace Visor.LanguageService
{
    [Language("RepGen", "1.0", "Repgen Programming Language")]
    public class Grammar : Irony.Parsing.Grammar
    {
        public Grammar() : base(false) // insenstive case
        {
            #region Declare Terminals Here

            var blockComment = new CommentTerminal("block-comment", "[", "]");
            NonGrammarTerminals.Add(blockComment);

            var number = new NumberLiteral("number", NumberOptions.IntOnly);
            var money = new NumberLiteral("money", NumberOptions.AllowSign);
            var rate = new NumberLiteral("rate");
            var date = new StringLiteral("date", "'");
            var character = new StringLiteral("character", "\"");

            var identifier = new IdentifierTerminal("identifier", "@", "@#");

            #endregion

            #region Declare NonTerminals Here

            var includeStatement = new NonTerminal("include");
            var program = new NonTerminal("program");
            var declarations = new NonTerminal("declaration");
            var declaration = new NonTerminal("declaration");
            var literal = new NonTerminal("literal");
            var assignments = new NonTerminal("assignments");
            var arrayParen = new NonTerminal("array-paren");
            var arraySize = new NonTerminal("array-size");
            var arrayIndex = new NonTerminal("array-index");
            var parenParameters = new NonTerminal("paren-parameters");
            var parameters = new NonTerminal("parameters");
            var variableType = new NonTerminal("variable-type");
            var block = new NonTerminal("block");
            var statements = new NonTerminal("statements");
            var statement = new NonTerminal("statement");
            var parenExpression = new NonTerminal("paren-expression");
            var forHeader = new NonTerminal("for-header");
            var forRecordHeader = new NonTerminal("for-record");
            var forRecordWithHeader = new NonTerminal("for-record-with");
            var forEachHeader = new NonTerminal("foreach-header");
            var semiStatement = new NonTerminal("semi-statement");
            var arguments = new NonTerminal("arguments");
            var parenArguments = new NonTerminal("paren-arguments");
            var assignExpression = new NonTerminal("assign-expression");
            var expression = new NonTerminal("expression");
            var booleanOperator = new NonTerminal("boolean-operator");
            var relationalExpression = new NonTerminal("relational-expression");
            var relationalOperator = new NonTerminal("relational-operator");
            var addExpression = new NonTerminal("add-expression");
            var addOperator = new NonTerminal("add-operator");
            var multiplyExpression = new NonTerminal("multiply");
            var multiplyOperator = new NonTerminal("multiply-operator");
            var prefixExpression = new NonTerminal("prefix-expression");
            var prefixOperator = new NonTerminal("prefix-operator");
            var factor = new NonTerminal("factor");

            var procedureDefinitions = new NonTerminal("procedure-list");
            var procedure = new NonTerminal("procedure");
            var procedureHeader = new NonTerminal("procedure-header");
            var procedureCall = new NonTerminal("procedure-call");
            var printStatement = new NonTerminal("print-statement");
            var printAlign = new NonTerminal("print-align");

            var programType = new NonTerminal("program-type");
            var programTypeWords = new NonTerminal("program-typedef");
            var targetStatement = new NonTerminal("target");
            var defineSection = new NonTerminal("define-section");
            var setupSection = new NonTerminal("setup-section");
            var selectSection = new NonTerminal("select-section");
            var sortSection = new NonTerminal("sort-section");
            var factorList = new NonTerminal("factor-list");
            var printSection = new NonTerminal("print-section");
            var totalSection = new NonTerminal("total-section");
            var printSectionHeader = new NonTerminal("print-header");
            var headerSection = new NonTerminal("header-section");

            var databaseField = new NonTerminal("database-field");
            var databaseRecord = new NonTerminal("database-record");
            var fieldName = new NonTerminal("field-name");
            var recordWithStatement = new NonTerminal("record-with");

            var functionCall = new NonTerminal("function-call");
            var functionName = new NonTerminal("function-name");

            #endregion

            #region Place Rules Here

            Root = program;

            program.Rule
                = programTypeWords + targetStatement + defineSection + setupSection + (selectSection | Empty) +
                  (sortSection | Empty) + printSection + procedureDefinitions
                  | includeStatement;

            includeStatement.Rule = ToTerm("#include") + character;

            programTypeWords.Rule
                = programTypeWords + programType
                  | programType
                  | Empty;

            programType.Rule
                = ToTerm("windows")
                  | ToTerm("windowsprint")
                  | ToTerm("customforms")
                  | ToTerm("customformswindows")
                  | ToTerm("audio")
                  | ToTerm("cardcreationwizard")
                  | ToTerm("certificate")
                  | ToTerm("demand")
                  | ToTerm("subroutine")
                  | ToTerm("symconnect")
                  | ToTerm("mcwinteractive")
                  | ToTerm("validation")
                  | ToTerm("mcw");

            targetStatement.Rule = ToTerm("target") + "=" + databaseRecord;

            defineSection.Rule
                = ToTerm("define") + ToTerm("end")
                  | ToTerm("define") + declarations + ToTerm("end");

            setupSection.Rule
                = ToTerm("setup") + ToTerm("end")
                  | ToTerm("setup") + statements + ToTerm("end");

            selectSection.Rule
                = ToTerm("select") + (Empty | expression | ToTerm("all") | ToTerm("none")) + ToTerm("end");

            sortSection.Rule
                = ToTerm("sort") + ToTerm("end")
                  | ToTerm("sort") + factorList + ToTerm("end");

            printSection.Rule
                = printSectionHeader + ToTerm("end")
                  | printSectionHeader + statements + ToTerm("end");

            totalSection.Rule
                = ToTerm("total") + ToTerm("end")
                  | ToTerm("total") + statements + ToTerm("end");

            headerSection.Rule
                = ToTerm("headers") + ToTerm("end")
                  | ToTerm("headers") + statements + ToTerm("end");

            printSectionHeader.Rule = ToTerm("print") + "title" + "=" + character;

            factorList.Rule
                = factorList + factor
                  | factor;

            procedureDefinitions.Rule
                = procedureDefinitions + procedure
                  | procedure;

            procedure.Rule
                = procedureHeader + statements + ToTerm("end")
                  | includeStatement;

            procedureHeader.Rule = ToTerm("procedure") + identifier;
            procedureCall.Rule = ToTerm("call") + identifier;

            declarations.Rule
                = declarations + declaration
                  | declaration;

            declaration.Rule
                = identifier + ToTerm("=") + variableType + ToTerm("array") + arrayParen
                  | identifier + ToTerm("=") + variableType
                  | identifier + ToTerm("=") + factor // constant
                  | includeStatement;

            assignments.Rule
                = assignments + assignExpression
                  | assignExpression;

            literal.Rule
                = number
                  | character
                  | date
                  | rate
                  | rate + "%"
                  | money;

            arrayParen.Rule
                = ToTerm("(") + arraySize + ")";

            arraySize.Rule
                = arraySize + "," + number
                  | number;

            arrayIndex.Rule
                = ToTerm("(") + parameters + ")";

            parameters.Rule
                = parameters + "," + factor
                  | factor;

            parenParameters.Rule
                = ToTerm("(") + ")"
                  | "(" + parameters + ")";

            variableType.Rule
                = ToTerm("number")
                  | "date"
                  | "rate"
                  | "float"
                  | "character"
                  | "money";

            block.Rule
                = ToTerm("do") + "end"
                  | "do" + statements + "end"
                  | statement;

            statements.Rule
                = statements + statement
                  | statement;

            statement.Rule
                = semiStatement
                  | ToTerm("while") + expression + block
                  | ToTerm("for") + ToTerm("each") + forEachHeader + block
                  | ToTerm("for") + forHeader + block
                  | ToTerm("for") + forRecordHeader + block
                  | ToTerm("for") + forRecordWithHeader + block
                  | ToTerm("if") + expression + ToTerm("then") + block
                  | ToTerm("else") + block;

            parenExpression.Rule = ToTerm("(") + expression + ")";

            forHeader.Rule
                = identifier + "=" + factor + "to" + factor + (("by" + factor) | Empty);

            forEachHeader.Rule = databaseRecord + "with" + expression;

            forRecordHeader.Rule = databaseRecord + factor;
            forRecordWithHeader.Rule = databaseRecord + "with" + identifier + factor;

            semiStatement.Rule
                = assignExpression
                  | includeStatement
                  | functionCall
                  | procedureCall
                  | printStatement
                  | headerSection;

            printStatement.Rule
                = ToTerm("print") + addExpression
                  | ToTerm("col") + "=" + factor + printAlign + addExpression
                  | ToTerm("newline")
                  | ToTerm("newpage");

            printAlign.Rule
                = ToTerm("left")
                  | ToTerm("right")
                  | Empty;

            arguments.Rule
                = expression + "," + arguments
                  | expression;

            parenArguments.Rule
                = ToTerm("(") + ")"
                  | "(" + arguments + ")";

            assignExpression.Rule
                = identifier + (Empty | arrayIndex) + ToTerm("=") + expression;

            expression.Rule
                = relationalExpression + booleanOperator + expression
                  | relationalExpression
                  | prefixOperator + forEachHeader;

            prefixOperator.Rule = ToTerm("not any") | "any";

            booleanOperator.Rule
                = ToTerm("and")
                  | "or";

            relationalExpression.Rule
                = addExpression + relationalOperator + addExpression
                  | addExpression;

            relationalOperator.Rule
                = ToTerm(">")
                  | ">="
                  | "<"
                  | "<="
                  | "="
                  | "<>";

            addExpression.Rule
                = multiplyExpression + addOperator + addExpression
                  | multiplyExpression;

            addOperator.Rule
                = ToTerm("+") | "-";

            multiplyExpression.Rule
                = factor + multiplyOperator + multiplyExpression
                  | factor;

            multiplyOperator.Rule
                = ToTerm("*")
                  | "/";

            factor.Rule
                = databaseField
                  | identifier + (Empty | arrayIndex)
                  | literal
                  | parenExpression
                  | functionCall;

            databaseField.Rule = databaseRecord + ":" + fieldName;

            databaseRecord.Rule
                = ToTerm("account")
                  | "name"
                  | "tracking"
                  | "share"
                  | "loan"
                  | "card";

            fieldName.Rule
                = ToTerm("id")
                  | "number"
                  | "closedate"
                  | "status"
                  | "type"
                  | "longname";

            functionCall.Rule = functionName + parenArguments;

            functionName.Rule = ToTerm(RepgenFunctions.List[0].Name);
            foreach (string name in RepgenFunctions.List.Select(x => x.Name))
            {
                functionName.Rule |= ToTerm(name);
            }

            #endregion

            #region Define Keywords

            MarkReservedWords(RepgenKeywords.List.ToArray());

            //var color = (TokenColor)Configuration.CreateColor("Repgen.DatabaseRecord", COLORINDEX.CI_USERTEXT_FG, COLORINDEX.CI_USERTEXT_BK);
            //var records = SymitarDatabase.Records.Select(record => DatabaseRecord(record.Name, color)).ToList();
            //color = (TokenColor)Configuration.CreateColor("Repgen.DatabaseField", COLORINDEX.CI_USERTEXT_FG, COLORINDEX.CI_USERTEXT_BK);
            //var fields = (from pair in SymitarDatabase.Fields from field in pair.Value select DatabaseField(field.Name, color)).ToList();

            #endregion

            MarkPunctuation(",", ":");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");

            foreach (
                string brace in
                    new[] {"do", "select", "define", "print", "setup", "sort", "total", "headers", "procedure"})
            {
                RegisterBracePair(brace, "end");
            }
        }

        public KeyTerm DatabaseRecord(string recordName, TokenColor color)
        {
            KeyTerm term = ToTerm(recordName);
            MarkReservedWords(recordName);
            term.EditorInfo = new TokenEditorInfo(TokenType.Identifier, color, TokenTriggers.None);
            return term;
        }

        public KeyTerm DatabaseField(string field, TokenColor color)
        {
            KeyTerm term = ToTerm(field);
            MarkReservedWords(field);
            term.EditorInfo = new TokenEditorInfo(TokenType.Identifier, color, TokenTriggers.None);
            return term;
        }
    }
}

// ReSharper restore InconsistentNaming