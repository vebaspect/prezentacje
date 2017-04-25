using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InterfaceMustBePublic
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceMustBePublicCodeFixProvider)), Shared]
    public class InterfaceMustBePublicCodeFixProvider : CodeFixProvider
    {
        private const string title = "Ustaw publiczny modyfikator dostępu";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(InterfaceMustBePublicAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            InterfaceDeclarationSyntax interfaceDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: cancellationToken => AddPublicAccessModifierAsync(context.Document, interfaceDeclaration, cancellationToken),
                    equivalenceKey: title
                ),
                diagnostic
            );
        }

        private async Task<Document> AddPublicAccessModifierAsync(Document document, InterfaceDeclarationSyntax interfaceDeclaration, CancellationToken cancellationToken)
        {
            SyntaxTokenList newModifiers = new SyntaxTokenList();
            newModifiers = newModifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            newModifiers = newModifiers.AddRange(interfaceDeclaration.Modifiers.Where(x =>
                !x.IsKind(SyntaxKind.ProtectedKeyword) &&
                !x.IsKind(SyntaxKind.InternalKeyword) &&
                !x.IsKind(SyntaxKind.PrivateKeyword))
            );

            InterfaceDeclarationSyntax newInterfaceDeclaration = interfaceDeclaration.WithModifiers(newModifiers);

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);
            SyntaxNode newRoot = root.ReplaceNode(interfaceDeclaration, newInterfaceDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}