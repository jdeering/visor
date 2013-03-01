using System.Collections.Generic;
using System.Linq;
using Irony.Parsing;
using Irony;
using Visor.LanguageService.ReservedWords;
using Microsoft.VisualStudio.TextManager.Interop;

// ReSharper disable InconsistentNaming
namespace Visor.LanguageService
{
    [Language("RepGen", "1.0", "Repgen Programming Language")]
    public class Grammar : Irony.Parsing.Grammar
    {
        public Grammar() : base(false) // insenstive case
        {

            #region Declare Terminals Here
            CommentTerminal blockComment = new CommentTerminal("block-comment", "[", "]");
            NonGrammarTerminals.Add(blockComment);

            NumberLiteral number = new NumberLiteral("number", NumberOptions.IntOnly);
            NumberLiteral money = new NumberLiteral("money", NumberOptions.AllowSign);
            NumberLiteral rate = new NumberLiteral("rate");
            StringLiteral date = new StringLiteral("date", "'");
            StringLiteral character = new StringLiteral("character", "\"");

            IdentifierTerminal identifier = new IdentifierTerminal("identifier");
            #endregion

            #region Declare NonTerminals Here
            NonTerminal includeStatement = new NonTerminal("include");
            NonTerminal program = new NonTerminal("program");
            NonTerminal declarations = new NonTerminal("declaration");
            NonTerminal declaration = new NonTerminal("declaration");
            NonTerminal literal = new NonTerminal("literal");
            NonTerminal assignments = new NonTerminal("assignments");
            NonTerminal arrayParen = new NonTerminal("array-paren");
            NonTerminal arraySize = new NonTerminal("array-size");
            NonTerminal arrayIndex = new NonTerminal("array-index");
            NonTerminal parenParameters = new NonTerminal("paren-parameters");
            NonTerminal parameters = new NonTerminal("parameters");
            NonTerminal variableType = new NonTerminal("variable-type");
            NonTerminal block = new NonTerminal("block");
            NonTerminal statements = new NonTerminal("statements");
            NonTerminal statement = new NonTerminal("statement");
            NonTerminal parenExpression = new NonTerminal("paren-expression");
            NonTerminal forHeader = new NonTerminal("for-header");
            NonTerminal forEachHeader = new NonTerminal("foreach-header");
            NonTerminal semiStatement = new NonTerminal("semi-statement");
            NonTerminal arguments = new NonTerminal("arguments");
            NonTerminal parenArguments = new NonTerminal("paren-arguments");
            NonTerminal assignExpression = new NonTerminal("assign-expression");
            NonTerminal expression = new NonTerminal("expression");
            NonTerminal booleanOperator = new NonTerminal("boolean-operator");
            NonTerminal relationalExpression = new NonTerminal("relational-expression");
            NonTerminal relationalOperator = new NonTerminal("relational-operator");
            NonTerminal addExpression = new NonTerminal("add-expression");
            NonTerminal addOperator = new NonTerminal("add-operator");
            NonTerminal multiplyExpression = new NonTerminal("multiply");
            NonTerminal multiplyOperator = new NonTerminal("multiply-operator");
            NonTerminal prefixExpression = new NonTerminal("prefix-expression");
            NonTerminal prefixOperator = new NonTerminal("prefix-operator");
            NonTerminal factor = new NonTerminal("factor");

            NonTerminal procedureDefinitions = new NonTerminal("procedure-list");
            NonTerminal procedure = new NonTerminal("procedure");
            NonTerminal procedureHeader = new NonTerminal("procedure-header");
            NonTerminal procedureCall = new NonTerminal("procedure-call");
            NonTerminal printStatement = new NonTerminal("print-statement");

            NonTerminal programType = new NonTerminal("program-type");
            NonTerminal programTypeWords = new NonTerminal("program-typedef");
            NonTerminal targetStatement = new NonTerminal("target");
            NonTerminal defineSection = new NonTerminal("define-section");
            NonTerminal setupSection = new NonTerminal("setup-section");
            NonTerminal selectSection = new NonTerminal("select-section");
            NonTerminal sortSection = new NonTerminal("sort-section");
            NonTerminal factorList = new NonTerminal("factor-list");
            NonTerminal printSection = new NonTerminal("print-section");
            NonTerminal totalSection = new NonTerminal("total-section");
            NonTerminal printSectionHeader = new NonTerminal("print-header");
            NonTerminal headerSection = new NonTerminal("header-section");

            NonTerminal databaseField = new NonTerminal("database-field");
            NonTerminal databaseRecord = new NonTerminal("database-record");
            NonTerminal fieldName = new NonTerminal("field-name");
            NonTerminal recordWithStatement = new NonTerminal("record-with");

            NonTerminal functionCall = new NonTerminal("function-call");
            NonTerminal functionName = new NonTerminal("function-name");
            #endregion

            #region Place Rules Here
            this.Root = program;

            program.Rule 
                = programTypeWords + targetStatement + defineSection + setupSection + selectSection + printSection + procedureDefinitions
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
                | "character";

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
                | ToTerm("if") + expression + ToTerm("then") + block;

            parenExpression.Rule = ToTerm("(") + expression + ")";

            forHeader.Rule = identifier + "=" + factor + "to" + factor
                | identifier + "=" + factor + "to" + factor + "by" + factor;

            forEachHeader.Rule = databaseRecord + "with" + expression;

            semiStatement.Rule
                = assignExpression
                | includeStatement
                | functionCall
                | procedureCall
                | printStatement
                | headerSection;

            printStatement.Rule
                = ToTerm("print") + addExpression
                | ToTerm("col") + "=" + factor + addExpression
                | ToTerm("newline");

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
            foreach (var name in RepgenFunctions.List.Select(x => x.Name))
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

            foreach (var brace in new[] { "do", "select", "define", "print", "setup", "sort", "total", "headers", "procedure" })
            {
                RegisterBracePair(brace, "end");
            }
        }

        public KeyTerm DatabaseRecord(string recordName, TokenColor color)
        {
            var term = ToTerm(recordName);
            MarkReservedWords(recordName);
            term.EditorInfo = new TokenEditorInfo(TokenType.Identifier, color, TokenTriggers.None);
            return term;
        }

        public KeyTerm DatabaseField(string field, TokenColor color)
        {
            var term = ToTerm(field);
            MarkReservedWords(field);
            term.EditorInfo = new TokenEditorInfo(TokenType.Identifier, color, TokenTriggers.None);
            return term;
        }
    }
}

// ReSharper restore InconsistentNaming