using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace ClassLibraryEstudios
{
    public class cEnvios
    {
        public static string ubi_Imagenes = ConfigurationManager.AppSettings["Imagenes"];
        public static string ubi_Envios = ConfigurationManager.AppSettings["Envios"];

        public static void generarEnvios(List<cEstudiosPacientes> busqueda)
        {
            try
            {
                string ubicacion = ubi_Envios;
                string archivo;
                string pdf;
                string texto;
                List<string> estudios = new List<string>();
                string tmpPuerta = String.Empty;
                string tmpPaciente = String.Empty;
                string tmpProfesional = String.Empty;
                string tmpInstitucio = String.Empty;
                string tmpMail = String.Empty;
                string tmpFecha = String.Empty;
                string tmpFirma = String.Empty;
                int tmpOrden = 0;
                int cantEstudios = 0;

                //Genero un archivo por cada paciente para su posterior envio.
                foreach (var b in busqueda)
                {
                    cantEstudios++;

                    texto = ConversorRTF.CConversorRTF.StripRichTextFormat(b.Texto);

                    if (b.Puerta == tmpPuerta && b.Orden == tmpOrden || tmpOrden == 0)
                    {
                        if (!(cantEstudios == busqueda.Count()))
                        {
                            estudios.Add(b.Titulo + "\n" + texto);
                            tmpPuerta = b.Puerta;
                            tmpOrden = b.Orden;
                            tmpPaciente = b.NombrePaciente;
                            tmpProfesional = b.Profesional;
                            tmpInstitucio = b.Institucion;
                            tmpFecha = b.Fecha.ToString();
                            tmpFirma = b.Firma;
                            tmpMail = b.Mail;
                        }
                        else
                        {
                            estudios.Add(b.Titulo + "\n" + texto);

                            //DateTime fecha = new DateTime();
                            //fecha = DateTime.Parse(tmpFecha);
                            string fec = b.Fecha.ToString("dd/MM/yyyy");
                            
                            if(tmpPuerta!="" || tmpOrden > 0)
                            {
                                pdf = cPDF.crearPDF(tmpPuerta.Trim(), tmpOrden.ToString().Trim(), estudios,
                                tmpPaciente.Trim(), tmpProfesional.Trim(), tmpInstitucio.Trim(), fec, tmpFirma);

                                archivo = ubicacion + tmpPuerta.Trim() + "_" + tmpOrden.ToString().Trim() + ".env";

                                using (FileStream fs = File.Create(archivo))
                                {
                                    byte[] info = new UTF8Encoding(true).GetBytes(tmpMail.Trim() + ";" +
                                        tmpPaciente.Trim() + ";" + tmpProfesional.Trim() + ";" + fec.Trim() + ";" +
                                        tmpPuerta.Trim() + ";" + tmpOrden.ToString().Trim());

                                    fs.Write(info, 0, info.Length);
                                }
                            }
                            else
                            {
                                pdf = cPDF.crearPDF(b.Puerta.Trim(), b.Orden.ToString().Trim(), estudios,
                                b.NombrePaciente.Trim(), b.Profesional.Trim(), b.Institucion.Trim(), fec, b.Firma);

                                archivo = ubicacion + b.Puerta.Trim() + "_" + b.Orden.ToString().Trim() + ".env";

                                using (FileStream fs = File.Create(archivo))
                                {
                                    byte[] info = new UTF8Encoding(true).GetBytes(b.Mail.Trim() + ";" +
                                        b.NombrePaciente.Trim() + ";" + b.Profesional.Trim() + ";" + fec.Trim() + ";" +
                                        b.Puerta.Trim() + ";" + b.Orden.ToString().Trim());

                                    fs.Write(info, 0, info.Length);
                                }
                            }
                        }
                    }
                    else
                    {
                        DateTime fecha = new DateTime();
                        fecha = DateTime.Parse(tmpFecha);
                        string fec = fecha.ToString("dd/MM/yyyy");

                        pdf = cPDF.crearPDF(tmpPuerta.Trim(), tmpOrden.ToString().Trim(), estudios,
                       tmpPaciente.Trim(), tmpProfesional.Trim(), tmpInstitucio.Trim(), fec, tmpFirma);

                        archivo = ubicacion + tmpPuerta.Trim() + "_" + tmpOrden.ToString().Trim() + ".env";

                        using (FileStream fs = File.Create(archivo))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(tmpMail.Trim() + ";" +
                                tmpPaciente.Trim() + ";" + tmpProfesional.Trim() + ";" + fec.Trim() + ";" +
                                tmpPuerta.Trim() + ";" + tmpOrden.ToString().Trim());

                            fs.Write(info, 0, info.Length);
                        }

                        tmpPuerta = b.Puerta;
                        tmpOrden = b.Orden;
                        tmpPaciente = b.NombrePaciente;
                        tmpProfesional = b.Profesional;
                        tmpInstitucio = b.Institucion;
                        tmpFecha = b.Fecha.ToString();
                        tmpFirma = b.Firma;
                        tmpMail = b.Mail;
                        estudios.Clear();
                        estudios.Add(b.Titulo + "\n" + texto);
                    }

                }
            }
            catch (Exception ex)
            {
                cLog.log.Error("cEnvios (generarEnvios): " + ex.ToString());
                throw;
            }
            
        
        }

        public static string completarCeros(string numero, int cantCeros)
        {
            int ceros;

            ceros = cantCeros - numero.Trim().Length;

            for (int i = 1; i <= ceros; i++)
            {
                numero = "0" + numero;
            }

            return numero;
        }

        public static bool comprobarImagenes(string puerta, string orden)
        {
            bool existeCarpeta;
            string[] archivos;
            int cantArchivos;
            bool hayImagenes = false;
            
                existeCarpeta = Directory.Exists(ubi_Imagenes + "\\" + puerta + orden + "\\");

                if (existeCarpeta)
                {
                    archivos = Directory.GetFiles(ubi_Imagenes + "\\" + puerta + orden + "\\");
                    cantArchivos = archivos.Count();

                    if (cantArchivos > 0)
                    {
                        hayImagenes = true;
                        return hayImagenes;
                    }
                        
                }
                else
                {
                    hayImagenes = false;
                    return hayImagenes;
                }
            return hayImagenes;
        }

        public static void eliminarEnvios()
        {
            List<string> strEnvios = Directory.GetFiles(ubi_Envios, "*", SearchOption.AllDirectories).ToList();

            foreach (string env in strEnvios)
            {   
                File.Delete(env);
            }
        }

    }
}
