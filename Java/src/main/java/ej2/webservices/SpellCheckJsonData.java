package ej2.webservices;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SpellCheckJsonData
{
    @JsonProperty("LanguageID")
    int languageID;

    @JsonProperty("TexttoCheck")
    String texttoCheck;

    @JsonProperty("CheckSpelling")
    boolean checkSpelling;

    @JsonProperty("CheckSuggestion")
    boolean checkSuggestion;

    @JsonProperty("AddWord")
    boolean addWord;

    @JsonProperty("IgnoreUppercase")
    boolean ignoreUppercase;

    @JsonProperty("UserName")
    String userName;

    public int getLanguageID() { return languageID; }
    public void setLanguageID(int languageID) { this.languageID = languageID; }

    public String getTexttoCheck() { return texttoCheck; }
    public void setTexttoCheck(String texttoCheck) { this.texttoCheck = texttoCheck; }

    public boolean isCheckSpelling() { return checkSpelling; }
    public void setCheckSpelling(boolean checkSpelling) { this.checkSpelling = checkSpelling; }

    public boolean isCheckSuggestion() { return checkSuggestion; }
    public void setCheckSuggestion(boolean checkSuggestion) { this.checkSuggestion = checkSuggestion; }

    public boolean isAddWord() { return addWord; }
    public void setAddWord(boolean addWord) { this.addWord = addWord; }

    public boolean isIgnoreUppercase() { return ignoreUppercase; }
    public void setIgnoreUppercase(boolean ignoreUppercase) { this.ignoreUppercase = ignoreUppercase; }

    public String getUserName() { return userName; }
    public void setUserName(String userName) { this.userName = userName; }
}
