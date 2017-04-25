using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace InterfaceMustBePublic
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceMustBePublicAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "InterfaceMustBePublic";

        private const string Title = "Deklaracja interfejsu musi zawierać publiczny modyfikator dostępu.";
        private const string MessageFormat = "Deklaracja interfejsu '{0}' musi zawierać publiczny modyfikator dostępu.";
        private const string Description = "Deklaracje interfejsów muszą zawierać publiczne modyfikatory dostępu.";
        private const string Category = "Design";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            if (namedTypeSymbol.TypeKind == TypeKind.Interface)
            {
                if (namedTypeSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    Diagnostic diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}