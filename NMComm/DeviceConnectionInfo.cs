using System;

namespace NMComm
{
    public enum DeviceConnStates { None, Disconnect, Bad, Good }

    public class DeviceConnectionInfo
    {
        private DeviceConnStates _state;
        public DeviceConnStates State { get { return _state; } set { if (_state != value) { _state = value; IsChanged = true; } } }
        private int _badTags = 0;
        public int NoBadTags { get { return _badTags; } set { if (_badTags != value) { _badTags = value; IsChanged = true; } } }

        public bool IsChanged { get; set; } = false;
    }
}
