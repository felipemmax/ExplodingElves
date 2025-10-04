using UnityEngine;
using UnityEngine.Profiling;

namespace ExplodingElves.GameDebug
{
    public class MemoryOverlay : MonoBehaviour
    {
        private void OnGUI()
        {
            float total = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
            float reserved = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
            float mono = Profiler.GetMonoUsedSizeLong() / (1024f * 1024f);
            GUI.Label(new Rect(10, 10, 500, 60),
                $"Mem Used: {total:F1} MB (Reserved: {reserved:F1} MB) Mono: {mono:F1} MB");
        }
    }
}