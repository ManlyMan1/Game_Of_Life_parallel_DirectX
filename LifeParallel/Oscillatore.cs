using System.Windows;
using System.Windows.Input;
using AudioPhysics.Provider;
using NAudio.Wave.SampleProviders;
using System.Collections.Generic;
using NAudio.Wave;
using System;
using LifeParallel;

namespace AudioPhysics
{
    /// <summary>
    /// Logica di interazione per Oscillatore.xaml
    /// </summary>
    public partial class Oscillatore
    {
        public  GeneratoreSegnale GS;
        SampleToWaveProvider SPZ;
        bool isPronto = false;
        public List<double> Valori = new List<double>(44100);
        public static WaveFormat WF = new WaveFormat(44100,2);
        int SampleRate = 44100;
        int Canali = 2;

        public Oscillatore()
        {
            Valori.Add(Funzioni.Seno(2000, WF, 44100, 0, ((100 / 100D))));
        }

        ~Oscillatore()
        {
            try { Distruggi(); }
            catch { }
        }

        [Obsolete("Deprecato")]
        public void Aggiorna()
        {
            if (isPronto) RichiediAggiornamento(GS.Tipo);
        }

        

        public void Prepara()
        {
            GS = PreparaOnda();
            SPZ = new SampleToWaveProvider(GS);
            RichiediAggiornamento(GeneratoreSegnale.TipoSegnale.Seno);
            Form1.MW.GlobalMixer.Mw.AddInputStream(SPZ);
            isPronto = true;
        }

        private GeneratoreSegnale PreparaOnda()
        {
            return new GeneratoreSegnale(SampleRate, Canali, GeneratoreSegnale.TipoSegnale.Seno,
                400, 1,1 / 100D);
        }

        public void Distruggi()
        {
            Form1.MW.GlobalMixer.Mw.RemoveInputStream(SPZ);
            SPZ = null;
        }


        private void RichiediAggiornamento(GeneratoreSegnale.TipoSegnale tipo)
        {

        }



  
    }
}
