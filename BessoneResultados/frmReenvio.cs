using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassLibraryEstudios;
using BessoneResultados.Repositorio;
using BessoneResultados.Servicio;

namespace BessoneResultados
{
    public partial class frmReenvio : Form
    {
        public frmReenvio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string puerta = lblPuerta.Text;
            string orden = lblOrden.Text;
            string mail = txtMail.Text;
            string paciente = lblPaciente.Text;
            string fecha = lblFecha.Text;
            string profesional = lblProfesional.Text;

            reenviarEstudios(puerta, orden, mail, paciente, fecha, profesional);

        }

        private void reenviarEstudios(string puerta, string orden, string mail,
            string paciente, string fecha, string profesional)
        {
            try
            {
                if (cMail.comprobarFormatoEmail(mail))
                {
                    //Agrego ceros
                    orden = cEnvios.completarCeros(orden, 6);

                    if (cMail.enviarMail(puerta,
                                    orden,
                                    mail,
                                    paciente,
                                    fecha,
                                    paciente)
                                )
                    {
                        ServicioEnviados servicioEnviados = new ServicioEnviados();

                        ESTUDIOS_ENVIADOS enviado = new ESTUDIOS_ENVIADOS();

                        enviado.PTA = puerta;
                        enviado.ORD = int.Parse(orden);
                        enviado.PAC = paciente;
                        enviado.MAI = mail;
                        enviado.ENV = DateTime.Today;

                        servicioEnviados.actualizarEnvio(enviado);
                    }
                }

                cLog.log.Debug("frmReenvio (reenviarEstudios): " + String.Format("Estudio reenviado: Paciente {0}, Estudio: {1} ", paciente, puerta+orden));

                this.Close();
            }
            catch (Exception ex)
            {
                cLog.log.Error("frmReenvio (reenviarEstudios): " + ex.ToString());
                throw;
            }

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txtMail.Enabled = true;
            btnOk.Visible = true;
            btnCancel.Visible = true;
            btnEdit.Visible = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            txtMail.Enabled = false;
            btnOk.Visible = false;
            btnCancel.Visible = false;
            btnEdit.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtMail.Text = lblHideMail.Text;
            txtMail.Enabled = false;
            btnOk.Visible = false;
            btnCancel.Visible = false;
            btnEdit.Visible = true;
        }

        private void frmReenvio_Load(object sender, EventArgs e)
        {

        }
    }
}
