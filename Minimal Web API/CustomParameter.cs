namespace DocumentEditorCore
{
    /// <summary>
    /// Represents the parameters required for converting clipboard data (HTML/RTF) into SFDT.
    /// </summary>
    public class CustomParameter
    {
        public string? content { get; set; }
        public string type { get; set; }
    }
}
