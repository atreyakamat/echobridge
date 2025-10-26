using NAudio.Wave;
using System;

namespace EchoBridge.Audio.Effects
{
    /// <summary>
    /// Hard limiter to prevent clipping
    /// </summary>
    public class Limiter : IEffect, IDisposable
    {
        private float _threshold = -0.5f; // dB
        private bool _isEnabled = true;

        public string Name => "Limiter";
        
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public float ThresholdDb
        {
            get => _threshold;
            set => _threshold = Math.Clamp(value, -12f, 0f);
        }

        public ISampleProvider Apply(ISampleProvider source)
        {
            if (!IsEnabled)
                return source;

            return new LimiterSampleProvider(source, _threshold);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    internal class LimiterSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly float _thresholdLinear;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public LimiterSampleProvider(ISampleProvider source, float thresholdDb)
        {
            _source = source;
            _thresholdLinear = DbToLinear(thresholdDb);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                float sample = buffer[offset + i];
                
                // Hard limiting
                if (sample > _thresholdLinear)
                    buffer[offset + i] = _thresholdLinear;
                else if (sample < -_thresholdLinear)
                    buffer[offset + i] = -_thresholdLinear;
            }

            return samplesRead;
        }

        private static float DbToLinear(float db) => (float)Math.Pow(10.0, db / 20.0);
    }
}
