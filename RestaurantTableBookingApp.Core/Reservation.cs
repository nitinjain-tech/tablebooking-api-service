﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantTableBookingApp.Core;

public partial class Reservation
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TimeSlotId { get; set; }

    public DateTime ReservationDate { get; set; }

    public string ReservationStatus { get; set; } = null!;

    public bool ReminderSent { get; set; }

    [ForeignKey("TimeSlotId")]
    [InverseProperty("Reservations")]
    public virtual TimeSlot TimeSlot { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Reservations")]
    public virtual User User { get; set; } = null!;
}
