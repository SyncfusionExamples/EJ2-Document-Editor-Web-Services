using SkiaSharp;
using BitMiracle.LibTiff.Classic;
using Syncfusion.EJ2.DocumentEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentEditorLibrary
{
    public class Documenteditor
    {
        // Converts the uploaded Word document into the SFDT string required by the DOCX Editor.
        // The MetafileImageParsed event converts TIFF, EMF, and WMF images to raster images
        // because web browsers cannot display metafile images.
        public string Import(byte[] byteArray, string fileName)
        {
            Stream stream = new MemoryStream(byteArray);
            int index = fileName.LastIndexOf('.');
            string type = index > -1 && index < fileName.Length - 1 ?
                fileName.Substring(index) : ".docx";

            try
            {
                //Hooks MetafileImageParsed event.
                WordDocument.MetafileImageParsed += OnMetafileImageParsed;
                WordDocument document = WordDocument.Load(stream, GetFormatType(type.ToLower()));
                //Unhooks MetafileImageParsed event.
                WordDocument.MetafileImageParsed -= OnMetafileImageParsed;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                stream.Dispose();
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Converts clipboard content (HTML/RTF) into SFDT so content can be pasted with formatting.
        public string SystemClipboard(string Content, string type)
        {
            if (Content != null && Content != "")
            {
                try
                {
                    WordDocument document = WordDocument.LoadString(Content, GetFormatType(type));
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                    document.Dispose();
                    return json;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "";
        }

        // Saves the SFDT content as a Word document with the given file name and returns it.
        public byte[] Save(string content, string fileName)
        {
            Syncfusion.DocIO.DLS.WordDocument document = WordDocument.Save(content);
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, Syncfusion.DocIO.FormatType.Docx);
                document.Close();
                return stream.ToArray();
            }
        }

        // Computes the password hash used by the document protection feature.
        public string[] RestrictEditing(string passwordBase64, string saltBase64, int spinCount)
        {
            if (passwordBase64 == "" && passwordBase64 == null)
                return null;
            return WordDocument.ComputeHash(passwordBase64, saltBase64, spinCount);
        }

        // Performs mail merge on the uploaded Word document and returns the merged document as SFDT.
        public string MailMerge(string documentData)
        {
            byte[] data = Convert.FromBase64String(
                documentData.Contains(',') ? documentData.Split(',')[1] : documentData);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                using (Syncfusion.DocIO.DLS.WordDocument document =
                    new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx))
                {
                    document.MailMerge.RemoveEmptyGroup = true;
                    document.MailMerge.RemoveEmptyParagraphs = true;
                    document.MailMerge.ClearFields = true;
                    document.MailMerge.Execute(CustomerDataModel.GetAllRecords());
                    document.Save(stream, Syncfusion.DocIO.FormatType.Docx);
                }

                stream.Position = 0;
                WordDocument wordDocument = WordDocument.Load(stream, FormatType.Docx);
                string sfdtText = Newtonsoft.Json.JsonConvert.SerializeObject(wordDocument);
                wordDocument.Dispose();
                return sfdtText;
            }
        }

        // Compares the original and revised Word documents and returns
        // the comparison result (with tracked changes) as SFDT.
        public string CompareDocuments(byte[] originalFile, byte[] revisedFile)
        {
            using (Syncfusion.DocIO.DLS.WordDocument revisedDocument =
                new Syncfusion.DocIO.DLS.WordDocument(new MemoryStream(revisedFile), Syncfusion.DocIO.FormatType.Docx))
            using (Syncfusion.DocIO.DLS.WordDocument originalDocument =
                new Syncfusion.DocIO.DLS.WordDocument(new MemoryStream(originalFile), Syncfusion.DocIO.FormatType.Docx))
            {
                originalDocument.Compare(revisedDocument);

                //Hooks MetafileImageParsed event.
                WordDocument.MetafileImageParsed += OnMetafileImageParsed;
                WordDocument document = WordDocument.Load(originalDocument);
                //Unhooks MetafileImageParsed event.
                WordDocument.MetafileImageParsed -= OnMetafileImageParsed;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                return json;
            }
        }

        //Converts Metafile to raster image.
        private static void OnMetafileImageParsed(object sender, MetafileImageParsedEventArgs args)
        {
            if (args.IsMetafile)
            {
                //MetaFile image conversion(EMF and WMF)
                //You can write your own method definition for converting metafile to raster image using any third-party image converter.
                args.ImageStream = ConvertMetafileToRasterImage(args.MetafileStream);
            }
            else
            {
                //TIFF image conversion
                args.ImageStream = TiffToPNG(args.MetafileStream);
            }
        }

        // Converting Tiff to Png image using Bitmiracle https://www.nuget.org/packages/BitMiracle.LibTiff.NET
        private static MemoryStream TiffToPNG(Stream tiffStream)
        {
            MemoryStream imageStream = new MemoryStream();
            using (BitMiracle.LibTiff.Classic.Tiff tif =
                BitMiracle.LibTiff.Classic.Tiff.ClientOpen("in-memory", "r", tiffStream, new BitMiracle.LibTiff.Classic.TiffStream()))
            {
                // Find the width and height of the image
                BitMiracle.LibTiff.Classic.FieldValue[] value =
                    tif.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGEWIDTH);
                int width = value[0].ToInt();

                value = tif.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGELENGTH);
                int height = value[0].ToInt();

                // Read the image into the memory buffer
                int[] raster = new int[height * width];
                if (!tif.ReadRGBAImage(width, height, raster))
                {
                    throw new Exception("Could not read image");
                }

                // Create a bitmap image using SkiaSharp.
                using (SKBitmap sKBitmap = new SKBitmap(
                    width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul))
                {
                    // Convert a RGBA value to byte array.
                    byte[] bitmapData = new byte[sKBitmap.RowBytes * sKBitmap.Height];
                    for (int y = 0; y < sKBitmap.Height; y++)
                    {
                        int rasterOffset = y * sKBitmap.Width;
                        int bitsOffset = (sKBitmap.Height - y - 1) * sKBitmap.RowBytes;

                        for (int x = 0; x < sKBitmap.Width; x++)
                        {
                            int rgba = raster[rasterOffset++];
                            bitmapData[bitsOffset++] = (byte)((rgba >> 16) & 0xff);
                            bitmapData[bitsOffset++] = (byte)((rgba >> 8) & 0xff);
                            bitmapData[bitsOffset++] = (byte)(rgba & 0xff);
                            bitmapData[bitsOffset++] = (byte)((rgba >> 24) & 0xff);
                        }
                    }

                    // Convert a byte array to SKColor array.
                    SKColor[] sKColor = new SKColor[bitmapData.Length / 4];
                    int index = 0;
                    for (int i = 0; i < bitmapData.Length; i++)
                    {
                        sKColor[index] = new SKColor(
                            bitmapData[i + 2], bitmapData[i + 1], bitmapData[i], bitmapData[i + 3]);
                        i += 3;
                        index += 1;
                    }

                    // Set the SKColor array to SKBitmap.
                    sKBitmap.Pixels = sKColor;

                    // Save the SKBitmap to PNG image stream.
                    sKBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(imageStream);
                    imageStream.Flush();
                }
            }
            return imageStream;
        }

        private static Stream ConvertMetafileToRasterImage(Stream ImageStream)
        {
            //Here we are loading a default raster image as fallback.
            Stream imgStream = GetManifestResourceStream("ImageNotFound.jpg");
            return imgStream;
            //To do : Write your own logic for converting metafile to raster image using any third-party image converter(Syncfusion doesn't provide any image converter).
        }

        private static Stream GetManifestResourceStream(string fileName)
        {
            System.Reflection.Assembly execAssembly = typeof(Syncfusion.DocIO.DLS.WordDocument).Assembly;
            string[] resourceNames = execAssembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.EndsWith("." + fileName))
                {
                    fileName = resourceName;
                    break;
                }
            }
            return execAssembly.GetManifestResourceStream(fileName);
        }

        internal static FormatType GetFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
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
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }
    }

    // Data records used by the MailMerge API.
    public class CustomerDataModel
    {
        public static List<Customer> GetAllRecords()
        {
            List<Customer> customers = new List<Customer>();
            customers.Add(new Customer("9072379", "50%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "2000", "19072379", "Folk och fä HB", "100000", "440", "32.34", "472.34", "28023", "12000", "2020-11-07 00:00:00", "2020-12-07 00:00:00"));
            customers.Add(new Customer("9072378", "20%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "", "2", "19072369", "Maersk", "140000", "245", "20", "265", "28024", "12400", "2020-11-31 00:00:00", "2020-12-22 00:00:00"));
            customers.Add(new Customer("9072377", "30%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "100", "19072879", "Mediterranean Shipping Company", "104000", "434", "50.43", "484.43", "28025", "10000", "2020-11-07 00:00:00", "2020-12-02 00:00:00"));
            customers.Add(new Customer("9072393", "10%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "2050", "19072378", "China Ocean Shipping Company", "175000", "500", "32", "532", "28026", "17000", "2020-09-23 00:00:00", "2020-10-09 00:00:00"));
            customers.Add(new Customer("9072377", "14%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "2568", "19072380", "CGM", "155000", "655", "20.54", "675.54", "28027", "13000", "2020-10-11 00:00:00", "2020-11-17 00:00:00"));
            customers.Add(new Customer("9072376", "0%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "1532", "19072345", " Hapag-Lloyd", "106500", "344", "30", "374", "28028", "14500", "2020-06-17 00:00:00", "2020-07-07 00:00:00"));
            customers.Add(new Customer("9072369", "05%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "4462", "190723452", "Ocean Network Express", "100054", "541", "50", "591", "28029", "16500", "2020-04-07 00:00:00", "2020-05-07 00:00:00"));
            customers.Add(new Customer("9072359", "4%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "27547", "190723713", "Evergreen Line", "124000", "800", "10.23", "810.23", "28030", "12500", "2020-03-07 00:00:00", "2020-04-07 00:00:00"));
            customers.Add(new Customer("9072380", "20%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "7582", "19072312", "Yang Ming Marine Transport", "1046000", "290", "10", "300", "27631", "12670", "2020-11-10 00:00:00", "2020-12-13 00:00:00"));
            customers.Add(new Customer("9072381", "42%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "862", "19072354", "Hyundai Merchant Marine", "145000", "800", "10.23", "810.23", "28032", "45000", "2020-10-17 00:00:00", "2020-12-23 00:00:00"));
            customers.Add(new Customer("9072391", "84%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "82", "19072364", "Pacific International Line", "10094677", "344", "30", "374", "28033", "16500", "2020-11-14 00:00:00", "2020-12-21 00:00:00"));
            customers.Add(new Customer("9072392", "92%", "C/ Araquil, 67", "Madrid", "22020-08-10 00:00:00", "Spain", "Brittania", "82", "19072385", "Österreichischer Lloyd", "104270", "500", "32", "532", "28034", "156500", "2020-06-07 00:00:00", "2020-07-07 00:00:00"));
            return customers;
        }
    }

    public class Customer
    {
        public string CustomerID { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string ShipName { get; set; }
        public string UnitPrice { get; set; }
        public string Discount { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string OrderDate { get; set; }
        public string ShipCountry { get; set; }
        public string OrderId { get; set; }
        public string Subtotal { get; set; }
        public string Freight { get; set; }
        public string Total { get; set; }
        public string ShipPostalCode { get; set; }
        public string RequiredDate { get; set; }
        public string ShippedDate { get; set; }
        public string ExtendedPrice { get; set; }

        public Customer(
            string orderId, string discount, string shipAddress, string shipCity,
            string orderDate, string shipCountry, string productName, string quantity,
            string customerID, string shipName, string unitPrice, string subtotal,
            string freight, string total, string shipPostalCode, string extendedPrice,
            string requiredDate, string shippedDate)
        {
            this.CustomerID = customerID;
            this.ProductName = productName;
            this.Quantity = quantity;
            this.ShipName = shipName;
            this.UnitPrice = unitPrice;
            this.Discount = discount;
            this.ShipAddress = shipAddress;
            this.ShipCity = shipCity;
            this.OrderDate = orderDate;
            this.ShipCountry = shipCountry;
            this.OrderId = orderId;
            this.Subtotal = subtotal;
            this.Freight = freight;
            this.Total = total;
            this.ShipPostalCode = shipPostalCode;
            this.ShippedDate = shippedDate;
            this.RequiredDate = requiredDate;
            this.ExtendedPrice = extendedPrice;
        }
    }
}