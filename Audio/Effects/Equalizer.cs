using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;
using System;

namespace EchoBridge.Audio.Effects
{
    /// <summary>
    /// 3-band parametric equalizer using biquad filters
    /// </summary>
    public class Equalizer : IEffect, IDisposable
    {
        private float _lowGain = 0f;      // dB
        private float _midGain = 0f;      // dB
        private float _highGain = 0f;     // dB
        private bool _isEnabled = true;

        public string Name => "Equalizer";
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public float LowGain
        {
            get => _lowGain;
            set => _lowGain = Math.Clamp(value, -12f, 12f);
        }

        public float MidGain
        {
            get => _midGain;
            set => _midGain = Math.Clamp(value, -12f, 12f);
        }

        public float HighGain
        {
            get => _highGain;
            set => _highGain = Math.Clamp(value, -12f, 12f);
        }

        public ISampleProvider Apply(ISampleProvider source)
        {
            if (!IsEnabled)
                return source;

            // Apply low shelf filter (80 Hz)
            var lowShelf = new EqualizerSampleProvider(source, 80, _lowGain, 1.0f);
            
            // Apply mid peak filter (1000 Hz)
            var midPeak = new EqualizerSampleProvider(lowShelf, 1000, _midGain, 1.0f);
            
            // Apply high shelf filter (8000 Hz)
            var highShelf = new EqualizerSampleProvider(midPeak, 8000, _highGain, 1.0f);

            return highShelf;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Custom equalizer sample provider using biquad filters
    /// </summary>
    internal class EqualizerSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly BiQuadFilter[] _filters;
        private readonly int _channels;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public EqualizerSampleProvider(ISampleProvider source, float frequency, float gainDb, float q)
        {
            _source = source;
            _channels = source.WaveFormat.Channels;
            _filters = new BiQuadFilter[_channels];

            for (int i = 0; i < _channels; i++)
            {
                if (frequency < 200)
                {
                    // Low shelf
                    _filters[i] = BiQuadFilter.LowShelf(source.WaveFormat.SampleRate, frequency, q, gainDb);
                }
                else if (frequency > 5000)
                {
                    // High shelf
                    _filters[i] = BiQuadFilter.HighShelf(source.WaveFormat.SampleRate, frequency, q, gainDb);
                }
                else
                {
                    // Peaking EQ
                    _filters[i] = BiQuadFilter.PeakingEQ(source.WaveFormat.SampleRate, frequency, q, gainDb);
                }
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                int channel = i % _channels;
                buffer[offset + i] = _filters[channel].Transform(buffer[offset + i]);
            }

            return samplesRead;
        }
    }
}
