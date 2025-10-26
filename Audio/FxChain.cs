using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using EchoBridge.Audio.Effects;
using System;
using System.Collections.Generic;

namespace EchoBridge.Audio
{
    /// <summary>
    /// Manages a chain of DSP effects applied to an audio stream
    /// </summary>
    public class FxChain : IDisposable
    {
        private readonly List<IEffect> _effects;
        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public IReadOnlyList<IEffect> Effects => _effects.AsReadOnly();

        public FxChain()
        {
            _effects = new List<IEffect>();
        }

        public void AddEffect(IEffect effect)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));
            
            _effects.Add(effect);
        }

        public void RemoveEffect(IEffect effect)
        {
            _effects.Remove(effect);
        }

        public void ClearEffects()
        {
            foreach (var effect in _effects)
            {
                if (effect is IDisposable disposable)
                    disposable.Dispose();
            }
            _effects.Clear();
        }

        public ISampleProvider Apply(ISampleProvider source)
        {
            if (!_isEnabled || _effects.Count == 0)
                return source;

            ISampleProvider chain = source;
            
            foreach (var effect in _effects)
            {
                if (effect.IsEnabled)
                {
                    chain = effect.Apply(chain);
                }
            }

            return chain;
        }

        public void Dispose()
        {
            ClearEffects();
        }
    }

    /// <summary>
    /// Base interface for all audio effects
    /// </summary>
    public interface IEffect
    {
        string Name { get; }
        bool IsEnabled { get; set; }
        ISampleProvider Apply(ISampleProvider source);
    }
}
