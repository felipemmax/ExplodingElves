using System.Collections.Generic;
using ExplodingElves.Core;
using ExplodingElves.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ExplodingElves.UI
{
    /// <summary>
    ///     Simple UI controller that maps four sliders to four spawners (Black, Red, White, Blue).
    ///     SRP: Only translates UI interactions to ISpawner.SetInterval calls.
    /// </summary>
    public class IntervalUI : MonoBehaviour
    {
        [Header("Sliders (assign in inspector)")] [SerializeField]
        private Slider blackSlider;

        [SerializeField] private Slider redSlider;
        [SerializeField] private Slider whiteSlider;
        [SerializeField] private Slider blueSlider;

        private readonly Dictionary<ElfColor, ISpawner> _spawnerByColor = new();

        private void OnEnable()
        {
            WireSlider(blackSlider, ElfColor.Black);
            WireSlider(redSlider, ElfColor.Red);
            WireSlider(whiteSlider, ElfColor.White);
            WireSlider(blueSlider, ElfColor.Blue);
        }

        private void OnDisable()
        {
            UnwireSlider(blackSlider);
            UnwireSlider(redSlider);
            UnwireSlider(whiteSlider);
            UnwireSlider(blueSlider);
        }

        [Inject]
        private void Construct(List<ISpawner> spawners)
        {
            _spawnerByColor.Clear();
            foreach (ISpawner s in spawners)
                if (s != null)
                    _spawnerByColor[s.ElfColor] = s;
        }

        private void WireSlider(Slider slider, ElfColor color)
        {
            if (slider == null) return;
            slider.onValueChanged.AddListener(v => OnSliderChanged(color, v));
            if (_spawnerByColor.TryGetValue(color, out ISpawner sp)) slider.SetValueWithoutNotify(sp.Interval);
        }

        private void UnwireSlider(Slider slider)
        {
            if (slider == null) return;
            slider.onValueChanged.RemoveAllListeners();
        }

        private void OnSliderChanged(ElfColor color, float value)
        {
            if (_spawnerByColor.TryGetValue(color, out ISpawner sp)) sp.SetInterval(value);
        }
    }
}