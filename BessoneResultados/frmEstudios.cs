using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BessoneResultados.Repositorio;
using System.Configuration;
using BessoneResultados.Servicio;
using ClassLibraryEstudios;
using System.IO;
using log4net;

namespace BessoneResultados
{
    public partial class frmEstudios : Form
    {
        public static string ubi_Envios = ConfigurationManager.AppSettings["Envios"];
        
        public frmEstudios()
        {
            InitializeComponent();
        }

        private void frmEstudios_Load(object sender, EventArgs e)
        {
            cLog.log.Debug("Se inicia la aplicacion.");
            llenarCombo();
            llenarEnviados();
        }

        private void llenarCombo()
        {
            ServicioEstudios servicioEstudios = new ServicioEstudios();

            cbPuerta.DataSource = servicioEstudios.listadoPuertas();

            cbPuerta.DisplayMember = "PTA";
        }


        private void llenarEnviados()
        {

            List<cEstudiosEnviados> busqueda = new List<cEstudiosEnviados>();
            ServicioEnviados servicioEnviados = new ServicioEnviados();

            busqueda = servicioEnviados.listadoEnviados();

            cargarGrilla(busqueda);

        }

        private void cargarGrilla(List<cEstudiosEnviados> busqueda)
        {
            try
            {
                dgvEstudiosEnviados.DataSource = busqueda;
                dgvEstudiosEnviados.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvEstudiosEnviados.Columns[6].Visible = false;
                dgvEstudiosEnviados.Columns[7].Visible = false;
                dgvEstudiosEnviados.Columns[8].Visible = false;
                dgvEstudiosEnviados.Columns[9].Visible = false;
                dgvEstudiosEnviados.Columns[10].Visible = false;
            }
            catch (Exception ex)
            {
                cLog.log.Error("frmEstudios (cargarGrilla): " + ex.ToString());
                throw;
            }
            
        }

        private void btnBuscarEstudios_Click(object sender, EventArgs e)
        {
            buscarEstudios();

        }

        private void buscarEstudios()
        {
            try
            {
                //Limpio la carpeta de envios.
                cEnvios.eliminarEnvios();

                List<cEstudiosPacientes> busqueda = new List<cEstudiosPacientes>();

                string puerta = cbPuerta.SelectedItem.ToString();

                if (rbPorNumero.Checked)
                {
                    int desde = int.Parse(txtDesde.Text);
                    int hasta = int.Parse(txtHasta.Text);

                    //Realizo busqueda por numero
                    ServicioEstudios servicioEstudios = new ServicioEstudios();

                    busqueda = servicioEstudios.buscarPorNumero(puerta, desde, hasta);

                    cEnvios.generarEnvios(busqueda);

                    dgvEstudiosEncontrados.DataSource = busqueda;
                    dgvEstudiosEnviados.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvEstudiosEncontrados.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvEstudiosEncontrados.Columns[3].Visible = false;
                    dgvEstudiosEncontrados.Columns[8].Visible = false;
                    dgvEstudiosEncontrados.Columns[10].Visible = false;

                    tabControl1.SelectedTab = tabPage2;

                    colorFilas();

                }
                else
                {
                    //Realizo busqueda por numero
                    ServicioEstudios servicioEstudios = new ServicioEstudios();

                    DateTime desde = dtDesde.Value;
                    DateTime hasta = dtHasta.Value;

                    busqueda = servicioEstudios.buscarPorFecha(puerta, desde, hasta);

                    cEnvios.generarEnvios(busqueda);

                    dgvEstudiosEncontrados.DataSource = busqueda;
                    dgvEstudiosEnviados.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvEstudiosEncontrados.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvEstudiosEncontrados.Columns[3].Visible = false;
                    dgvEstudiosEncontrados.Columns[8].Visible = false;
                    dgvEstudiosEncontrados.Columns[10].Visible = false;
                    tabControl1.SelectedTab = tabPage2;

                    colorFilas();

                }
            }
            catch (Exception ex)
            {
                cLog.log.Error("frmEstudios (buscarEstudios): " + ex.ToString());
                throw;
            }
            
        }

        private void btnEnviarEstudios_Click(object sender, EventArgs e)
        {
            enviarEstudios();
            tabControl1.SelectedTab = tabPage3;
            llenarEnviados();
        }

        private void enviarEstudios()
        {
            try
            {
                string[] envios = Directory.GetFiles(ubi_Envios);

                int cantEnvios = envios.Length;
                progressBar1.Minimum = 1;
                progressBar1.Maximum = cantEnvios;

                int vuelta = 0;

                foreach (string en in envios)
                {

                    vuelta++;

                    if (vuelta <= progressBar1.Maximum)
                    {
                        progressBar1.Value = vuelta;
                        lblEnviados.Text = vuelta.ToString() + " enviados";
                    }

                    string linea;
                    string[] env;
                    string orden;
                    bool hay_imagenes;

                    StreamReader archivo = new StreamReader(en);
                    linea = archivo.ReadLine();

                    env = linea.Split(';');

                    //Si no tiene imagenes, no envio
                    hay_imagenes = cEnvios.comprobarImagenes(env[4].ToString().Trim(), env[5].ToString().ToString());

                    if(hay_imagenes)
                    {
                        //Verifico mail
                        if (cMail.comprobarFormatoEmail(env[0].ToString().Trim()))
                        {
                            //Agrego ceros
                            orden = cEnvios.completarCeros(env[5].ToString(), 6);

                            if (cMail.enviarMail(env[4].ToString().Trim(),
                                            orden,
                                            env[0].ToString().Trim(),
                                            env[1].ToString().Trim(),
                                            env[3].ToString().Trim(),
                                            env[2].ToString().Trim())
                                        )
                            {
                                ServicioEnviados servicioEnviados = new ServicioEnviados();

                                ESTUDIOS_ENVIADOS enviado = new ESTUDIOS_ENVIADOS();

                                enviado.PTA = env[4].ToString().Trim();
                                enviado.ORD = int.Parse(orden);
                                enviado.PAC = env[1].ToString().Trim();
                                enviado.MAI = env[0].ToString().Trim();
                                enviado.ENV = DateTime.Today;

                                servicioEnviados.agregarEnvio(enviado);
                            }
                        }

                    }
                    else
                    {
                        cLog.log.Debug(String.Format("El estudio {0}, no posee imagenes. No se realiza envio.", env[4].ToString().Trim()+ env[5].ToString()));
                    }

                    archivo.Close();

                    File.Delete(en);

                }
            }
            catch (Exception ex)
            {
                cLog.log.Error("frmEstudios (enviarEstudios): " + ex.ToString());
                throw;
            }
            

        }


        private void colorFilas()
        {
            try
            {
                foreach (DataGridViewRow Myrow in dgvEstudiosEncontrados.Rows)
                {
                    bool esMail;
                    bool hayImagenes;

                    esMail = cMail.comprobarFormatoEmail(Myrow.Cells[7].Value.ToString().Trim());
                    hayImagenes = cEnvios.comprobarImagenes(Myrow.Cells[0].Value.ToString().Trim(), 
                            Myrow.Cells[1].Value.ToString());

                    if (hayImagenes)
                    {
                        if(!esMail)
                        {
                            Myrow.DefaultCellStyle.BackColor = Color.LightCoral;
                        }
                    }
                    else
                    {
                        Myrow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }

                }
            }
            catch (Exception ex)
            {
                cLog.log.Error("frmEstudios (colorFilas): " + ex.ToString());
                throw;
            }
            
        }

        private void txtDesde_Leave(object sender, EventArgs e)
        {
            txtHasta.Text = txtDesde.Text;
        }

        private void rbPorNumero_CheckedChanged(object sender, EventArgs e)
        {
            if(rbPorNumero.Checked)
            {
                gbFecha.Enabled = false;
                gbOrden.Enabled = true;
            }
            else
            {
                gbOrden.Enabled = false;
                gbFecha.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<cEstudiosEnviados> busqueda = new List<cEstudiosEnviados>();
            ServicioEnviados servicioEnviados = new ServicioEnviados();
            string filtro = cbFiltro.Text;
            string dato = txtBuscado.Text;
            int numero;

            if (filtro == "Orden")
            {
                if(!int.TryParse(dato, out numero))
                {
                    MessageBox.Show("El valor de Orden debe ser numerico.");
                }
               
            }
            else
            {
                cLog.log.Debug("frmEstudios (Filtro): Filtro: " + filtro + " - Dato: " + dato.ToString() );
                busqueda = servicioEnviados.filtrarEnviados(filtro, dato);

                cargarGrilla(busqueda);
            }

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            llenarEnviados();
            txtBuscado.Text = String.Empty;
            cbFiltro.Text = "Paciente";
        }

   
        private void dgvEstudiosEnviados_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            frmReenvio reenvio = new frmReenvio();

            reenvio.lblPuerta.Text = dgvEstudiosEnviados.CurrentRow.Cells[0].Value.ToString();
            reenvio.lblOrden.Text = dgvEstudiosEnviados.CurrentRow.Cells[1].Value.ToString();
            reenvio.lblProfesional.Text = dgvEstudiosEnviados.CurrentRow.Cells[6].Value.ToString();
            reenvio.lblFecha.Text = dgvEstudiosEnviados.CurrentRow.Cells[10].Value.ToString();
            reenvio.lblPaciente.Text = dgvEstudiosEnviados.CurrentRow.Cells[3].Value.ToString();
            reenvio.txtMail.Text = dgvEstudiosEnviados.CurrentRow.Cells[4].Value.ToString();
            reenvio.lblHideMail.Text = dgvEstudiosEnviados.CurrentRow.Cells[4].Value.ToString();
            reenvio.ShowDialog();
        }


    }
}
