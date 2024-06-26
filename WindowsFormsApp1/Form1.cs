using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp1.ScrollableMessageBox;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static string test = @"Mirabar insanis conemur in ac allatis ideoque mo. Ente ac meos huic soli vero et. Expectem lectores effectus age per nihildum assumere. Du et addantur rationes ut perpauca. Ex velut vulgo pappo majus ha illam eo vocem. Mo ab talis se si inter somno locum nulla. Iis iii rari omni tur ista.
                    Mem dem ima nequeam vos gratiam junctas aliunde maximam.Tot denuo terea tur ritas justa talem vel.At possumus ac privatio superare in excitari tractant.Advertisse incrementi quaerendum conservant denegassem ei in facillimum.Ii mo utilius quamvis rationi ut fuerunt.Mea dignum vos ibidem tantae quidam cau quaeri cap.Adipisci co de rationis ut originem competit sessione sequatur ad.Artificium frequenter agi excoluisse mortalibus sum describere cau accidentia.
                    Dixi heri ut nunc prae de odor quos vi im.Quaerendum quaecunque falsitatis ii persuaderi ei procederet.Me ipsamet sentire co admonet referam ex gi perduci.Me communibus de cogitantem ex conflantur.Halitus deludat suppono petitis im humanae et.Facit mea sonum usu fit adhuc lus.Accepit creasse brachia de corpore corpori de.Pendent hac cum sed usu minimum colores.Ingenio vim colores istarum cui equidem.
                    Eo ha diversitas perspicuum praecipuus potentiale at in sequuturum.Lucem suo aliis ullam age rerum.Tangatur ii ob convenit turbatus ad dumtaxat.Ii ac mentemque componant in ad suscipere effecerit rationale somniemus.Archimedes mo labefactat quaerendum deceperunt ad ex at.Uno veritatem aut ego solvendae argumenta excaecant. ";


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScrollableMessageBox msgBox = new ScrollableMessageBox(
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Stop,
                    "Please confirm", test + test + test,
                    false,
                new Dictionary<ScrollableMsgBoxButtonType, string>
            {
                {  ScrollableMsgBoxButtonType.OkButton, "&Ok" },
                {  ScrollableMsgBoxButtonType.CancelButton, "&Abbrechen" },
                {  ScrollableMsgBoxButtonType.YesButton, "&Ja" },
                {  ScrollableMsgBoxButtonType.NoButton, "&Nein" },
                {  ScrollableMsgBoxButtonType.AbortButton, "&Beenden" },
                {  ScrollableMsgBoxButtonType.RetryButton, "&Wiederholen" },
                {  ScrollableMsgBoxButtonType.IgnoreButton, "&Ignorieren" }
            });
            msgBox.ShowDialog();
            MessageBox.Show(msgBox.Response.ToString());
            msgBox.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            
            ScrollableMessageBox msgBox = new ScrollableMessageBox(
                MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop,
                    "Avviso", test + test + test,
                    true,
            new Dictionary<ScrollableMsgBoxButtonType, string>
            {
                {  ScrollableMsgBoxButtonType.OkButton, "&Ok" },
                {  ScrollableMsgBoxButtonType.CancelButton, "&Annulla" },
                {  ScrollableMsgBoxButtonType.YesButton, "&Sì" },
                {  ScrollableMsgBoxButtonType.NoButton, "&No" },
                {  ScrollableMsgBoxButtonType.AbortButton, "&Termina" },
                {  ScrollableMsgBoxButtonType.RetryButton, "&Riprova" },
                {  ScrollableMsgBoxButtonType.IgnoreButton, "&Ignora" }
            });

            msgBox.ShowDialog();
            MessageBox.Show(msgBox.Response.ToString());
            msgBox.Dispose();
        }
    }
}
