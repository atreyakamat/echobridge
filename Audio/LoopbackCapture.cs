using NAudio.Wave;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoBridge.Audio
{
    /// <summary>
    /// Captures system audio output using WASAPI loopback mode
    /// </summary>
    public class LoopbackCapture : IDisposable
    {
        private WasapiLoopbackCapture? _capture;
        private bool _isRecording;
        private MMDevice? _selectedDevice;

        public event EventHandler<WaveInEventArgs>? DataAvailable;
        public event EventHandler<StoppedEventArgs>? RecordingStopped;

        public WaveFormat? WaveFormat => _capture?.WaveFormat;
        public bool IsRecording => _isRecording;

        /// <summary>
        /// Get all available audio capture devices
        /// </summary>
        public static List<CaptureDeviceInfo> GetCaptureDevices()
        {
            var devices = new List<CaptureDeviceInfo>();
            var enumerator = new MMDeviceEnumerator();
            
            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                devices.Add(new CaptureDeviceInfo
                {
                    Id = device.ID,
                    FriendlyName = device.FriendlyName,
                    IsDefault = device.ID == enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).ID
                });
            }

            return devices;
        }

        public void StartRecording(string? deviceId = null)
        {
            if (_isRecording)
                return;

            try
            {
                if (!string.IsNullOrEmpty(deviceId))
                {
                    // Use specific device
                    var enumerator = new MMDeviceEnumerator();
                    _selectedDevice = enumerator.GetDevice(deviceId);
                    _capture = new WasapiLoopbackCapture(_selectedDevice);
                }
                else
                {
                    // Use default device
                    _capture = new WasapiLoopbackCapture();
                }

                _capture.DataAvailable += OnDataAvailable;
                _capture.RecordingStopped += OnRecordingStopped;
                _capture.StartRecording();
                _isRecording = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to start loopback capture: " + ex.Message, ex);
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
            _selectedDevice?.Dispose();
        }
    }

    public class CaptureDeviceInfo
    {
        public string Id { get; set; } = string.Empty;
        public string FriendlyName { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
