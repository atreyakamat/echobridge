using NAudio.Wave;
using System;

namespace EchoBridge.Audio.Effects
{
    /// <summary>
    /// Simple reverb effect using Schroeder reverberator
    /// </summary>
    public class Reverb : IEffect, IDisposable
    {
        private float _wetMix = 0.3f;
        private float _roomSize = 0.5f;
        private bool _isEnabled = true;

        public string Name => "Reverb";
        
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public float WetMix
        {
            get => _wetMix;
            set => _wetMix = Math.Clamp(value, 0f, 1f);
        }

        public float RoomSize
        {
            get => _roomSize;
            set => _roomSize = Math.Clamp(value, 0f, 1f);
        }

        public ISampleProvider Apply(ISampleProvider source)
        {
            if (!IsEnabled)
                return source;

            return new ReverbSampleProvider(source, _wetMix, _roomSize);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    internal class ReverbSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly float _wetMix;
        private readonly float[][] _delayBuffers;
        private readonly int[] _delayIndices;
        private readonly int[] _delaySizes;
        private readonly int _channels;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public ReverbSampleProvider(ISampleProvider source, float wetMix, float roomSize)
        {
            _source = source;
            _wetMix = wetMix;
            _channels = source.WaveFormat.Channels;

            // Schroeder reverb delay times in samples (scaled by room size)
            int sampleRate = source.WaveFormat.SampleRate;
            int[] baseDelays = { 1557, 1617, 1491, 1422, 1277, 1356, 1188, 1116 };
            
            _delaySizes = new int[baseDelays.Length];
            for (int i = 0; i < baseDelays.Length; i++)
            {
                _delaySizes[i] = (int)(baseDelays[i] * (0.5f + roomSize));
            }

            _delayBuffers = new float[_channels][];
            _delayIndices = new int[_channels];

            for (int ch = 0; ch < _channels; ch++)
            {
                int totalSize = 0;
                foreach (var size in _delaySizes)
                    totalSize += size;
                
                _delayBuffers[ch] = new float[totalSize];
                _delayIndices[ch] = 0;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                int channel = i % _channels;
                float input = buffer[offset + i];
                
                // Simple comb filter reverb
                float delayed = 0;
                int bufferOffset = 0;
                
                foreach (var delaySize in _delaySizes)
                {
                    int readIndex = (_delayIndices[channel] + bufferOffset) % delaySize;
                    delayed += _delayBuffers[channel][bufferOffset + readIndex] * 0.5f;
                    bufferOffset += delaySize;
                }

                delayed /= _delaySizes.Length;
                
                // Write to delay buffer with feedback
                bufferOffset = 0;
                foreach (var delaySize in _delaySizes)
                {
                    int writeIndex = (_delayIndices[channel] + bufferOffset) % delaySize;
                    _delayBuffers[channel][bufferOffset + writeIndex] = input + delayed * 0.3f;
                    bufferOffset += delaySize;
                }

                _delayIndices[channel] = (_delayIndices[channel] + 1) % _delaySizes[0];

                // Mix wet and dry
                buffer[offset + i] = input * (1f - _wetMix) + delayed * _wetMix;
            }

            return samplesRead;
        }
    }
}
