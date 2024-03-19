using System;
using System.Collections.Generic;

namespace VoiceProximityMeasurementSystem.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public double? Result { get; set; }

    public string Name { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
