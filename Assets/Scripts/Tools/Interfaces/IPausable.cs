using UnityEngine;
using System;

public interface IPausable
{
    bool IsPaused { get; }
    void Pause();   // Called to pause this component
    void Resume();  // Called to resume this component
    event Action OnPausedChanged;  // Optional: For reactive updates (e.g., UI feedback)
}