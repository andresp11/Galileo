using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Galileo
{
    public partial class Voz : System.Web.UI.Page
    {
        SpeechSynthesizer sSynth = new SpeechSynthesizer();
        PromptBuilder pBuilder = new PromptBuilder(new CultureInfo("es-MX"));
        SpeechRecognitionEngine sR = new SpeechRecognitionEngine(new CultureInfo("es-MX"));

        private String[] UNIDADES = { "", "un ", "dos ", "tres ", "cuatro ", "cinco ", "seis ", "siete ", "ocho ", "nueve " };
        private String[] DECENAS = {"diez ", "once ", "doce ", "trece ", "catorce ", "quince ", "dieciseis ",
        "diecisiete ", "dieciocho ", "diecinueve", "veinte ", "treinta ", "cuarenta ",
        "cincuenta ", "sesenta ", "setenta ", "ochenta ", "noventa "};
        private Regex r;
        static string[] numerosO = new string[104];
        static string[,] numeros = new string[104, 2];
        static string[,] momento = new string[2, 2];
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            RegisterAsyncTask(new PageAsyncTask(asyncInitilizer));            
            for (int i = 0; i < 100; i++)
            {
                numeros[i, 0] = Convertir(i.ToString(), false);
                numeros[i, 1] = i.ToString();
            }
            numeros[100, 0] = "va";
            numeros[101, 0] = "cerrar";
            numeros[102, 0] = "p";
            numeros[103, 0] = "guión";
//            numeros[104, 0] = "petres";
//            numeros[105, 0] = "pecuatro";
//            numeros[106, 0] = "pecinco";
            numeros[100, 1] = "va";
            numeros[101, 1] = "cerrar";
            numeros[102, 1] = "p";
            numeros[103, 1] = "guión";
//            numeros[104, 1] = "petres";
//            numeros[105, 1] = "pecuatro";
//            numeros[106, 1] = "pecinco";
            for (int i = 0; i < 104; i++)
                for (int j = 0; j < 103; j++)
                    if (String.Compare(numeros[j, 0], numeros[j + 1, 0]) > 0)
                    {
                        momento[0, 0] = numeros[j, 0];
                        momento[0, 1] = numeros[j, 1];
                        numeros[j , 0] = numeros[j + 1, 0];
                        numeros[j , 1] = numeros[j + 1, 1];
                        numeros[j + 1, 0] = momento[0, 0];
                        numeros[j + 1, 1] = momento[0, 1];
                    }
            for (int i = 0; i < 104; i++)
                numerosO[i] = numeros[i, 1];

            Choices sList = new Choices();
            sList.Add(numerosO);
            Grammar gr = new Grammar(new GrammarBuilder(sList));
            try
            {
                sR.RequestRecognizerUpdate();
                sR.LoadGrammar(gr);
                sR.SpeechRecognized += sR_SpeechRecognized;
                sR.MaxAlternates = 1;
                sR.SetInputToDefaultAudioDevice();
                //                sR.EndSilenceTimeout = System.TimeSpan.FromSeconds(1);

            }
            catch (Exception e1)
            {
                //MessageBox.Show(e1.ToString());
            }


        }

        protected void imgAgregar_Click(object sender, ImageClickEventArgs e)
        {
            imgIniciar.Enabled = false;
            imgTerminar.Enabled = true;
            try
            {
                sR.RequestRecognizerUpdate();
                sR.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception e1)
            {
//                MessageBox.Show(e1.ToString());
            }
        }

        public void lector(Object sender, SpeechRecognizedEventArgs e)
        {
            foreach (RecognizedWordUnit palabra in e.Result.Words)
            {
                Label1.Text = palabra.Text;
//                if(palabra.Text = "uno")
//                    Label1.Text += "ok";
            }
        }

        private void sR_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //            
            //            sR.RequestRecognizerUpdate();
            //            sR.SpeechRecognized += null;
            //            if (e.Result.Text != "va")
            //            {
            string texto = "";
            if (e.Result.Text == "cerrar")
                Terminar_Click(sender, null);
            else if (e.Result.Text == "va")
                texto = Environment.NewLine;
            else if (e.Result.Text == "guión")
                texto = "-";
            else
                texto = e.Result.Text.ToString();
            //                sR.RecognizeAsyncStop();
            txtEscribir.Text = txtEscribir.Text + texto;
            //            }
        }

        private void Terminar_Click(object sender, EventArgs e)
        {
            sR.RecognizeAsyncStop();
            imgIniciar.Enabled = true;
            imgTerminar.Enabled = false;
        }



        private String getUnidades(String numero)
        {   // 1 - 9            
            //si tuviera algun 0 antes se lo quita -> 09 = 9 o 009=9
            String num = numero.Substring(numero.Length - 1);
            return UNIDADES[int.Parse(num)];
        }

        private String getDecenas(String num)
        {// 99                        
            int n = int.Parse(num);
            if (n < 10)
            {//para casos como -> 01 - 09
                return getUnidades(num);
            }
            else if (n > 19)
            {//para 20...99
                String u = getUnidades(num);
                if (u.Equals(""))
                { //para 20,30,40,50,60,70,80,90
                    return DECENAS[int.Parse(num.Substring(0, 1)) + 8];
                }
                else
                {
                    return DECENAS[int.Parse(num.Substring(0, 1)) + 8] + "y " + u;
                }
            }
            else
            {//numeros entre 11 y 19
                return DECENAS[n - 10];
            }
        }

        public String Convertir(String numero, bool mayusculas)
        {

            string literal;
            if (int.Parse(numero) == 0)
            {//si el valor es cero                
                literal = "cero ";
            }
            else if (int.Parse(numero) > 9)
            {//si es decena
                literal = getDecenas(numero);
            }
            else
            {//sino unidades -> 9
                literal = getUnidades(numero);
            }
            //devuelve el resultado en mayusculas o minusculas
            if (mayusculas)
            {
                return (literal).ToUpper();
            }
            else
            {
                return (literal);
            }
        }

        protected void imgHablar_Click(object sender, ImageClickEventArgs e)
        {
            txtHablar.Text = "1. ¿Cuál es el número de cuartos, piezas o habitaciones, con que cuenta tu hogar?(no incluir baños, pasillos, patios, etc.)\n";
            txtHablar.Text += "2. ¿Cuántos baños completos hay para los integrantes de la casa?\n";
            txtHablar.Text += "3. ¿En tu hogar cuentas con regaderas funcionando en alguno de los baños?\n";
            txtHablar.Text += "4. ¿Contando todos los focos que utilizas para iluminar tu hogar (interiores y exteriores)¿Cuántos focos tienes en tu hogar?\n";
            txtHablar.Text += "5. El piso de tu casa es predominanatemente de tierra o de cemento o algún otro tipo de material?\n";
            txtHablar.Text += "6. ¿Cuántos automóviles tienes en tu hogar?\n";
            txtHablar.Text += "7. ¿En tu hogar tienen estufa de gas o eléctrica?\n";
            txtHablar.Text += "8. Pensando en la persona que aporta la mayor parte del ingreso en tu hogar ¿Cuál es su escolaridad?\n";
            txtHablar.Text += "9. ¿Qué edad tienes?";


            if (sSynth == null)
            {
                sSynth = new SpeechSynthesizer();
                asyncInitilizer();

            }


            pBuilder.ClearContent();
            pBuilder.AppendText(txtHablar.Text);
            sSynth.Speak(pBuilder);

        }

        protected void imgTerminar_Click(object sender, ImageClickEventArgs e)
        {
            sR.RecognizeAsyncStop();
            imgIniciar.Enabled = true;
            imgTerminar.Enabled = false;
        }

        private static async Task asyncInitilizer()
        {
            await Task.Run(() =>
            {

                try
                {
                    sSynth.SetOutputToDefaultAudioDevice();
                    sSynth.SelectVoiceByHints(VoiceGender.Female);
                    sSynth.Rate = -1;
                    sSynth.VisemeReached += synth_VisemeReached;
                    sSynth.SpeakStarted += synth_SpeakStarted;
                    sSynth.SpeakCompleted += synth_SpeakCompleted;
                }
                catch (Exception ex)
                {
//                    MessageSender.sendExceptionMessage(ex.ToString());
                }
            });
        }*/
        }  
    }
}

