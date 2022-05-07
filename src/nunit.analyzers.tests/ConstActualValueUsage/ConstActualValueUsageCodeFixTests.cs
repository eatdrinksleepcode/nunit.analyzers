using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Analyzers.ConstActualValueUsage;
using NUnit.Analyzers.Constants;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.Analyzers.Tests.ConstActualValueUsage
{
    public class ConstActualValueUsageCodeFixTests
    {
        private static readonly DiagnosticAnalyzer analyzer = new ConstActualValueUsageAnalyzer();
        private static readonly CodeFixProvider fix = new ConstActualValueUsageCodeFix();
        private static readonly ExpectedDiagnostic expectedDiagnostic =
            ExpectedDiagnostic.Create(AnalyzerIdentifiers.ConstActualValueUsage);

        [TestCase(nameof(Assert.AreEqual))]
        [TestCase(nameof(Assert.AreNotEqual))]
        [TestCase(nameof(Assert.AreSame))]
        [TestCase(nameof(Assert.AreNotSame))]
        public void LiteralArgumentIsProvidedForClassicAssertCodeFix(string classicAssertMethod)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    int expected = 5;
                    Assert.{classicAssertMethod}(expected, ↓1);
                }}");

            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    int expected = 5;
                    Assert.{classicAssertMethod}(1, expected);
                }}");

            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode,
                fixTitle: ConstActualValueUsageCodeFix.SwapArgumentsDescription);
        }

        [Test]
        public void LiteralNamedArgumentIsProvidedForAreEqualCodeFix()
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
                public void Test()
                {
                    int expected = 5;
                    Assert.AreEqual(actual: ↓1, expected: expected);
                }");

            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
                public void Test()
                {
                    int expected = 5;
                    Assert.AreEqual(actual: expected, expected: 1);
                }");

            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode,
                fixTitle: ConstActualValueUsageCodeFix.SwapArgumentsDescription);
        }

        [TestCase(nameof(Is.EqualTo))]
        [TestCase(nameof(Is.SameAs))]
        [TestCase(nameof(Is.SamePath))]
        [TestCase("Not.EqualTo")]
        [TestCase("Not.SameAs")]
        [TestCase("Not.SamePath")]
        public void LiteralArgumentIsProvidedForAssertThatCodeFix(string isConstraint)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    var expected = ""abc"";
                    Assert.That(↓""a"", Is.{isConstraint}(expected));
                }}");

            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    var expected = ""abc"";
                    Assert.That(expected, Is.{isConstraint}(""a""));
                }}");

            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode,
                fixTitle: ConstActualValueUsageCodeFix.SwapArgumentsDescription);
        }

        [TestCase(nameof(Is.GreaterThan))]
        [TestCase(nameof(Is.LessThan))]
        public void NoCodeFixForNonSymmetricAssertThat(string isConstraint)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    var expected = 5;
                    Assert.That(↓4, Is.{isConstraint}(expected));
                }}");

            RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code);
        }

        [Test]
        public void NoCodeFixForCombinedAssertThat()
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    var expected = 5;
                    Assert.That(↓4, Is.Not.EqualTo(3).And.Not.EqualTo(expected));
                }}");

            RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code);
        }

        [TestCase(nameof(StringAssert.AreEqualIgnoringCase))]
        [TestCase(nameof(StringAssert.AreNotEqualIgnoringCase))]
        [TestCase(nameof(StringAssert.Contains))]
        [TestCase(nameof(StringAssert.EndsWith))]
        [TestCase(nameof(StringAssert.IsMatch))]
        [TestCase(nameof(StringAssert.StartsWith))]
        [TestCase(nameof(StringAssert.DoesNotContain))]
        [TestCase(nameof(StringAssert.DoesNotMatch))]
        [TestCase(nameof(StringAssert.DoesNotEndWith))]
        [TestCase(nameof(StringAssert.DoesNotStartWith))]
        public void LiteralArgumentIsProvidedForClassicStringAssertCodeFix(string classicAssertMethod)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    string actual = ""act"";
                    StringAssert.{classicAssertMethod}(actual, ↓""exp"");
                }}");

            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    string actual = ""act"";
                    StringAssert.{classicAssertMethod}(""exp"", actual);
                }}");

            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode,
                fixTitle: ConstActualValueUsageCodeFix.SwapArgumentsDescription);
        }

        [Test]
        public void LiteralNamedArgumentIsProvidedForStringAssertContainsCodeFix()
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
                public void Test()
                {
                    string actual = ""act"";
                    StringAssert.Contains(actual: ↓""exp"", expected: actual);
                }");

            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
                public void Test()
                {
                    string actual = ""act"";
                    StringAssert.Contains(actual: actual, expected: ""exp"");
                }");

            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode,
                fixTitle: ConstActualValueUsageCodeFix.SwapArgumentsDescription);
        }

        [TestCase(nameof(CollectionAssert.AreEqual))]
        [TestCase(nameof(CollectionAssert.AreEquivalent))]
        [TestCase(nameof(CollectionAssert.AreNotEqual))]
        [TestCase(nameof(CollectionAssert.AreNotEquivalent))]
        public void LiteralArgumentIsProvidedForClassicCollectionAssertCodeFix(string classicAssertMethod)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    string[] actual = new [] {{ ""act"" }};
                    CollectionAssert.{classicAssertMethod}(actual, ↓new [] {{ ""exp"" }});
                }}");

            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
                public void Test()
                {{
                    string actual = ""act"";
                    CollectionAssert.{classicAssertMethod}(↓new [] {{ ""exp"" }}, actual);
                }}");

            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode,
                fixTitle: ConstActualValueUsageCodeFix.SwapArgumentsDescription);
        }

        [Test]
        public void LiteralNamedArgumentIsProvidedForCollectionAssertContainsCodeFix()
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
                public void Test()
                {
                    string[] actual = new[] { ""act"" };
                    CollectionAssert.Contains(actual: ↓""exp"", expected: actual);
                }");

            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
                public void Test()
                {
                    string actual = ""act"";
                    CollectionAssert.Contains(actual: actual, expected: ""exp"");
                }");

            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode,
                fixTitle: ConstActualValueUsageCodeFix.SwapArgumentsDescription);
        }
    }
}
