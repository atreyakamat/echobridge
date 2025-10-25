using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoBridge.Audio
{
    /// <summary>
    /// Manages global audio routing from system loopback to multiple output devices
    /// </summary>
    public class AudioRouter : IDisposable
    {
        private readonly List<OutputDevice> _devices;
        private LoopbackCapture? _capture;
        private bool _isRunning;

        public IReadOnlyList<OutputDevice> Devices => _devices.AsReadOnly();
        public bool IsRunning => _isRunning;

        public event EventHandler<string>? StatusChanged;
        public event EventHandler<Exception>? ErrorOccurred;

        public AudioRouter()
        {
            _devices = new List<OutputDevice>();
        }

        /// <summary>
        /// Enumerates available output devices on the system
        /// </summary>
        public static List<DeviceInfo> GetAvailableDevices()
        {
            var devices = new List<DeviceInfo>();
            
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var caps = WaveOut.GetCapabilities(i);
                devices.Add(new DeviceInfo
                {
                    DeviceNumber = i,
                    DeviceName = caps.ProductName,
                    Channels = caps.Channels
                });
            }

            return devices;
        }

        public void AddDevice(OutputDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            if (_devices.Any(d => d.DeviceNumber == device.DeviceNumber))
                throw new InvalidOperationException($"Device {device.DeviceName} is already added");

            _devices.Add(device);
            StatusChanged?.Invoke(this, $"Added device: {device.DeviceName}");
        }

        public void RemoveDevice(OutputDevice device)
        {
            if (_devices.Remove(device))
            {
                device.Stop();
                device.Dispose();
                StatusChanged?.Invoke(this, $"Removed device: {device.DeviceName}");
            }
        }

        public void Start()
        {
            if (_isRunning)
                return;

            if (_devices.Count == 0)
                throw new InvalidOperationException("No output devices configured");

            try
            {
                // Initialize loopback capture
                _capture = new LoopbackCapture();
                _capture.DataAvailable += OnAudioDataAvailable;
                _capture.RecordingStopped += OnRecordingStopped;

                // Initialize all output devices with the capture format
                var format = _capture.WaveFormat ?? throw new InvalidOperationException("Failed to get wave format");
                
                foreach (var device in _devices)
                {
                    device.Initialize(format);
                    device.Play();
                }

                // Start capturing
                _capture.StartRecording();
                _isRunning = true;
                StatusChanged?.Invoke(this, "Audio routing started");
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
                Stop();
                throw;
            }
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _capture?.StopRecording();

            foreach (var device in _devices)
            {
                device.Stop();
            }

            _isRunning = false;
            StatusChanged?.Invoke(this, "Audio routing stopped");
        }

        private void OnAudioDataAvailable(object? sender, WaveInEventArgs e)
        {
            // Distribute captured audio to all output devices
            foreach (var device in _devices)
            {
                try
                {
                    device.Write(e.Buffer, 0, e.BytesRecorded);
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, new Exception($"Error writing to device {device.DeviceName}", ex));
                }
            }
        }

        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                ErrorOccurred?.Invoke(this, e.Exception);
            }
            _isRunning = false;
        }

        public void Dispose()
        {
            Stop();
            
            foreach (var device in _devices)
            {
                device.Dispose();
            }
            _devices.Clear();

            _capture?.Dispose();
            _capture = null;
        }
    }

    public class DeviceInfo
    {
        public int DeviceNumber { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public int Channels { get; set; }
    }
}
