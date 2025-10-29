using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EchoBridge.Audio;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace EchoBridge.UI
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly AudioRouter _audioRouter;

        [ObservableProperty]
        private ObservableCollection<OutputDeviceViewModel> _outputDevices;

        [ObservableProperty]
        private ObservableCollection<DeviceInfo> _availableDevices;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _isRouting;

        public MainWindowViewModel()
        {
            _audioRouter = new AudioRouter();
            _outputDevices = new ObservableCollection<OutputDeviceViewModel>();
            _availableDevices = new ObservableCollection<DeviceInfo>();
            _statusMessage = "Ready";

            _audioRouter.StatusChanged += OnRouterStatusChanged;
            _audioRouter.ErrorOccurred += OnRouterError;

            LoadAvailableDevices();
        }

        private void LoadAvailableDevices()
        {
            try
            {
                var devices = AudioRouter.GetAvailableDevices();
                AvailableDevices.Clear();
                foreach (var device in devices)
                {
                    AvailableDevices.Add(device);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading devices: {ex.Message}";
            }
        }

        [RelayCommand]
        private void AddOutputDevice(DeviceInfo deviceInfo)
        {
            try
            {
                if (OutputDevices.Any(d => d.DeviceNumber == deviceInfo.DeviceNumber))
                {
                    StatusMessage = $"Device '{deviceInfo.DeviceName}' is already added";
                    return;
                }

                if (OutputDevices.Count >= 5)
                {
                    StatusMessage = "Maximum 5 output devices supported";
                    return;
                }

                var outputDevice = new OutputDevice(deviceInfo.DeviceName, deviceInfo.DeviceNumber);
                _audioRouter.AddDevice(outputDevice);

                var viewModel = new OutputDeviceViewModel(outputDevice);
                OutputDevices.Add(viewModel);

                StatusMessage = $"Added device: {deviceInfo.DeviceName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error adding device: {ex.Message}";
            }
        }

        [RelayCommand]
        private void RemoveOutputDevice(OutputDeviceViewModel deviceVm)
        {
            try
            {
                _audioRouter.RemoveDevice(deviceVm.Device);
                OutputDevices.Remove(deviceVm);
                StatusMessage = $"Removed device: {deviceVm.DeviceName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error removing device: {ex.Message}";
            }
        }

        [RelayCommand]
        private void StartRouting()
        {
            try
            {
                if (OutputDevices.Count == 0)
                {
                    StatusMessage = "Please add at least one output device";
                    return;
                }

                _audioRouter.Start();
                IsRouting = true;
                StatusMessage = "Audio routing started";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error starting routing: {ex.Message}";
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void StopRouting()
        {
            try
            {
                _audioRouter.Stop();
                IsRouting = false;
                StatusMessage = "Audio routing stopped";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error stopping routing: {ex.Message}";
            }
        }

        [RelayCommand]
        private void RefreshDevices()
        {
            LoadAvailableDevices();
            StatusMessage = "Device list refreshed";
        }

        private void OnRouterStatusChanged(object? sender, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusMessage = message;
            });
        }

        private void OnRouterError(object? sender, Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusMessage = $"Error: {ex.Message}";
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }

    public partial class OutputDeviceViewModel : ObservableObject
    {
        public OutputDevice Device { get; }

        public string DeviceName => Device.DeviceName;
        public int DeviceNumber => Device.DeviceNumber;

        [ObservableProperty]
        private double _volume = 100;

        [ObservableProperty]
        private int _delay = 0;

        [ObservableProperty]
        private bool _equalizerEnabled = false;

        [ObservableProperty]
        private bool _reverbEnabled = false;

        [ObservableProperty]
        private bool _bassBoostEnabled = false;

        [ObservableProperty]
        private bool _delayEnabled = false;

        [ObservableProperty]
        private bool _compressorEnabled = false;

        [ObservableProperty]
        private bool _limiterEnabled = false;

        public OutputDeviceViewModel(OutputDevice device)
        {
            Device = device;
        }

        partial void OnVolumeChanged(double value)
        {
            Device.Volume = (float)(value / 100.0);
        }

        partial void OnDelayChanged(int value)
        {
            Device.DelayMs = value;
        }

        [RelayCommand]
        private void ToggleEqualizer()
        {
            // Effect toggling will be implemented with the FX UI
        }

        [RelayCommand]
        private void ToggleReverb()
        {
            // Effect toggling will be implemented with the FX UI
        }

        [RelayCommand]
        private void ToggleBassBoost()
        {
            // Effect toggling will be implemented with the FX UI
        }
    }
}
