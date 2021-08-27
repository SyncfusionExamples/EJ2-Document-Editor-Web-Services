package ej2.webservices;

import org.springframework.web.bind.annotation.RestController;

import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.multipart.MultipartFile;
import com.syncfusion.ej2.wordprocessor.WordProcessorHelper;
import com.google.gson.Gson;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.syncfusion.ej2.spellchecker.DictionaryData;
import com.syncfusion.ej2.spellchecker.SpellChecker;
import com.syncfusion.ej2.wordprocessor.FormatType;

@RestController
public class WordEditorController {

        List<DictionaryData> spellDictionary;
        String personalDictPath;
	int count;
	public WordEditorController() throws Exception {
		 
	String jsonFilePath = "src/main/resources/spellcheck.json";
	count = 0;
	String jsonContent = new String(Files.readAllBytes(Paths.get(jsonFilePath)), StandardCharsets.UTF_8);
	JsonArray spellDictionaryItems = new Gson().fromJson(jsonContent, JsonArray.class);
	personalDictPath = "src/main/resources/customDict.dic";
	spellDictionary = new ArrayList<DictionaryData>();
	for(int i = 0; i < spellDictionaryItems.size(); i++) {
	    JsonObject spellCheckerInfo = spellDictionaryItems.get(i).getAsJsonObject();
	    DictionaryData dict = new DictionaryData();
	    
	    if(spellCheckerInfo.has("LanguadeID")) 
	    	dict.setLanguadeID(spellCheckerInfo.get("LanguadeID").getAsInt());
	    if(spellCheckerInfo.has("DictionaryPath")) 
	    	dict.setDictionaryPath("src/main/resources/"+spellCheckerInfo.get("DictionaryPath").getAsString());
	    if(spellCheckerInfo.has("AffixPath")) 
	    	dict.setAffixPath("src/main/resources/"+spellCheckerInfo.get("AffixPath").getAsString());
	    spellDictionary.add(dict);
	}
	
	}

	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@GetMapping("/api/wordeditor/test")
	public String test() {
		System.out.println("==== in test ====");
		return "{\"sections\":[{\"blocks\":[{\"inlines\":[{\"text\":\"Hello World\"}]}]}]}";
	}
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/Import")
	public String uploadFile(@RequestParam("files") MultipartFile file) throws Exception {
		try {
			return WordProcessorHelper.load(file.getInputStream(), FormatType.Docx);
		} catch (Exception e) {
			e.printStackTrace();
			return "{\"sections\":[{\"blocks\":[{\"inlines\":[{\"text\":" + e.getMessage() + "}]}]}]}";
		}
	}
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/SpellCheck")
	public String spellCheck(@RequestBody SpellCheckJsonData spellChecker) throws Exception {
		try {
			  SpellChecker spellCheck = new SpellChecker(spellDictionary,personalDictPath);
               String data = spellCheck.getSuggestions(spellChecker.languageID, spellChecker.texttoCheck, spellChecker.checkSpelling, spellChecker.checkSuggestion, spellChecker.addWord);
              return data;
		} catch (Exception e) {
			e.printStackTrace();
			return "{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}";
		}
	}
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/SpellCheckByPage")
	public String spellCheckByPage(@RequestBody SpellCheckJsonData spellChecker) throws Exception {
		try {
			  SpellChecker spellCheck = new SpellChecker(spellDictionary,personalDictPath);
               String data = spellCheck.checkSpelling(spellChecker.languageID, spellChecker.texttoCheck);
              return data;
		} catch (Exception e) {
			e.printStackTrace();
			return "{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}";
		}
	}


	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/RestrictEditing")
	public String[] restrictEditing(@RequestBody CustomRestrictParameter param) throws Exception {
		if (param.passwordBase64 == "" && param.passwordBase64 == null)
			return null;
		return WordProcessorHelper.computeHash(param.passwordBase64, param.saltBase64, param.spinCount);
	}

	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/SystemClipboard")
	public String systemClipboard(@RequestBody CustomParameter param) {
		if (param.content != null && param.content != "") {
			try {
				return  WordProcessorHelper.loadString(param.content, GetFormatType(param.type.toLowerCase()));
			} catch (Exception e) {
				return "";
			}
		}
		return "";
	}
	
	static FormatType GetFormatType(String format)
    {
        switch (format)
        {
            case ".dotx":
            case ".docx":
            case ".docm":
            case ".dotm":
                return FormatType.Docx;
            case ".dot":
            case ".doc":
                return FormatType.Doc;
            case ".rtf":
                return FormatType.Rtf;
            case ".txt":
                return FormatType.Txt;
            case ".xml":
                return FormatType.WordML;
            case ".html":
                return FormatType.Html;
            default:
                return FormatType.Docx;
        }
    }
}
