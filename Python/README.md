# Integrated .Net Core Library in python

We have created Documenteditor utility with following functionalities:

* Import
* ExportSfdt
* SystemClipboard
* RestrictEditing

## Import

You can provide parameter as base64String of file which will convert to SFDT format by using this API. In import.py, we have provided default document as base64String.

## SystemClipboard

You can make use of this service in order to paste system clipboard data by preserving the formatting. In parameter, you can provide clipboard content as rtf or html string with type.

## ExportSfdt

You can export the SFDT string Doc, DOCX, RTF, Txt, WordML, HTML formats by using this API in server side.

## RestrictEditing

You can make use of this service in order to encrypt/decrypt protected content. 