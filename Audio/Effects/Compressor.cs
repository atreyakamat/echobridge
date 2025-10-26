using NAudio.Wave;
using System;

namespace EchoBridge.Audio.Effects
{
    /// <summary>
    /// Dynamic range compressor
    /// </summary>
    public class Compressor : IEffect, IDisposable
    {
        private float _threshold = -20f;    // dB
        private float _ratio = 4f;          // 4:1
        private float _attack = 5f;         // ms
        private float _release = 50f;       // ms
        private float _makeupGain = 0f;     // dB
        private bool _isEnabled = true;

        public string Name => "Compressor";
        
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public float Threshold
        {
            get => _threshold;
            set => _threshold = Math.Clamp(value, -60f, 0f);
        }

        public float Ratio
        {
            get => _ratio;
            set => _ratio = Math.Clamp(value, 1f, 20f);
        }

        public float AttackMs
        {
            get => _attack;
            set => _attack = Math.Clamp(value, 0.1f, 100f);
        }

        public float ReleaseMs
        {
            get => _release;
            set => _release = Math.Clamp(value, 10f, 1000f);
        }

        public float MakeupGainDb
        {
            get => _makeupGain;
            set => _makeupGain = Math.Clamp(value, 0f, 24f);
        }

        public ISampleProvider Apply(ISampleProvider source)
        {
            if (!IsEnabled)
                return source;

            return new CompressorSampleProvider(source, _threshold, _ratio, _attack, _release, _makeupGain);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    internal class CompressorSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly float _threshold;
        private readonly float _ratio;
        private readonly float _attackCoeff;
        private readonly float _releaseCoeff;
        private readonly float _makeupGain;
        private readonly float[] _envelope;
        private readonly int _channels;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public CompressorSampleProvider(ISampleProvider source, float thresholdDb, float ratio, 
            float attackMs, float releaseMs, float makeupGainDb)
        {
            _source = source;
            _threshold = DbToLinear(thresholdDb);
            _ratio = ratio;
            _makeupGain = DbToLinear(makeupGainDb);
            _channels = source.WaveFormat.Channels;
            _envelope = new float[_channels];

            int sampleRate = source.WaveFormat.SampleRate;
            _attackCoeff = (float)Math.Exp(-1.0 / (sampleRate * attackMs / 1000.0));
            _releaseCoeff = (float)Math.Exp(-1.0 / (sampleRate * releaseMs / 1000.0));
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                int channel = i % _channels;
                float input = buffer[offset + i];
                float inputAbs = Math.Abs(input);

                // Envelope follower
                float coeff = inputAbs > _envelope[channel] ? _attackCoeff : _releaseCoeff;
                _envelope[channel] = coeff * _envelope[channel] + (1f - coeff) * inputAbs;

                // Calculate gain reduction
                float gain = 1f;
                if (_envelope[channel] > _threshold)
                {
                    float excess = _envelope[channel] / _threshold;
                    float excessDb = LinearToDb(excess);
                    float gainReductionDb = excessDb * (1f - 1f / _ratio);
                    gain = DbToLinear(-gainReductionDb);
                }

                // Apply compression and makeup gain
                buffer[offset + i] = input * gain * _makeupGain;
            }

            return samplesRead;
        }

        private static float DbToLinear(float db) => (float)Math.Pow(10.0, db / 20.0);
        private static float LinearToDb(float linear) => 20f * (float)Math.Log10(linear);
    }
}
