using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileNameMustBeTheSameAsClassName
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileNameMustBeTheSameAsClassNameCodeFixProvider)), Shared]
    public class FileNameMustBeTheSameAsClassNameCodeFixProvider : CodeFixProvider
    {
        private const string title = "Zmień nazwę pliku źródłowego";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(FileNameMustBeTheSameAsClassNameAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return null;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            ClassDeclarationSyntax classDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: cancellationToken => ChangeDocumentNameAsync(context.Document, classDeclaration, cancellationToken),
                    equivalenceKey: title
                ),
                diagnostic
            );
        }

        private async Task<Solution> ChangeDocumentNameAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            DocumentId newDocumentId = DocumentId.CreateNewId(document.Project.Id);

            Solution newSolution = document.Project.Solution;

            newSolution = newSolution.AddDocument(
                documentId: newDocumentId, 
                name: classDeclaration.Identifier.ValueText,
                text: await document.GetTextAsync(),
                folders: document.Folders
            );

            newSolution = newSolution.RemoveDocument(
                documentId: document.Id
            );

            return newSolution;
        }
    }
}