namespace DocumentEditorCore
{
    /// <summary>
    /// This Class contains the property for Restrict editing
    /// </summary>
    public class CustomRestrictParameter
    {
        public string? passwordBase64 { get; set; }
        public string? saltBase64 { get; set; }
        public int spinCount { get; set; }
    }
}
