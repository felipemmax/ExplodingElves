using UnityEngine;

namespace ExplodingElves.GameDebug
{
    public class MemoryOverlay : MonoBehaviour {
        void OnGUI() {
            var total = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f*1024f);
            var reserved = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024f*1024f);
            var mono = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / (1024f*1024f);
            GUI.Label(new Rect(10,10,500,60), $"Mem Used: {total:F1} MB (Reserved: {reserved:F1} MB) Mono: {mono:F1} MB");
        }
    }
}
