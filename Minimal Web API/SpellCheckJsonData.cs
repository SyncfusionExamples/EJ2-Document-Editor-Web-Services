namespace DocumentEditorCore
{
    /// <summary>
    /// Represents the parameters required for spell check validation.
    /// </summary>
    public class SpellCheckJsonData
    {
        public int LanguageID { get; set; }
        public string? TexttoCheck { get; set; }
        public bool CheckSpelling { get; set; }
        public bool CheckSuggestion { get; set; }
        public bool AddWord { get; set; }

    }
}
