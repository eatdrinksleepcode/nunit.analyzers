using Microsoft.CodeAnalysis;

namespace NUnit.Analyzers.Constants
{
    public static class DescriptionExtensions
    {
        public static bool IsInstanceOf(this IMethodSymbol source, NUnitFrameworkConstants.MethodDescription methodDescription)
        {
            return source.ContainingType.IsInstanceOf(methodDescription.Parent) && source.Name == methodDescription.Name;
        }

        public static bool IsInstanceOf(this INamedTypeSymbol source, NUnitFrameworkConstants.TypeDescription typeDescription)
        {
            return source.Name == typeDescription.Name;
        }
    }
}
