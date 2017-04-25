using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace FileNameMustBeTheSameAsClassName
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FileNameMustBeTheSameAsClassNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FileNameMustBeTheSameAsClassName";

        private const string Title = "Nazwa pliku Ÿród³owego musi byæ to¿sama z nazw¹ zadeklarowanej w nim klasy.";
        private const string MessageFormat = "Nazwa pliku Ÿród³owego '{0}' musi byæ to¿sama z nazw¹ zadeklarowanej w nim klasy '{1}'.";
        private const string Description = "Nazwy plików Ÿród³owych musz¹ byæ to¿same z nazwami zadeklarowanych w nich klas.";
        private const string Category = "Design";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            string fileName = Path.GetFileName(context.Tree.FilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(context.Tree.FilePath);

            IEnumerable<BaseTypeDeclarationSyntax> types = context.Tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>();

            if (types != null && types.Count() == 1)
            {
                ClassDeclarationSyntax @class = types.First() as ClassDeclarationSyntax;

                if (@class != null)
                {
                    if (@class.Identifier.ValueText != fileNameWithoutExtension)
                    {
                        Diagnostic diagnostic = Diagnostic.Create(Rule, @class.Identifier.GetLocation(), fileName, @class.Identifier.ValueText);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}