using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Analyzers.Constants;
using NUnit.Analyzers.UseCollectionConstraint;
using NUnit.Framework;

namespace NUnit.Analyzers.Tests.UseCollectionConstraint
{
    [TestFixture]
    public sealed class UseCollectionConstraintAnalyzerTests
    {
        private readonly DiagnosticAnalyzer analyzer = new UseCollectionConstraintAnalyzer();
        private readonly ExpectedDiagnostic diagnostic = ExpectedDiagnostic.Create(AnalyzerIdentifiers.UsePropertyConstraint);

        [Test]
        public void AnalyzeWhenHasLengthIsUsed()
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void Test()
        {
            var array = new int[] { 1 };
            Assert.That(array, Has.Length.EqualTo(1));
        }");
            RoslynAssert.Valid(this.analyzer, testCode);
        }

        [Test]
        public void AnalyzeWhenHasCountIsUsed()
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void Test()
        {
            var list = new List<int>() { 1 };
            Assert.That(list, Has.Count.EqualTo(1));
        }", "using System.Collections.Generic;");
            RoslynAssert.Valid(this.analyzer, testCode);
        }

        [Test]
        public void AnalyzeWhenPropertyLengthIsUsed()
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void Test()
        {
            var array = new int[] { 1 };
            Assert.That(↓array.Length, Is.EqualTo(1));
        }");
            RoslynAssert.Diagnostics(this.analyzer, this.diagnostic, testCode);
        }

        [Test]
        public void AnalyzeWhenPropertyCountIsUsed()
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void Test()
        {
            var list = new List<int>() { 1 };
            Assert.That(↓list.Count, Is.Not.Zero);
        }", "using System.Collections.Generic;");
            RoslynAssert.Diagnostics(this.analyzer, this.diagnostic, testCode);
        }

        [Test]
        public void AnalyzeComplex()
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void Test()
        {
            var array = new int[] { 1 };
            Assert.That(↓array.Length, Is.GreaterThan(1).And.LessThan(9));
        }");
            RoslynAssert.Diagnostics(this.analyzer, this.diagnostic, testCode);
        }
    }
}
