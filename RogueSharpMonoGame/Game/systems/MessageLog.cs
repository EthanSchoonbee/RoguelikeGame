using System.Collections.Generic;
using System.Security.Permissions;
using RLNET;

namespace Game.systems
{
    // represents a queue of messages that can be added to
    // has a method for and drawing to an RLConsole
    public class MessageLog
    {
        // define the maximum number of lines to store
        private static readonly int maxLines = 9;

        // use a queue structure to keep track of line sof text
        // first line added to the log will also be the first removed
        private readonly Queue<string> _lines;

        // initialise a message log with a queue of strings

        public MessageLog()
        {
           _lines = new Queue<string>();
        }

        public void Add(string message)
        {
            _lines.Enqueue(message);

            // when exceeding the maximum number of lines remove the oldest one
            if (_lines.Count > maxLines)
            {
                _lines.Dequeue();
            }
        }

        // draw each line of the MessageLog queue to the console (message sub-console)
        public void Draw(RLConsole console)
        {
            console.Clear();
            string[] lines = _lines.ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i + 1, lines[i], RLColor.White);
            }
        }
    }
}