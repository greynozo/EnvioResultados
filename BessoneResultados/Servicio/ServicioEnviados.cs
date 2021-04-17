using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BessoneResultados.Repositorio;
using ClassLibraryEstudios;

namespace BessoneResultados.Servicio
{
    public class ServicioEnviados
    {
        public virtual List<cEstudiosEnviados> listadoEnviados()
        {
           
                try
                {
                using (DATATECHEntities cx = new DATATECHEntities())
                {
                    List<cEstudiosEnviados> enviados = new List<cEstudiosEnviados>();

                    var lsEnviados = cx.vw_Estudios_Enviados.OrderByDescending(d => d.ENV).ToList();

                    foreach (var env in lsEnviados)
                    {
                        enviados.Add(new cEstudiosEnviados
                        {
                            Puerta = env.PTA.Trim(),
                            Orden = env.NRO,
                            DNI = (int)env.NDO,
                            NombrePaciente = env.NOM.Trim(),
                            Mail = env.MAI.Trim(),
                            Enviado = env.ENV.ToString().Replace("00:00:00", ""),
                            Profesional = env.PRF.ToString(),
                            Fecha = env.FEC.ToString().Replace("00:00:00", "")
                        });
                    }

                    return enviados;
                }
            }
                catch (Exception ex)
                {
                    cLog.log.Error("ServicioEnviados (listadoEnviados): " + ex.ToString());
                    throw;
                }
                
           
        }

        public virtual void agregarEnvio(ESTUDIOS_ENVIADOS enviado)
        {
            try
            {
                DATATECHEntities cx = new DATATECHEntities();

                cEstudiosEnviados estudiosEnviados = new cEstudiosEnviados();

                cx.ESTUDIOS_ENVIADOS.Add(enviado);

                cx.SaveChanges();
            }
            catch (Exception ex)
            {
                cLog.log.Error("ServicioEnviados (agregarEnvio)" + ex.ToString());
                throw;
            }
        }

        public virtual void actualizarEnvio(ESTUDIOS_ENVIADOS enviado)
        {
            try
            {
                DATATECHEntities cx = new DATATECHEntities();

                ESTUDIOS_ENVIADOS bEnviado = new ESTUDIOS_ENVIADOS();
                ESTUDIOS_ENVIADOS uEnviado = new ESTUDIOS_ENVIADOS();

                bEnviado = buscarEnviado(enviado.PTA, enviado.ORD);

                long id = bEnviado.ID;


                uEnviado = cx.ESTUDIOS_ENVIADOS.Find(id);

                uEnviado.MAI = enviado.MAI;
                uEnviado.ENV = DateTime.Today;

                cx.SaveChanges();
            }
            catch (Exception ex)
            {
                cLog.log.Error("ServicioEnviados (actualizarEnvio)" + ex.ToString());
                throw;
            }
            

        }

        public virtual ESTUDIOS_ENVIADOS buscarEnviado(string puerta, int orden)
        {
            try
            {
                DATATECHEntities cx = new DATATECHEntities();
                ESTUDIOS_ENVIADOS enviado = new ESTUDIOS_ENVIADOS();

                var encontrado = from a in cx.ESTUDIOS_ENVIADOS
                                 where a.PTA == puerta && a.ORD == orden
                                 select a;

                foreach (var en in encontrado)
                {
                    enviado.ID = en.ID;
                    enviado.PTA = en.PTA;
                    enviado.ORD = en.ORD;
                    enviado.MAI = en.MAI;
                    enviado.ENV = en.ENV;
                    enviado.PAC = en.PAC;
                }

                return enviado;
            }
            catch (Exception ex) 
            {
                cLog.log.Error("ServicioEnviados (buscarEnviado)" + ex.ToString());
                throw;
            }
           
        }

        public virtual List<cEstudiosEnviados> filtrarEnviados(string filtro, string dato)
        {
                try
                {
                using (DATATECHEntities cx = new DATATECHEntities())
                {
                    List<cEstudiosEnviados> enviados = new List<cEstudiosEnviados>();

                    switch (filtro)
                    {
                        case "Puerta":
                            string puerta = dato.ToUpper();
                            var listaPuerta = cx.vw_Estudios_Enviados.
                                Where(d => d.PTA.Contains(puerta)).OrderByDescending(c => c.ENV).ToList();
                            enviados = cargoLista(listaPuerta);
                            break;
                        case "Orden":
                            int numero = int.Parse(dato);
                            var listaOrden = cx.vw_Estudios_Enviados.
                                Where(d => d.NRO.Equals(numero)).OrderByDescending(c => c.ENV).ToList();
                            enviados = cargoLista(listaOrden);
                            break;
                        case "Mail":
                            string mail = dato.ToUpper();
                            var listaMail = cx.vw_Estudios_Enviados.
                                Where(d => d.MAI.ToUpper().Contains(mail)).OrderByDescending(c => c.ENV).ToList();
                            enviados = cargoLista(listaMail);
                            break;
                        case "Paciente":
                            string paciente = dato.ToUpper();
                            var listaPaciente = cx.vw_Estudios_Enviados.
                                Where(d => d.NOM.Contains(paciente)).OrderByDescending(c => c.ENV).ToList();
                            enviados = cargoLista(listaPaciente);
                            break;
                        default:
                            break;
                    }

                    return enviados;
                }
                }
                catch (Exception ex)
                {
                    cLog.log.Error("ServicioEnviados (filtrarEnviados)" + ex.ToString());
                    throw ex;
                }
              
        }

        private static List<cEstudiosEnviados> cargoLista(List<vw_Estudios_Enviados> listaPuerta)
        {
            try
            {
                List<cEstudiosEnviados> listado = new List<cEstudiosEnviados>();
                foreach (var env in listaPuerta)
                {
                    listado.Add(new cEstudiosEnviados
                    {
                        Puerta = env.PTA.Trim(),
                        Orden = env.NRO,
                        DNI = (int)env.NDO,
                        NombrePaciente = env.NOM.Trim(),
                        Mail = env.MAI.Trim(),
                        Profesional = env.PRF.Trim(),
                        Fecha = env.FEC.ToString().Replace("00:00:00", ""),
                        Enviado = env.ENV.ToString().Replace("00:00:00", "")
                    });

                }

                return listado;
            }
            catch (Exception ex)
            {
                cLog.log.Error("ServicioEnviados (cargoLista)" + ex.ToString());
                throw;
            }

        }
    }
}