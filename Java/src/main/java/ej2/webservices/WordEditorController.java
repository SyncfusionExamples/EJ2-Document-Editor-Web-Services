package ej2.webservices;

import org.springframework.web.bind.annotation.RestController;

import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import javax.imageio.ImageIO;
import javax.imageio.ImageReader;
import javax.imageio.ImageWriter;
import javax.imageio.spi.IIORegistry;

import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.FileOutputStream;
import java.io.InputStream;

import org.springframework.core.io.ByteArrayResource;
import org.springframework.core.io.Resource;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.multipart.MultipartFile;
import com.syncfusion.ej2.wordprocessor.WordProcessorHelper;
import com.syncfusion.javahelper.system.collections.generic.ListSupport;
import com.syncfusion.javahelper.system.io.StreamSupport;
import com.syncfusion.javahelper.system.reflection.AssemblySupport;
import com.twelvemonkeys.imageio.plugins.tiff.TIFFImageReaderSpi;
import com.google.gson.Gson;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.syncfusion.ej2.spellchecker.DictionaryData;
import com.syncfusion.ej2.spellchecker.SpellChecker;
import com.syncfusion.docio.WordDocument;
import com.syncfusion.ej2.wordprocessor.FormatType;
import com.syncfusion.ej2.wordprocessor.MetafileImageParsedEventArgs;
import com.syncfusion.ej2.wordprocessor.MetafileImageParsedEventHandler;

@RestController
public class WordEditorController {

	public WordEditorController() throws Exception {
	String jsonFilePath = "src/main/resources/spellcheck.json";
	String jsonContent = new String(Files.readAllBytes(Paths.get(jsonFilePath)), StandardCharsets.UTF_8);
	JsonArray spellDictionaryItems = new Gson().fromJson(jsonContent, JsonArray.class);
	String personalDictPath = "src/main/resources/customDict.dic";
	List<DictionaryData> spellDictionary = new ArrayList<DictionaryData>();
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
	SpellChecker.initializeDictionaries(spellDictionary, personalDictPath, 3);
	}

	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@GetMapping("/api/wordeditor/test")
	public String test() {
		System.out.println("==== in test ====");
		return "{\"sections\":[{\"blocks\":[{\"inlines\":[{\"text\":\"Hello World\"}]}]}]}";
	}
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/Import")
	public String importFile(@RequestParam("files") MultipartFile file) throws Exception {
		try {	
			String name = file.getOriginalFilename();
			if (name == null || name.isEmpty()) {
				name = "Document1.docx";
			}
			String format = retrieveFileType(name);
			
			MetafileImageParsedEventHandler metafileImageParsedEvent = new MetafileImageParsedEventHandler() {

	            ListSupport<MetafileImageParsedEventHandler> delegateList = new ListSupport<MetafileImageParsedEventHandler>(
	                    MetafileImageParsedEventHandler.class);

	            // Represents event handling for MetafileImageParsedEventHandlerCollection.
	            public void invoke(Object sender, MetafileImageParsedEventArgs args) throws Exception {
	            	onMetafileImageParsed(sender, args);
	            }

	            // Represents the method that handles MetafileImageParsed event.
	            public void dynamicInvoke(Object... args) throws Exception {
	            	onMetafileImageParsed((Object) args[0], (MetafileImageParsedEventArgs) args[1]);
	            }

	            // Represents the method that handles MetafileImageParsed event to add collection item.
	            public void add(MetafileImageParsedEventHandler delegate) throws Exception {
	                if (delegate != null)
	                    delegateList.add(delegate);
	            }

	            // Represents the method that handles MetafileImageParsed event to remove collection
	            // item.
	            public void remove(MetafileImageParsedEventHandler delegate) throws Exception {
	                if (delegate != null)
	                    delegateList.remove(delegate);
	            }
	        };
	        // Hooks MetafileImageParsed event.
	        WordProcessorHelper.MetafileImageParsed.add("OnMetafileImageParsed", metafileImageParsedEvent);
	        // Converts DocIO DOM to SFDT DOM.
	        String sfdtContent = WordProcessorHelper.load(file.getInputStream(), getFormatType(format));
	        // Unhooks MetafileImageParsed event.
	        WordProcessorHelper.MetafileImageParsed.remove("OnMetafileImageParsed", metafileImageParsedEvent);
	        return sfdtContent;
		} catch (Exception e) {
			e.printStackTrace();
			return "{\"sections\":[{\"blocks\":[{\"inlines\":[{\"text\":" + e.getMessage() + "}]}]}]}";
		}
	}
	
	// Converts Metafile to raster image.
	private static void onMetafileImageParsed(Object sender, MetafileImageParsedEventArgs args) throws Exception {
		if(args.getIsMetafile()) {	    // You can write your own method definition for converting Metafile to raster image using any third-party image converter.
	        args.setImageStream(convertMetafileToRasterImage(args.getMetafileStream()));
		}else {
	        StreamSupport inputStream = args.getMetafileStream();

	        IIORegistry.getDefaultInstance().registerServiceProvider(new TIFFImageReaderSpi());

	        // Create ImageReader and ImageWriter instances
	        ImageReader tiffReader = ImageIO.getImageReadersByFormatName("TIFF").next();
	        ImageWriter pngWriter = ImageIO.getImageWritersByFormatName("PNG").next();

	        // Set up input and output streams
	        tiffReader.setInput(ImageIO.createImageInputStream(tiffInputStream));
	        ByteArrayOutputStream pngOutputStream = new ByteArrayOutputStream();
	        pngWriter.setOutput(ImageIO.createImageOutputStream(pngOutputStream));

	        // Read the TIFF image and write it as a PNG
	        BufferedImage image = tiffReader.read(0);
	        pngWriter.write(image);
	        pngWriter.dispose();
	        byte[] jpgData = pngOutputStream.toByteArray();
	        InputStream jpgStream = new ByteArrayInputStream(jpgData);
	        args.setImageStream(StreamSupport.toStream(jpgStream));
		}
	}
	
	private static StreamSupport convertMetafileToRasterImage(StreamSupport ImageStream) throws Exception {
        //Here we are loading a default raster image as fallback.
		StreamSupport imgStream = getManifestResourceStream("ImageNotFound.jpg");
        return imgStream;
        //To do : Write your own logic for converting Metafile to raster image using any third-party image converter(Syncfusion doesn't provide any image converter).
    }

    private static StreamSupport getManifestResourceStream(String fileName) throws Exception {
    	AssemblySupport assembly = AssemblySupport.getExecutingAssembly();
        return assembly.getManifestResourceStream("ImageNotFound.jpg");
    }
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/SpellCheck")
	public String spellCheck(@RequestBody SpellCheckJsonData spellChecker) throws Exception {
		try {
			   SpellChecker spellCheck = new SpellChecker();
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
			   SpellChecker spellCheck = new SpellChecker();
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
				MetafileImageParsedEventHandler metafileImageParsedEvent = new MetafileImageParsedEventHandler() {

					ListSupport<MetafileImageParsedEventHandler> delegateList = new ListSupport<MetafileImageParsedEventHandler>(
							MetafileImageParsedEventHandler.class);
	
					// Represents event handling for MetafileImageParsedEventHandlerCollection.
					public void invoke(Object sender, MetafileImageParsedEventArgs args) throws Exception {
						onMetafileImageParsed(sender, args);
					}
	
					// Represents the method that handles MetafileImageParsed event.
					public void dynamicInvoke(Object... args) throws Exception {
						onMetafileImageParsed((Object) args[0], (MetafileImageParsedEventArgs) args[1]);
					}
	
					// Represents the method that handles MetafileImageParsed event to add collection item.
					public void add(MetafileImageParsedEventHandler delegate) throws Exception {
						if (delegate != null)
							delegateList.add(delegate);
					}
	
					// Represents the method that handles MetafileImageParsed event to remove collection
					// item.
					public void remove(MetafileImageParsedEventHandler delegate) throws Exception {
						if (delegate != null)
							delegateList.remove(delegate);
					}
				};
				// Hooks MetafileImageParsed event.
				WordProcessorHelper.MetafileImageParsed.add("OnMetafileImageParsed", metafileImageParsedEvent);
				String json = WordProcessorHelper.loadString(param.content, getFormatType(param.type.toLowerCase()));
				// Unhooks MetafileImageParsed event.
				WordProcessorHelper.MetafileImageParsed.remove("OnMetafileImageParsed", metafileImageParsedEvent);
				return json;
			} catch (Exception e) {
				return "";
			}
		}
		return "";
	}
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/Save")
	public void save(@RequestBody SaveParameter data) throws Exception {
		try {
			String name = data.getFileName();
			String format = retrieveFileType(name);
			if (name == null || name.isEmpty()) {
				name = "Document1.docx";
			}
			WordDocument document = WordProcessorHelper.save(data.getContent());
			FileOutputStream fileStream = new FileOutputStream(name);
			document.save(fileStream, getWFormatType(format));
			fileStream.close();
            document.close();
		} catch (Exception ex) {
			throw new Exception(ex.getMessage());
		}
	}
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/ExportSFDT")
	public ResponseEntity<Resource> exportSFDT(@RequestBody SaveParameter data) throws Exception {
		try {
			String name = data.getFileName();
			if (name == null || name.isEmpty()) {
				name = "Document1.docx";
			}
			String format = retrieveFileType(name);

			WordDocument document = WordProcessorHelper.save(data.getContent());
			return saveDocument(document, format);
		} catch (Exception ex) {
			throw new Exception(ex.getMessage());
		}
	}
	
	@CrossOrigin(origins = "*", allowedHeaders = "*")
	@PostMapping("/api/wordeditor/Export")
	public ResponseEntity<Resource> export(@RequestParam("data") MultipartFile data, String fileName) throws Exception {
		try {
			String name = fileName;
			if (name == null || name.isEmpty()) {
				name = "Document1";
			}
			String format = retrieveFileType(name);

			WordDocument document = new WordDocument(data.getInputStream(), com.syncfusion.docio.FormatType.Docx);
			return saveDocument(document, format);
		} catch (Exception ex) {
			throw new Exception(ex.getMessage());
		}
	}
	
	private ResponseEntity<Resource> saveDocument(WordDocument document, String format) throws Exception {
		String contentType = "";
		ByteArrayOutputStream outStream = new ByteArrayOutputStream();
		com.syncfusion.docio.FormatType type = getWFormatType(format);
		switch (type.toString()) {
		case "Rtf":
			contentType = "application/rtf";
			break;
		case "WordML":
			contentType = "application/xml";
			break;
		case "Dotx":
			contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
			break;
		case "Docx":
            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            break;
		case "Html":
			contentType = "application/html";
			break;
		}
		document.save(outStream, type);
		ByteArrayResource resource = new ByteArrayResource(outStream.toByteArray());
		outStream.close();
		document.close();
		
		return ResponseEntity.ok().contentLength(resource.contentLength())
				.contentType(MediaType.parseMediaType(contentType)).body(resource);
	}

	private String retrieveFileType(String name) {
		int index = name.lastIndexOf('.');
		String format = index > -1 && index < name.length() - 1 ? name.substring(index) : ".docx";
		return format;
	}

	static com.syncfusion.docio.FormatType getWFormatType(String format) throws Exception {
		if (format == null || format.trim().isEmpty())
			throw new Exception("EJ2 WordProcessor does not support this file format.");
		switch (format.toLowerCase()) {
		case ".dotx":
			return com.syncfusion.docio.FormatType.Dotx;
		case ".docm":
			return com.syncfusion.docio.FormatType.Docm;
		case ".dotm":
			return com.syncfusion.docio.FormatType.Dotm;
		case ".docx":
			return com.syncfusion.docio.FormatType.Docx;
		case ".rtf":
			return com.syncfusion.docio.FormatType.Rtf;
		case ".html":
			return com.syncfusion.docio.FormatType.Html;
		case ".txt":
			return com.syncfusion.docio.FormatType.Txt;
		case ".xml":
			return com.syncfusion.docio.FormatType.WordML;
		default:
			throw new Exception("EJ2 WordProcessor does not support this file format.");
		}
	}
	
	static FormatType getFormatType(String format)
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
