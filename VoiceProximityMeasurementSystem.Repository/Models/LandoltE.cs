using System;
using System.Collections.Generic;

namespace VoiceProximityMeasurementSystem.Repository.Models;

public partial class LandoltE
{
    public int Id { get; set; }

    public string Entity { get; set; }

    public double? Size { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
