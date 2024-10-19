using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parchís
{
    public partial class FormPartida : Form
    {
        int allowed_to_paint = 0;
        int permitido_pintar = 0;
        double centroX = 0;
        double centroY = 0;



        public FormPartida()
        {
            InitializeComponent();
            medioCentro.Paint += new PaintEventHandler(medioCentro_Paint);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ajuste de los margenes y tablero
            margenSuperior.Height = Convert.ToInt32(Convert.ToDouble(gamePanel.Height) * 0.05);
            margenIzquierdo.Width = Convert.ToInt32(Convert.ToDouble(gamePanel.Width) * 0.05);
            panelCasillas.Width = panelCasillas.Height;

            //ajuste inicial de filas
            fila1.Height = Convert.ToInt32(Convert.ToDouble(panelCasillas.Height) * 0.30);
            fila2.Height = Convert.ToInt32(Convert.ToDouble(panelCasillas.Height) * 0.30);
            fila3.Height = Convert.ToInt32(Convert.ToDouble(panelCasillas.Height) * 0.30);

            //fila1
            baseRoja.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            casillasSuperiores.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            baseAzul.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);

            //ajuste de las columnas de las casillas superiores
            //columna1
            casillaSuperior1.Width = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Width) * 1 / 3);
            casilla35.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla36.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla37.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla38.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla39.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla40.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla41.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);

            //columna2
            casillaSuperior2.Width = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Width) * 1 / 3);
            casilla34.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr1.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr2.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr3.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr4.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr5.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr6.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);

            //columna3
            casillaSuperior3.Width = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Width) * 1 / 3);
            casilla33.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla32.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla31.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla30.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla29.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla28.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla27.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);



            //fila2
            medioIzquierda.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            medioCentro.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            medioDerecha.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);

            //fila2 - filaizquierda1
            casillaIzquierda1.Height = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Height) * 1 / 3);
            casilla44.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla45.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla46.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla47.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla48.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla49.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla50.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);

            //fila2 - filaizquierda2
            casillaIzquierda2.Height = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Height) * 1 / 3);
            casilla51.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv1.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv2.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv3.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv4.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv5.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv6.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);

            //fila2 - filaizquierda3
            casillaIzquierda3.Height = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Height) * 1 / 3);
            casilla52.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla53.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla54.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla55.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla56.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla57.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla58.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);

            //FILAS CENTRALES
            centroSuperior.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7);
            centroInferior.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7);
            centroIzquierda.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 7);
            centroDerecha.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 7);
            //DIBUJAR GEOMETRIAS (VERTICES TRIANGULARES)
            casilla42.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla_mr7.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla26.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);

            casilla60.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla_ma7.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla8.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);

            casilla43.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));
            casilla_mv7.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3);
            casilla59.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));

            casilla25.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));
            casilla_mb7.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3);
            casilla9.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));



            //fila2 - filaderecha1
            casillaDerecha1.Height = Convert.ToInt32(Convert.ToDouble(medioDerecha.Height) * 1 / 3);
            casilla24.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla23.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla22.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla21.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla20.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla19.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla18.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);

            //fila2 - filaderecha2
            casillaDerecha2.Height = Convert.ToInt32(Convert.ToDouble(medioDerecha.Height) * 1 / 3);
            casilla17.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb1.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb2.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb3.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb4.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb5.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb6.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);

            //fila2 - filaderecha3
            casillaDerecha3.Height = Convert.ToInt32(Convert.ToDouble(medioDerecha.Height) * 1 / 3);
            casilla10.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla11.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla12.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla13.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla14.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla15.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla16.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);


            //fila3
            baseVerde.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            casillasInferiores.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            baseAmarilla.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);

            //ajuste de las columnas de las casillas inferiores
            //columna1
            casillaInferior1.Width = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Width) * 1/3);
            casilla61.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla62.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla63.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla64.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla65.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla66.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla67.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);



            //columna2
            casillaInferior2.Width = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Width) * 1/3);
            casilla68.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma1.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma2.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma3.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma4.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma5.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma6.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);


            //columna3
            casillaInferior3.Width = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Width) * 1/3);
            casilla7.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla6.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla5.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla4.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla3.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla2.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla1.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);



            
            allowed_to_paint = 1;
            medioCentro.Invalidate();
            medioCentro.Refresh();
            gamePanel.Visible = true;
        }



        private void medioCentro_Paint(object sender, PaintEventArgs e)
        {
            if (allowed_to_paint == 1)
            {

                using (Pen pen = new Pen(Color.Gray, 4)) // Grosor de 2 píxeles
                {
                    // Obtener el objeto Graphics del evento Paint
                    Graphics g = e.Graphics;

                    // Dibujar una línea diagonal desde (0,0) hasta (Width, Height)
                    g.DrawLine(pen, 0, 0, medioCentro.Width, medioCentro.Height);
                    g.DrawLine(pen, medioCentro.Width, 0, 0, medioCentro.Height);

                }

                //TRIANGULO ROJO
                Point[] puntos = new Point[]
                {
                    new Point(medioCentro.Width/7, medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Height/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.Red)) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos);
                }

                //TRIANGULO AMARILLO
                Point[] puntos2 = new Point[]
                {
                    new Point(medioCentro.Width/7, medioCentro.Height - medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Height - medioCentro.Height/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos2);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 255, 192))) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos2);
                }

                //TRIANGULO VERDE
                Point[] puntos3 = new Point[]
                {
                    new Point(medioCentro.Width/7, medioCentro.Height - medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width/7, medioCentro.Width/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos3);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 128))) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos3);
                }

                //TRIANGULO AZUL
                Point[] puntos4 = new Point[]
                {
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Height - medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Width/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos4);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.Blue)) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos4);
                }
            }
        }

        private void turnoJugador(int numeroJugador)
        {
            if (numeroJugador == 1)
            {
                turno_P1.Visible = true;
            }
            else if (numeroJugador == 2)
            {
                turno_P2.Visible = true;
            }
            else if (numeroJugador == 3)
            {
                turno_P3.Visible = true;
            }
            else if (numeroJugador == 4)
            {
                turno_P4.Visible = true;
            }
        }

        private void terminarTurnos()
        {
            turno_P1.Visible = false;
            turno_P2.Visible = false;
            turno_P3.Visible = false;
            turno_P4.Visible = false;
        }

        private double[] obtenerPosicion(int numeroCasilla)
        {
            double posX = 0;
            double posY = 0;

            if (((numeroCasilla >= 1) && (numeroCasilla <= 7)) || ((numeroCasilla >= 61) && (numeroCasilla <= 68)) || ((numeroCasilla >= 27) && (numeroCasilla <= 41)))
            {
                posX = casilla1.Width / 4;
                posY = casilla1.Height / 2;
            }
            else if (((numeroCasilla >= 44) && (numeroCasilla <= 58)) || ((numeroCasilla >= 10) && (numeroCasilla <= 24)))
            {
                posX = casilla1.Width / 2;
                posY = casilla1.Height / 4;
            }

            double[] result = {posX, posY};

            return result;
        }

        private bool dibujarFicha(string numeroCasilla, string color)
        {
            try
            {
                double[] result = obtenerPosicion(Convert.ToInt32(numeroCasilla));
                double centroX = result[0];
                double centroY = result[1];
            }
            catch
            {

            }
            
            string nombreCasillaADibujar = "casilla" + numeroCasilla;

            // Buscar el control por su nombre
            Control[] mapaCasillas = this.Controls.Find(nombreCasillaADibujar, true);

            if (mapaCasillas.Length > 0 && mapaCasillas[0] is Panel)
            {
                Panel miCasilla = (Panel)mapaCasillas[0];
                // Cambiar el color de fondo del panel
                miCasilla.BackColor = Color.Gray;
                miCasilla.Refresh();
            }

            return true;

        }

        private void messageSenderButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Host unreachable");
            FinalizarCasillas("1");
            string numeroCasilla = chatInputTextBox.Text;

            double[] result = obtenerPosicion(Convert.ToInt32(numeroCasilla));
            centroX = result[0];
            centroY = result[1];


            InicializarCasillas(numeroCasilla);
            dibujarFicha(numeroCasilla, "r");
            

            //FinalizarCasillas(numeroCasilla);
           
        }

        private void InicializarCasillas(string sufijo_casilla)
        {
            // Bucle para recorrer los paneles numerados de "panel1" a "panel30"
            // Formar el nombre del panel dinámicamente
            string nombrePanel = "casilla" + sufijo_casilla;

            // Buscar el panel por su nombre
            Control[] paneles = this.Controls.Find(nombrePanel, true);

            if (paneles.Length > 0 && paneles[0] is Panel)
            {
                Panel panel = (Panel)paneles[0];
                // Suscribir el evento Paint a cada panel encontrado
                panel.Paint += Panel_Paint;
            }
           
        }

        private void FinalizarCasillas(string sufijo_casilla)
        {
            for (int i = 0; i < 64; i++)
            {
                // Bucle para recorrer los paneles numerados de "panel1" a "panel30"
                // Formar el nombre del panel dinámicamente
                string nombrePanel = "casilla" + i;

                // Buscar el panel por su nombre
                Control[] paneles = this.Controls.Find(nombrePanel, true);

                if (paneles.Length > 0 && paneles[0] is Panel)
                {
                    Panel panel = (Panel)paneles[0];
                    // Suscribir el evento Paint a cada panel encontrado
                    panel.Paint -= Panel_Paint;
                    panel.Refresh();
                }
            }

        }

        // Método que dibuja dentro del panel, llamado por el evento Paint
        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            if (1==1)
            {
                MessageBox.Show("HOLI");

                Panel panel = sender as Panel;  // Obtener el panel que disparó el evento

                if (panel != null)
                {
                    Graphics g = e.Graphics;

                    // Dibujar una elipse centrada dentro del panel
                    int width = panel.Width;
                    int height = panel.Height;

                    // Definir el tamaño y la posición de la elipse
                    int diameter = Math.Min(width, height)-10;  // Restar margen de 5 píxeles en cada lado
                    int x = Convert.ToInt32(centroX - diameter/2);
                    int y = Convert.ToInt32(centroY - diameter/2);

                    // Dibujar un círculo con lápiz negro
                    SolidBrush pincel = new SolidBrush(Color.Red);
                    g.FillEllipse(pincel, x, y, diameter, diameter);
                    g.DrawEllipse(Pens.Red, x, y, diameter, diameter);
                    
                }
            }
        }
    }



}
