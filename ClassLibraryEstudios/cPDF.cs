using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Font;
using iText.IO.Font;
using System.Configuration;

namespace ClassLibraryEstudios
{
    public class cPDF
    {
        public static string ubi_Estudios = ConfigurationManager.AppSettings["Estudios"];
        public static string ubi_Logo = ConfigurationManager.AppSettings["Logo"];

        public static String crearPDF(string puerta, string orden, List<string> texto, 
            string paciente, string profesional, string institucion, string fecha, string imgFirma)
        {
            string archivo = String.Empty;
            SolidLine line = new SolidLine(1f);
            
            try
            {
                archivo = ubi_Estudios + puerta + "_" + orden + ".pdf";
                int cantEstudios = texto.Count();
                int cantHojas = 0;

                //Imagen de encabezado
                ImageData imageData = ImageDataFactory.Create(ubi_Logo);
                Image image = new Image(imageData).ScaleToFit(530, 300).
                    SetHorizontalAlignment(HorizontalAlignment.LEFT);

                //Firma de profesional
                ImageData imageFirma = ImageDataFactory.Create(imgFirma);
                Image firma = new Image(imageFirma).ScaleToFit(200, 200).
                    SetHorizontalAlignment(HorizontalAlignment.RIGHT);

                PdfWriter writer = new PdfWriter(archivo);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                Paragraph nuevalinea = new Paragraph(" ")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(20);
                
                //Estilo para etiquetas
                Style etiqueta = new Style();
                PdfFont font = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD);
                etiqueta.SetFont(font).SetFontSize(10);

                //Estilo para el dato
                Style dato = new Style();
                PdfFont font2 = PdfFontFactory.CreateFont(FontConstants.HELVETICA);
                dato.SetFont(font2).SetFontSize(10);

                //Armo tabla de encabezado de tabla
                //Fila 1

                Paragraph A1 = new Paragraph()
                   .SetTextAlignment(TextAlignment.LEFT);
                A1.Add(new Text("Paciente: ").AddStyle(etiqueta));
                A1.Add(new Text(paciente).AddStyle(dato));

                Cell cellA1 = new Cell();
                cellA1.Add(A1);
                cellA1.SetBorder(Border.NO_BORDER);

                Paragraph B1 = new Paragraph()
                   .SetTextAlignment(TextAlignment.LEFT);
                B1.Add(new Text("Orden: ").AddStyle(etiqueta));
                B1.Add(new Text(puerta + " - " + orden).AddStyle(dato));

                Cell cellB1 = new Cell();
                cellB1.Add(B1);
                cellB1.SetBorder(Border.NO_BORDER);


                //Fila 2
                Paragraph A2 = new Paragraph()
                   .SetTextAlignment(TextAlignment.LEFT);
                A2.Add(new Text("Solicitó Dr/a.: ").AddStyle(etiqueta));
                A2.Add(new Text(profesional).AddStyle(dato));

                Cell cellA2 = new Cell();
                cellA2.Add(A2);
                cellA2.SetBorder(Border.NO_BORDER);

                Paragraph B2 = new Paragraph()
                   .SetTextAlignment(TextAlignment.LEFT);
                B2.Add(new Text("Fecha: ").AddStyle(etiqueta));
                B2.Add(new Text(fecha).AddStyle(dato));

                Cell cellB2 = new Cell();
                cellB2.Add(B2);
                cellB2.SetBorder(Border.NO_BORDER);


                //Fila 3
                Paragraph A3 = new Paragraph()
                   .SetTextAlignment(TextAlignment.LEFT);
                A3.Add(new Text("Institucion: ").AddStyle(etiqueta));
                A3.Add(new Text(institucion).AddStyle(dato));

                Cell cellA3 = new Cell();
                cellA3.Add(A3);
                cellA3.SetBorder(Border.NO_BORDER);

                foreach (string t in texto)
                {
                    cantHojas++;

                    //Agrego imagen de encabezado
                    document.Add(image);

                    //Separadores
                    document.Add(nuevalinea);
                    document.Add(nuevalinea);

                    //Armo y configuro tabla
                    float[] separacion = new float[] { 380f, 40f };
                    Table tabla = new Table(separacion);
                    tabla.SetFontSize(10);
                    tabla.SetHorizontalAlignment(HorizontalAlignment.LEFT);

                    //Numero de hoja
                    Paragraph B3 = new Paragraph()
                        .SetTextAlignment(TextAlignment.LEFT);
                    B3.Add(new Text("Hoja: ").AddStyle(etiqueta));
                    B3.Add(new Text(cEnvios.completarCeros(cantHojas.ToString(), 4)).AddStyle(dato));

                    Cell cellB3 = new Cell();
                    cellB3.Add(B3);
                    cellB3.SetBorder(Border.NO_BORDER);

                    //Agrego celdas
                    tabla.AddCell(cellA1);
                    tabla.AddCell(cellB1);
                    tabla.AddCell(cellA2);
                    tabla.AddCell(cellB2);
                    tabla.AddCell(cellA3);
                    tabla.AddCell(cellB3);

                    //Agrego la tabla
                    document.Add(tabla);

                    //Separadores
                    document.Add(nuevalinea);
                    document.Add(nuevalinea);

                    Paragraph txt = new Paragraph(t)
                      .SetTextAlignment(TextAlignment.LEFT)
                      .SetFontSize(10);

                    //Agrego texto del estudio
                    document.Add(txt);

                    //Separador
                    document.Add(nuevalinea);

                    //Imagen de firma
                    document.Add(firma);

                    if(cantHojas<cantEstudios)
                    //Nueva pagina
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }

                //Cierro el documento
                document.Close();
                              
            }
            catch(Exception ex)
            {
                cLog.log.Error("cPDF (crearPDF): " + ex.ToString() +" * " +archivo);
                cLog.log.Error(String.Format("cPDF (crearPDF): Puerta: {0}, Orden: {1}, Estudios: {2}, " +
                    "Paciente: {3}, Profesional: {4}, Institucion: {5}, Fecha: {6}, Firma: {7}.", puerta, orden, 
                    texto.Count.ToString(),paciente,profesional, institucion, fecha, imgFirma));
            }

            return archivo;

        }

    }
}
