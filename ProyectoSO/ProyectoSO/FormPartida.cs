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

        

    }



}
