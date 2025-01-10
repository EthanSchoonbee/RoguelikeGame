using System.Collections.Generic;
using System.Linq;
using Game.interfaces;

namespace Game.systems
{
    public class SchedulingSystem
    {
        // tracks the current game time (turn). Starts at 0
        private int _time;

        // a schedule where Key: time values | Values: list of objects scheduled to act at that time
        private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

        // initialize time and schedule
        public SchedulingSystem()
        {
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
        }

        // add a game object to the schedule
        // assigns a time for the object by combining
        // the current time (_time) and the number of item in the schedule (_scheduleables.Count)
        public void Add(IScheduleable scheduleable)
        {
            // get te key value for the scheudle which is the end od the queue plus the objects time
            int key = _time + _scheduleables.Count;

            // if the key (time) doesnt exist in the schedule create a new list for that time
            if (!_scheduleables.ContainsKey(key))
            {
                _scheduleables.Add(key, new List<IScheduleable>());
            }

            // add the game object to the list at that time
            _scheduleables[key].Add(scheduleable);
        }

        // remove a specific game object from the schedule
        // used to cancel future actions for an object (if a monster dies)
        public void Remove(IScheduleable scheduleable)
        {
            // initialise a KeyValuePair to store a scheduleable tha is found
            KeyValuePair<int, List<IScheduleable>> scheduleableListFound
                = new KeyValuePair<int, List<IScheduleable>>(-1, null);

            // search the schedule to find the list that contains the game object
            foreach (var scheduleablesList in _scheduleables)
            {
                // if the list contains the object, store a reference of it
                if (scheduleablesList.Value.Contains(scheduleable))
                {
                    scheduleableListFound = scheduleablesList;
                    // stop looking
                    break;
                }
            }

            // check if the captured list is empty, if not remove the game object from it
            if (scheduleableListFound.Value != null)
            {
                //remove that list from the schedule
                scheduleableListFound.Value.Remove(scheduleable);

                // if the list becomes empty, remove the key (time) from the schedule
                if (scheduleableListFound.Value.Count <= 0)
                {
                    _scheduleables.Remove(scheduleableListFound.Key);
                }
            }
        }

        // get the next game object whose turn it is to act, based on the schedule
        // advance the game time to the time of the next action
        public IScheduleable Get()
        {
            // get the first group in the schedule (the earliest turn)
            var firstScheduleableGroup = _scheduleables.First();

            // get the first object in that group
            var firstScheduleable = firstScheduleableGroup.Value.First();

            // remove the object from the schedule (prevent it from acting twice)
            Remove(firstScheduleable);

            // update time to the time of the retrieved object
            _time = firstScheduleableGroup.Key;

            // return the object
            return firstScheduleable;
        }

        // get the current time (turn) for the schedule (track turns)
        public int GetTime()
        {
            return _time;
        }

        // reset the time and clear the schedule
        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }
    }
}