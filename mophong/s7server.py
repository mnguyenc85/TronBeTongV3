# Mô phỏng PLC S7-1200 thiết bị thử khóa
# Thời gian: 2025/04/28

from snap7.server import Server
from snap7.type import SrvArea
import time
from datetime import datetime
import tramtron
import logging

last_delta = 0
# Setup logging
#logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(message)s')
def event_callback(event) -> None:
    """
    Callback xử lý event. Chỉ in thông báo cho connect/disconnect.
    - EVT_ConnOpen: Client connect.
    - EVT_ConnClosed: Client disconnect.
    Các event khác (read/write) bị bỏ qua.
    """
    if event.EvtCode == 0x08:
        print(f"[{time.strftime('%Y-%m-%d %H:%M:%S')}] Client CONNECTED từ địa chỉ: {event.EvtSender}")
    elif event.EvtCode == 0x80:
        print(f"[{time.strftime('%Y-%m-%d %H:%M:%S')}] Client DISCONNECTED từ địa chỉ: {event.EvtSender} -> delta = {last_delta}")
    elif event.EvtCode == 0x2000:
        pass
    # Bỏ qua các event khác như read/write (EVT_ReadReq, EVT_WriteReq, v.v.)

tb1 = tramtron.CTramTron()

srv = Server()
srv.set_events_callback(event_callback)
srv.register_area(SrvArea.MK, 0, tramtron.PLCM)    # Markers (M)
srv.register_area(SrvArea.DB, 9, tramtron.DB09)
srv.register_area(SrvArea.DB, 26, tramtron.DB26)
srv.register_area(SrvArea.DB, 29, tramtron.DB29)
srv.register_area(SrvArea.DB, 42, tramtron.DB42)
srv.register_area(SrvArea.DB, 43, tramtron.DB43)

ip = "0.0.0.0"

srv.start_to(ip, 5102)             # port=5102, tránh trùng port 102 của TIA Portal
print("Snap7 Server running at {0}:5102... Press Ctrl+C to stop.".format(ip))
t0 = datetime.now()
try:
  while True:
    t = datetime.now()
    last_delta = (t - t0).total_seconds()
    t0 = t

    srv.pick_event()                    # Process client requests

    tb1.process(last_delta)

    time.sleep(0.1)
except Exception as e:
  if e: print("\nError: ", e)
  print("\nShutting down server...")
  srv.stop()
  srv.destroy()