using NAudio.Wave;
using System;

namespace EchoBridge.Audio
{
    /// <summary>
    /// Captures system audio output using WASAPI loopback mode
    /// </summary>
    public class LoopbackCapture : IDisposable
    {
        private WasapiLoopbackCapture? _capture;
        private bool _isRecording;

        public event EventHandler<WaveInEventArgs>? DataAvailable;
        public event EventHandler<StoppedEventArgs>? RecordingStopped;

        public WaveFormat? WaveFormat => _capture?.WaveFormat;
        public bool IsRecording => _isRecording;

        public void StartRecording()
        {
            if (_isRecording)
                return;

            try
            {
                _capture = new WasapiLoopbackCapture();
                _capture.DataAvailable += OnDataAvailable;
                _capture.RecordingStopped += OnRecordingStopped;
                _capture.StartRecording();
                _isRecording = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to start loopback capture", ex);
            }
        }

        public void StopRecording()
        {
            if (!_isRecording || _capture == null)
                return;

            _capture.StopRecording();
            _isRecording = false;
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            DataAvailable?.Invoke(this, e);
        }

        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            _isRecording = false;
            RecordingStopped?.Invoke(this, e);
        }

        public void Dispose()
        {
            StopRecording();
            if (_capture != null)
            {
                _capture.DataAvailable -= OnDataAvailable;
                _capture.RecordingStopped -= OnRecordingStopped;
                _capture.Dispose();
                _capture = null;
            }
        }
    }
}
