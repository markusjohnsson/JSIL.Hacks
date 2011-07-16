using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Meta;

namespace JSIL.Dom
{
    public class IntervalDispatcher
    {
        private object _handle = null;

        public event EventHandler Tick;

        public long Interval { get; private set; }

        /// <summary>
        /// Creates an IntervalDispatcher that dispatches an event after each elapse of <paramref name="interval"/> ms.
        /// </summary>
        /// <param name="interval">The interval in ms</param>
        public IntervalDispatcher(long interval)
        {
            this.Interval = interval;
        }

        private void OnNativeEvent()
        {
            if (Tick != null)
            {
                Tick(this, EventArgs.Empty);
            }
        }

        [JSReplacement("$this._handle = setInterval(function () { $this.OnNativeEvent(); }, $this.Interval)")]
        public void Start()
        {
        }

        [JSReplacement("clearInterval($this._handle)")]
        public void Stop()
        {
        }
    }
}
