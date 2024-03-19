using System;
using System.Collections.Generic;

namespace VoiceProximityMeasurementSystem.Repository.Models;

public partial class UserAnswer
{
    public int UserAnswerId { get; set; }

    public int? LandoltId { get; set; }

    public int? UserId { get; set; }

    public virtual LandoltE Landolt { get; set; }

    public virtual User User { get; set; }
}
