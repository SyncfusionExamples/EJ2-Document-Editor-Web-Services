namespace DocumentEditorCore
{
    /// <summary>
    /// Represents the parameters required for generating the hash while protecting and unprotecting a Word document.
    /// </summary>
    public class CustomRestrictParameter
    {
        public string? passwordBase64 { get; set; }
        public string? saltBase64 { get; set; }
        public int spinCount { get; set; }
    }
}
