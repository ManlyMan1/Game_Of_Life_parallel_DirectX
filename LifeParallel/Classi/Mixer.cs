using System;
using NAudio.Mixer;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AudioPhysics.Classi
{
    public class Mixer
    {
        public MixingWaveProvider32 Mw = new MixingWaveProvider32();
        DirectSoundOut Wo = new DirectSoundOut();
        public double _volume = 1;

        public void Play()
        {
            Wo.Init(Mw);
            Wo.Play();
        }

        public void Stop()
        {
            Wo.Stop();
        }

        public void Volume(float volume)
        {
            _volume = volume;
        }

    }
}
