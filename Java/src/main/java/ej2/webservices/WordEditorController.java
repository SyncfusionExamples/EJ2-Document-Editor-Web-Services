package ej2.webservices;

import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.multipart.MultipartFile;
import com.syncfusion.ej2.wordprocessor.WordProcessorHelper;
import com.syncfusion.ej2.wordprocessor.FormatType;

@RestController
public class WordEditorController {

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
