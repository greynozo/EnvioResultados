using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BessoneResultados.Repositorio;
using ClassLibraryEstudios;
using System.Data.Entity;

namespace BessoneResultados.Servicio
{
    class ServicioEstudios
    {
        public virtual List<String> listadoPuertas()
        {
         try
           {
                using (DATATECHEntities cx = new DATATECHEntities())
                {
                    List<String> puertas = new List<String>();

                    var listado = cx.CLIPTA.Where(d => (d.SER == "EC" || d.SER == "EV" || d.SER == "EI") && d.ORI != "O").OrderBy(d => d.PTA).ToList();

                    foreach (var lp in listado)
                    {
                        puertas.Add(lp.PTA.ToString());
                    }
                    return puertas;
                }
            }
                catch (Exception ex)
                {
                    cLog.log.Error("ServicioEstudios (listadoPuertas): " + ex.ToString());
                    throw;
                }
                
        }


        public virtual List<cEstudiosPacientes> buscarPorNumero(string puerta, int desde, int hasta)
        {
            
                try
                {
                using (DATATECHEntities cx = new DATATECHEntities())
                {
                    List<cEstudiosPacientes> resultado = new List<cEstudiosPacientes>();

                    var listado = (from a in cx.vw_Estudios
                                   where a.PTA == puerta && (a.NRO >= desde && a.NRO <= hasta)
                                   select a).Select(d => new cEstudiosPacientes
                                   {
                                       Puerta = d.PTA.Trim(),
                                       Orden = d.NRO,
                                       Titulo = d.TIT.Trim(),
                                       Texto = d.TXT,
                                       Fecha = d.FEC,
                                       DNI = (int)d.NDO,
                                       NombrePaciente = d.NOM.Trim(),
                                       Mail = d.MAI.Trim(),
                                       Institucion = d.DES.Trim(),
                                       Profesional = d.PRF.Trim(),
                                       Firma = d.ARC_FIR.Trim(),

                                   }).OrderBy(d => d.Orden).ToList();

                    return listado;
                }
            }
                catch (Exception ex)
                {
                    cLog.log.Error("ServicioEstudios (buscarPorNumero): " + ex.ToString());
                    throw;
                }
               

            
            
        }

        public virtual List<cEstudiosPacientes> buscarPorFecha(string puerta, DateTime desde, DateTime hasta)
        {
           
               try
                {
                    using (DATATECHEntities cx = new DATATECHEntities())
                    {
                        List<cEstudiosPacientes> resultado = new List<cEstudiosPacientes>();

                        //TODO: Hacer lo mismo que la busqueda de numero
                        var listado = (from a in cx.vw_Estudios
                                       where a.PTA == puerta && (DbFunctions.TruncateTime(a.FEC) >= desde.Date && DbFunctions.TruncateTime(a.FEC) <= hasta.Date)
                                       select a).Select(d => new cEstudiosPacientes
                                       {
                                           Puerta = d.PTA.Trim(),
                                           Orden = d.NRO,
                                           Titulo = d.TIT.Trim(),
                                           Texto = d.TXT,
                                           Fecha = d.FEC,
                                           DNI = (int)d.NDO,
                                           NombrePaciente = d.NOM.Trim(),
                                           Mail = d.MAI.Trim(),
                                           Institucion = d.DES.Trim(),
                                           Profesional = d.PRF.Trim(),
                                           Firma = d.ARC_FIR.Trim(),

                                       }).OrderBy(d => d.Orden).ToList();

                        return listado;

                    }
                }
                catch (Exception ex)
                {
                    cLog.log.Error("ServicioEstudios (buscarPorFecha): " + ex.ToString());
                    throw;
                }
                

        }
    }
}
