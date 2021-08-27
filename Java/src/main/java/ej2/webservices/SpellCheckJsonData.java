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

}
