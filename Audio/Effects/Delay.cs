using NAudio.Wave;
using System;

namespace EchoBridge.Audio.Effects
{
    /// <summary>
    /// Simple delay/echo effect
    /// </summary>
    public class Delay : IEffect, IDisposable
    {
        private int _delayMs = 250;
        private float _feedback = 0.3f;
        private float _wetMix = 0.3f;
        private bool _isEnabled = true;

        public string Name => "Delay";
        
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public int DelayMs
        {
            get => _delayMs;
            set => _delayMs = Math.Clamp(value, 1, 2000);
        }

        public float Feedback
        {
            get => _feedback;
            set => _feedback = Math.Clamp(value, 0f, 0.9f);
        }

        public float WetMix
        {
            get => _wetMix;
            set => _wetMix = Math.Clamp(value, 0f, 1f);
        }

        public ISampleProvider Apply(ISampleProvider source)
        {
            if (!IsEnabled)
                return source;

            return new DelaySampleProvider(source, _delayMs, _feedback, _wetMix);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    internal class DelaySampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly float _feedback;
        private readonly float _wetMix;
        private readonly float[][] _delayBuffers;
        private readonly int[] _writeIndices;
        private readonly int _delayInSamples;
        private readonly int _channels;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public DelaySampleProvider(ISampleProvider source, int delayMs, float feedback, float wetMix)
        {
            _source = source;
            _feedback = feedback;
            _wetMix = wetMix;
            _channels = source.WaveFormat.Channels;

            _delayInSamples = (int)(source.WaveFormat.SampleRate * delayMs / 1000.0);
            _delayBuffers = new float[_channels][];
            _writeIndices = new int[_channels];

            for (int i = 0; i < _channels; i++)
            {
                _delayBuffers[i] = new float[_delayInSamples];
                _writeIndices[i] = 0;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                int channel = i % _channels;
                float input = buffer[offset + i];
                
                // Read from delay buffer
                float delayed = _delayBuffers[channel][_writeIndices[channel]];
                
                // Write to delay buffer with feedback
                _delayBuffers[channel][_writeIndices[channel]] = input + delayed * _feedback;
                
                // Advance write index
                _writeIndices[channel] = (_writeIndices[channel] + 1) % _delayInSamples;
                
                // Mix dry and wet signals
                buffer[offset + i] = input * (1f - _wetMix) + delayed * _wetMix;
            }

            return samplesRead;
        }
    }
}
