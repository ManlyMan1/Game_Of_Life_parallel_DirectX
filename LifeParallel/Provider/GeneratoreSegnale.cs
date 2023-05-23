using NAudio.Wave;

namespace AudioPhysics.Provider
{
    public class GeneratoreSegnale : ISampleProvider
    {

        private readonly WaveFormat FormatoWave;
        private int nSample;

        public double Frequenza { get; set; }
        public double Volume { get; set; }
        public double Sfasamento { get; set; }
        public TipoSegnale Tipo { get; set; }
        public WaveFormat WaveFormat { get { return FormatoWave; } }

        public GeneratoreSegnale() : this(44100,2) { }

        public GeneratoreSegnale(int SampleRate, int Canale)
        {
            FormatoWave = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Canale);
            Tipo = TipoSegnale.Seno;
            Frequenza = 440.0;
            Sfasamento = 0;
            Volume = 1;
        }

        public GeneratoreSegnale(int SampleRate, int Canale, TipoSegnale _tipo, double _frequenza, double _sfasamento, double _volume)
        {
            FormatoWave = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Canale);
            Tipo = _tipo;
            Frequenza = _frequenza;
            Sfasamento = _sfasamento;
            Volume = _volume;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int outIndex = offset;
            double ValoreSample;

            for (int sampleCount = 0; sampleCount < count / FormatoWave.Channels; sampleCount++)
            {
                switch (Tipo)
                {
                    case TipoSegnale.Seno:
                        ValoreSample = Funzioni.Seno(Frequenza, FormatoWave, nSample, Sfasamento, Volume);
                        nSample++;
                        break;

                    case TipoSegnale.Coseno:
                        ValoreSample = Funzioni.Coseno(Frequenza, FormatoWave, nSample, Sfasamento, Volume);
                        nSample++;
                        break;

                    case TipoSegnale.Quadro:
                        ValoreSample = Funzioni.Quadro(Frequenza, FormatoWave, nSample, Sfasamento, Volume);
                        nSample++;
                        break;

                    case TipoSegnale.Triangolo:
                        ValoreSample = Funzioni.Triangolo(Frequenza, FormatoWave, nSample, Sfasamento, Volume);
                        nSample++;
                        break;

                    case TipoSegnale.DenteDiSega:
                        ValoreSample = Funzioni.DenteDiSega(Frequenza, FormatoWave, nSample, Sfasamento, Volume);
                        nSample++;
                        break;

                    case TipoSegnale.Casuale:
                        ValoreSample = Funzioni.Casuale(Volume);
                        break;

                    default:
                        ValoreSample = 0.0;
                        break;
                }
                for (int i = 0; i < FormatoWave.Channels; i++)
                {
                    buffer[outIndex++] = ((float)100 * (float)ValoreSample) / (float)1;
                }
            }
            return count;
        }

        public enum TipoSegnale
        {
            Seno,           //Sin
            Coseno,         //Cos
            Quadro,         //Square
            Triangolo,      //Triangle
            DenteDiSega,    //Sawtooth
            Casuale         //Random
        }
    }
}
