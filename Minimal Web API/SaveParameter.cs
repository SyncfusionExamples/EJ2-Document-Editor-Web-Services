namespace DocumentEditorCore
{
    /// <summary>
    /// Represents the parameters required for saving a document in server.
    /// </summary>
    public class SaveParameter
    {
        public string? Content { get; set; }
        public string? FileName { get; set; }
    }
}
