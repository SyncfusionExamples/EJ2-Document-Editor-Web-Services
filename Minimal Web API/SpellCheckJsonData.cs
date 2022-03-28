namespace DocumentEditorCore
{
    /// <summary>
    /// Specifies the property for Spell check functionality
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
