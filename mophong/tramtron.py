from typing import List
import ctypes
import utils
import tpcan, tpsilo

PLCM = (ctypes.c_ubyte * 1001)()
DB09 = (ctypes.c_ubyte * 6)()
DB16 = (ctypes.c_ubyte * 68)()
DB19 = (ctypes.c_ubyte * 68)()
DB21 = (ctypes.c_ubyte * 68)()
DB23 = (ctypes.c_ubyte * 26)()
DB24 = (ctypes.c_ubyte * 68)()
DB26 = (ctypes.c_ubyte * 238)()
DB28 = (ctypes.c_ubyte * 28)()
DB29 = (ctypes.c_ubyte * 190)()
DB30 = (ctypes.c_ubyte * 28)()
DB31 = (ctypes.c_ubyte * 28)()
DB33 = (ctypes.c_ubyte * 28)()
DB34 = (ctypes.c_ubyte * 26)()
DB35 = (ctypes.c_ubyte * 92)()
DB36 = (ctypes.c_ubyte * 92)()
DB42 = (ctypes.c_ubyte * 16)()
DB43 = (ctypes.c_ubyte * 292)()
DB54 = (ctypes.c_ubyte * 78)()
DB68 = (ctypes.c_ubyte * 26)()
DB90 = (ctypes.c_ubyte * 92)()

# trạng thái cân
# utils.int162db(5, DB16, 66)
# utils.int162db(5, DB19, 66)
# utils.int162db(5, DB21, 66)
# utils.int162db(5, DB24, 66)

class CTramTron:
  def __init__(self):
    self.bRunning = 0
    self.bReset0 = 0
    self.bStart0 = 0

    self._init_can()

    self.xacl = 0
    self.btngang_run = 0
    self.btngang_tg = 0

  def _init_can(self):
    self.TPCL1 = tpcan.CTPCan("CL1", DB43, DB16, 66, 62, DB28, 24)
    tp = self.TPCL1.addTP(0, 62)
    tp.addVan(PLCM, 202, 4)
    tp.addVan(PLCM, 201, 1)
    self.TPCL1.addDischarge(PLCM, 202, 0)

    self.TPCL2 = tpcan.CTPCan("CL2", DB43, DB19, 66, 62, DB30, 24)
    tp = self.TPCL2.addTP(4, 70)
    tp.addVan(PLCM, 202, 5)
    tp.addVan(PLCM, 203, 2)    
    self.TPCL2.addDischarge(PLCM, 202, 1)

    self.TPCL3 = tpcan.CTPCan("CL3", DB43, DB21, 66, 62, DB31, 24)
    tp = self.TPCL3.addTP(8, 78)
    tp.addVan(PLCM, 202, 6)
    tp.addVan(PLCM, 203, 3)    
    self.TPCL3.addDischarge(PLCM, 202, 2)

    self.TPXM1 = tpcan.CTPCan("XM1", DB43, DB36, 90, 86, DB34, 24, 10)
    tp = self.TPXM1.addTP(24, 110)
    tp.addVan(PLCM, 200, 4)
    tp = self.TPXM1.addTP(28, 118)
    tp.addVan(PLCM, 203, 6)
    self.TPXM1.addDischarge(PLCM, 200, 5)

    self.TPXM2 = tpcan.CTPCan("XM2", DB43, DB90, 90, 86, DB68, 24, 10)
    tp = self.TPXM2.addTP(32, 126)
    tp.addVan(PLCM, 203, 7)
    tp = self.TPXM2.addTP(36, 134)
    tp.addVan(PLCM, 204, 0)
    self.TPXM2.addDischarge(PLCM, 205, 3)

    self.TPNuoc = tpcan.CTPCan("Nuoc", DB43, DB24, 66, 62, DB33, 24, 5)
    tp = self.TPNuoc.addTP(40, 142)
    tp.addVan(PLCM, 200, 6)
    self.TPNuoc.addDischarge(PLCM, 200, 7)

    self.TPPG1 = tpcan.CTPCan("PG1", DB43, DB35, 90, 86, DB23, 24, 0.3)
    tp = self.TPPG1.addTP(44, 150)
    tp.addVan(PLCM, 205, 5, 0.3, 0.1)
    tp = self.TPPG1.addTP(48, 158)
    tp.addVan(PLCM, 205, 6, 0.3, 0.1)
    self.TPPG1.addDischarge(PLCM, 203, 1)

  def process(self, delta):
    bStart = PLCM[1000] & 1 == 1
    bReset = PLCM[11] & 2 == 2

    if self.bRunning:
      self.TPCL1.run(delta)
      self.TPCL2.run(delta)
      self.TPCL3.run(delta)
      self.TPXM1.run(delta)
      self.TPXM2.run(delta)
      self.TPNuoc.run(delta)
      self.TPPG1.run(delta, 0)

      # quá trình xả:
      self.napnguyenlieu(delta)

      if not bStart:
        self.dung_he_thong()

      self.hoan_thanh()
    else:
      if not self.bReset0 and bReset:
        self.TPCL1.reset()
        self.TPCL2.reset()
        self.TPCL3.reset()
        self.TPXM1.reset()
        self.TPXM2.reset()
        self.TPNuoc.reset()
        self.TPPG1.reset(0.2)
      if not self.bStart0 and bStart:
        print('Bắt đầu chạy')
        m3 = utils.db2float(DB09, 0)
        soMe = utils.db2int16(DB09, 4)
        self.TPCL1.start(m3, soMe)
        self.TPCL2.start(m3, soMe)
        self.TPCL3.start(m3, soMe)
        self.TPXM1.start(m3, soMe)
        self.TPXM2.start(m3, soMe)
        self.TPNuoc.start(m3, soMe)
        self.TPPG1.start(m3, soMe)
        
        utils.setbit(PLCM, 200, 3, 1)                # chạy băng tải xiên

        self.bRunning = 1

    # ghi mô phỏng & cối trộn
    utils.setbit(PLCM, 11, 0, 1)
    utils.setbit(PLCM, 100, 1, 1)

    self.bReset0 = bReset
    self.bStart0 = bStart

  def dung_he_thong(self):    
    self.TPCL1.stop()
    self.TPCL2.stop()
    self.TPCL3.stop()
    self.TPXM1.stop()
    self.TPXM2.stop()
    self.TPNuoc.stop()
    self.TPPG1.stop()
    self.xacl = 0
    print("Dừng chạy")

    utils.setbit(PLCM, 200, 3, 0)                # dừng băng tải xiên
    utils.setbit(PLCM, 200, 1, 0)                # dừng băng tải ngang
    self.bRunning = 0

  def hoan_thanh(self):
    if self.TPCL1.check_hoanthanh() and self.TPCL2.check_hoanthanh() and self.TPCL3.check_hoanthanh():
      if self.TPXM1.check_hoanthanh() and self.TPXM2.check_hoanthanh():
        if self.TPNuoc.check_hoanthanh() and self.TPPG1.check_hoanthanh():
          self.dung_he_thong()
          utils.setbit(PLCM, 1000, 0, 0)        # xóa bStart

  def napnguyenlieu(self, delta):
    if not self.xacl:
      if (self.TPCL1.check_can_du() or self.TPCL1.soMeDat == 0) and (self.TPCL2.check_can_du() or self.TPCL2.soMeDat == 0) and (self.TPCL3.check_can_du() or self.TPCL3.soMeDat == 0):
        self.xacl = 1
        self.btngang_run = 1
        self.btngang_tg = 0
        utils.setbit(PLCM, 200, 1, 1)                # chạy băng tải ngang
        print(f'   Nạp cốt liệu: mẻ: {self.TPCL1.meht}, {self.TPCL2.meht}, {self.TPCL3.meht}')
    
    if self.btngang_run:
      self.btngang_tg += delta

    if self.xacl:
      if self.btngang_tg > 2 and not self.TPCL3.vanxa:
        self.TPCL3.start_discharge()
      if self.TPCL3.tgxa > 3 and not self.TPCL2.vanxa:
        self.TPCL2.start_discharge()
      if self.TPCL2.tgxa > 3 and not self.TPCL1.vanxa:
        self.TPCL1.start_discharge()

      if (self.TPCL1.check_xa_xong() or self.TPCL1.soMeDat == 0) and (self.TPCL2.check_xa_xong() or self.TPCL2.soMeDat == 0) and (self.TPCL3.check_xa_xong() or self.TPCL3.soMeDat == 0):
        self.TPCL3.stop_discharge()
        self.TPCL2.stop_discharge()
        self.TPCL1.stop_discharge()
        
        # reset trạng thái -> cân mẻ mới
        self.TPCL1.me_moi()
        self.TPCL2.me_moi()
        self.TPCL3.me_moi()
        
        self.xacl = 0
        self.btngang_tg = 0
    else:
      if self.btngang_run:
        if self.btngang_tg > 3:
          self.btngang_run = 0
          utils.setbit(PLCM, 200, 1, 0)                # dừng băng tải ngang

    mecl = max(self.TPCL1.meht, self.TPCL2.meht, self.TPCL3.meht)

    if self.TPXM1.check_xa(2) and self.xacl and self.TPXM1.check_can_du() and self.TPXM1.meht <= mecl:
      print(f'   XM1 -> xả mẻ {self.TPXM1.meht}')
      self.TPXM1.start_discharge()

    if self.TPXM2.check_xa(2) and self.xacl and self.TPXM2.check_can_du() and self.TPXM2.meht <= mecl:
      print(f'   XM2 -> xả mẻ {self.TPXM2.meht}')
      self.TPXM2.start_discharge()

    if self.TPNuoc.check_xa(2) and self.xacl and self.TPNuoc.check_can_du() and self.TPNuoc.meht <= mecl:
      print(f'   Nước -> xả mẻ {self.TPNuoc.meht}')
      self.TPNuoc.start_discharge()

    if self.TPPG1.check_xa(0.1) and self.TPNuoc.vanxa and self.TPPG1.check_can_du() and self.TPPG1.meht <= mecl:
      print(f'   PG1 -> xả mẻ {self.TPPG1.meht}')
      self.TPPG1.start_discharge(0)

    if self.TPXM1.check_xa_xong():
      self.TPXM1.stop_discharge()
      self.TPXM1.me_moi()

    if self.TPXM2.check_xa_xong():
      self.TPXM2.stop_discharge()
      self.TPXM2.me_moi()

    if self.TPNuoc.check_xa_xong():
      self.TPNuoc.stop_discharge()
      self.TPNuoc.me_moi()
    
    if self.TPPG1.check_xa_xong(0.1):
      self.TPPG1.stop_discharge()
      self.TPPG1.me_moi()
