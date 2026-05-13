using System;
using TronBeTongV3.Core;
namespace TronBeTongV3.View
{
    public class ButtonArgs: EventArgs
    {
        public ButtonTypes Button { get; set; }
        public int ObjectId { get; set; }

        public int BtState { get; set; }
        public double Value { get; set; }
    }
}
