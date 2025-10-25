using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace EchoBridge.Audio
{
    /// <summary>
    /// Represents a single output device with its own buffer, playback stream, and FX chain
    /// </summary>
    public class OutputDevice : IDisposable
    {
        public string DeviceName { get; }
        public int DeviceNumber { get; }
        public FxChain FxChain { get; }
        
        private WasapiOut? _output;
        private BufferedWaveProvider? _buffer;
        private VolumeSampleProvider? _volumeProvider;
        private bool _isPlaying;
        
        private float _volume = 1.0f;
        private int _delayMs = 0;

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Math.Clamp(value, 0f, 1f);
                if (_volumeProvider != null)
                    _volumeProvider.Volume = _volume;
            }
        }

        public int DelayMs
        {
            get => _delayMs;
            set => _delayMs = Math.Max(0, value);
        }

        public bool IsPlaying => _isPlaying;

        public OutputDevice(string deviceName, int deviceNumber)
        {
            DeviceName = deviceName;
            DeviceNumber = deviceNumber;
            FxChain = new FxChain();
        }

        public void Initialize(WaveFormat format)
        {
            try
            {
                // Create buffer for incoming audio
                _buffer = new BufferedWaveProvider(format)
                {
                    BufferLength = format.AverageBytesPerSecond * 2, // 2 seconds buffer
                    DiscardOnBufferOverflow = true
                };

                // Convert to sample provider for FX processing
                var sampleProvider = _buffer.ToSampleProvider();
                
                // Apply FX chain
                var processedProvider = FxChain.Apply(sampleProvider);
                
                // Add volume control
                _volumeProvider = new VolumeSampleProvider(processedProvider)
                {
                    Volume = _volume
                };

                // Initialize WASAPI output
                _output = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 50);
                _output.Init(_volumeProvider);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize output device '{DeviceName}'", ex);
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _buffer?.AddSamples(buffer, offset, count);
        }

        public void Play()
        {
            if (_isPlaying || _output == null)
                return;

            _output.Play();
            _isPlaying = true;
        }

        public void Pause()
        {
            if (!_isPlaying || _output == null)
                return;

            _output.Pause();
            _isPlaying = false;
        }

        public void Stop()
        {
            if (_output == null)
                return;

            _output.Stop();
            _isPlaying = false;
        }

        public void Dispose()
        {
            Stop();
            _output?.Dispose();
            _output = null;
            _buffer = null;
            FxChain.Dispose();
        }
    }
}
