# EJ2 Document editor Services

## Available Web API services in EJ2 Document Editor
* Import
* SpellCheck
* SystemClipboard
* RestrictEditing

## Import
In order to import word documents into document editor you can make use of this service call. Also you can convert word documents (.dotx,.docx,.docm,.dot,.doc), rich text format documents (.rtf), and text documents (.txt) into SFDT format by using this Web API service implementation.

## SystemClipboard
You can make use of this service in order to paste system clipboard data by preserving the formatting.

## Spell Check

Document editor performs spell check by processing hunspell dictionary files, So kindly follow below steps to include the necessary files to perform spell check.

### Where to find the dictionaries?
[Hunspell Dictionaries](https://github.com/wooorm/dictionaries) - Dictionary location

### Steps to configure spell checker

* Add the [`Syncfusion.EJ2.SpellChecker.AspNet.Mvc5`](<https://www.nuget.org/packages/Syncfusion.EJ2.SpellChecker.AspNet.Mvc5/>) Nuget in your project 

* In the application App_Data folder, include the dictionary, .aff dictionary files and JSON file. 

![](appData.PNG)

JSON file should contains the values in the following format.

```json
[
  {
    "LanguadeID": 1036, 
    "DictionaryPath": "fr_FR.dic",
    "AffixPath": "fr_FR.aff", 
    "PersonalDictPath": "customDict.dic"
  },
  {
    "LanguadeID": 1033,
    "DictionaryPath": "en_US.dic",
    "AffixPath": "en_US.aff",
    "PersonalDictPath": "customDict.dic"
  }
]
```

* For handling personal dictionary, place empty .dic file (ex. customDict.dic file) in the App_Data folder.

* Refer the added files in the spell checker service call as well where we will pass the file information
 ![](codeFile.png)

### How it works

* Spell checking will be performed based on the below information from client side and it will be passed to **GetSuggestions** API to process spell checker.

### LanguageID

* As mentioned document editor supports multi-language spell check. You can add as many languages (dictionaries) using **DictionaryData** class with unique ID for each language. Spell checking will be initiated only when languageID value passed in client side present in the **DictionaryData** collection. 

![](multiLang.PNG)

### TexttoCheck

* The text to be processed for spell checking.

### CheckSpelling
* Indicates spell checking need to be performed for the given text.

### CheckSuggestion
* Indicates whether to provide suggestions for the mis-spelled words.

### AddWord
* Indicates whether the text need to added in personal dictionary.

## RestrictEditing
Document Editor provides support for restrict editing. You can make use of this Web API service to encrypt/decrypt protected content. 