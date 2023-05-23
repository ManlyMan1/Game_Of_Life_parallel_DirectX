using NAudio.Wave;
using System;

namespace AudioPhysics.Provider
{
    /// <summary>
    /// Classe base che fornisce calcoli per generatore di onde
    /// Grazie wikipedia english: https://en.wikipedia.org/wiki/Waveform :P
    /// </summary>
    class Funzioni
    {
        public const double DoppioPi = 2 * Math.PI;
        private static readonly Random casuale = new Random();

        public static double Seno(double Frequenza, WaveFormat FormatoWave, double nSample, double Sfasamento, double Volume)
        {
            var Multiplo = DoppioPi * Frequenza / FormatoWave.SampleRate;
            return Volume * Math.Sin((nSample * Multiplo) + Sfasamento);
        }

        public static double Coseno(double Frequenza, WaveFormat FormatoWave, double nSample, double Sfasamento, double Volume)
        {
            var Multiplo = DoppioPi * Frequenza / FormatoWave.SampleRate;
            return Volume * Math.Cos((nSample * Multiplo) + Sfasamento);
        }

        public static double Triangolo(double Frequenza, WaveFormat FormatoWave, double nSample, double Sfasamento, double Volume)
        {
            var Multiplo = 2 * Frequenza / FormatoWave.SampleRate;
            var SampleSec = (((nSample * Multiplo) + Transforma(Sfasamento)) % 2);
            var ValoreSample = 2 * SampleSec;
            if (ValoreSample > 1)
                ValoreSample = 2 - ValoreSample;
            if (ValoreSample < -1)
                ValoreSample = -2 - ValoreSample;
            ValoreSample *= Volume;
            return ValoreSample;
        }

        public static double Quadro(double Frequenza, WaveFormat FormatoWave, double nSample, double Sfasamento, double Volume)
        {
            var Multiplo = 2 * Frequenza / FormatoWave.SampleRate;
            var SampleSec = (((nSample * Multiplo) + Transforma(Sfasamento)) % 2) - 1;
            return SampleSec > 0 ? Volume : -Volume;
        }

        public static double DenteDiSega(double Frequenza, WaveFormat FormatoWave, double nSample, double Sfasamento,double Volume)
        {
            var Multiplo = 2 * Frequenza / FormatoWave.SampleRate;
            var SampleSec = (((nSample * Multiplo) + Transforma(Sfasamento)) % 2) - 1;
            return Volume * SampleSec;

        }

        public static double Casuale(double Volume)
        {
            return (Volume * Randomico());
        }

        private static double Randomico()
        {
            return 2 * casuale.NextDouble() - 1; //Va da -1 a 1
        }

        private static double Transforma(double Numero)
        {
            return (Numero / Math.PI) + 2; //Premio nobel per la pseudo-precisione
        }
    }
}
