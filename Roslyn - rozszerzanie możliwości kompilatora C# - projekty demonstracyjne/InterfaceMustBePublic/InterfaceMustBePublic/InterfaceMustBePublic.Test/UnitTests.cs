using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace InterfaceMustBePublic.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestMethod2()
        {
            string source = @"
using System;

namespace Example
{
    interface Interface1
    {   
    }
}
";
            DiagnosticResult expected = new DiagnosticResult
            {
                Id = "InterfaceMustBePublic",
                Message = string.Format("Deklaracja interfejsu '{0}' musi zawierać publiczny modyfikator dostępu.", "Interface1"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test.cs", 6, 15)
                }
            };

            VerifyCSharpDiagnostic(source, expected);

            string newSource = @"
using System;

namespace Example
{
    public interface Interface1
    {   
    }
}
";
            VerifyCSharpFix(source, newSource);
        }

        [TestMethod]
        public void TestMethod3()
        {
            string source = @"
using System;

namespace Example
{
    private interface Interface1
    {   
    }
}
";
            DiagnosticResult expected = new DiagnosticResult
            {
                Id = "InterfaceMustBePublic",
                Message = string.Format("Deklaracja interfejsu '{0}' musi zawierać publiczny modyfikator dostępu.", "Interface1"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test.cs", 6, 23)
                }
            };

            VerifyCSharpDiagnostic(source, expected);

            string newSource = @"
using System;

namespace Example
{
    public interface Interface1
    {   
    }
}
";
            VerifyCSharpFix(source, newSource);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new InterfaceMustBePublicCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new InterfaceMustBePublicAnalyzer();
        }
    }
}