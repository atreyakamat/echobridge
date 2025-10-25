using NAudio.Wave;
using NAudio.Dsp;
using System;

namespace EchoBridge.Audio.Effects
{
    /// <summary>
    /// Bass boost effect using a low-shelf filter
    /// </summary>
    public class BassBoost : IEffect, IDisposable
    {
        private float _gainDb = 0f;
        private float _frequency = 100f;
        private bool _isEnabled = true;

        public string Name => "Bass Boost";
        
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public float GainDb
        {
            get => _gainDb;
            set => _gainDb = Math.Clamp(value, 0f, 12f);
        }

        public float Frequency
        {
            get => _frequency;
            set => _frequency = Math.Clamp(value, 20f, 250f);
        }

        public ISampleProvider Apply(ISampleProvider source)
        {
            if (!IsEnabled || _gainDb <= 0)
                return source;

            return new BassBoostSampleProvider(source, _frequency, _gainDb);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    internal class BassBoostSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly BiQuadFilter[] _filters;
        private readonly int _channels;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public BassBoostSampleProvider(ISampleProvider source, float frequency, float gainDb)
        {
            _source = source;
            _channels = source.WaveFormat.Channels;
            _filters = new BiQuadFilter[_channels];

            for (int i = 0; i < _channels; i++)
            {
                _filters[i] = BiQuadFilter.LowShelf(source.WaveFormat.SampleRate, frequency, 0.707f, gainDb);
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
