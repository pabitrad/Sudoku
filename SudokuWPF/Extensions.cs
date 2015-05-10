namespace SudokuWPF
{
    internal static class Extensions
    {
        /// <summary>
        /// Ensures trailing back-slash at the end of specified string.
        /// </summary>
        public static string EnsureSlash(this string source)
        {
            string sourceString = source ?? string.Empty;
            return sourceString.TrimEnd('\\') + "\\";
        }
    }
}