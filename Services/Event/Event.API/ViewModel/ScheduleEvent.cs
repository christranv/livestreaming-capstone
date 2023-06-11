using System;
using System.Collections;

namespace Event.API.ViewModel
{
    public class ScheduleEvent
    {
        public DateTime StartDate { get; }
        public IEnumerable SubEventInDate { get; }
        public ScheduleEvent(DateTime startDate, IEnumerable subEventInDate)
        {
            StartDate = startDate;
            SubEventInDate = subEventInDate;
        }
    }
}