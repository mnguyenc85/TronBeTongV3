from typing import List
import ctypes
import utils
import tpcan, tpsilo, analogInput, vanchuyencl, tpcoitron

PLCM = (ctypes.c_ubyte * 1001)()
DB09 = (ctypes.c_ubyte * 6)()
DB26 = (ctypes.c_ubyte * 378)()
DB29 = (ctypes.c_ubyte * 200)()
DB42 = (ctypes.c_ubyte * 16)()
DB43 = (ctypes.c_ubyte * 292)()

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

    self.xacl = False

    self.VanChuyenCL = vanchuyencl.CVanChuyenCL(PLCM)
    self.Coi = tpcoitron.CCoiTron(PLCM, DB26, DB29)
    self.AICL1 = analogInput.CAnalogInput(DB26, 306, 308, 310, 246)
    self.AIXI1 = analogInput.CAnalogInput(DB26, 346, 348, 350, 254)

  def _init_can(self):

    self.TPNuoc = tpcan.CTPCan('Nuoc', DB43, DB26, 268, 270, 274, 100)
    tp = self.TPNuoc.addTP(40, 280, 142, 146)
    tp.addVan(PLCM, 200, 6)
    self.TPNuoc.addDischarge(PLCM, 200, 7)

    self.TPCL1 = tpcan.CTPCan("CL1", DB43, DB26, 244, 246, 250, 100)
    tp = self.TPCL1.addTP(0, 240, 62, 66)
    tp.addVan(PLCM, 202, 4)
    tp.addVan(PLCM, 201, 1, 50, 10)
    self.TPCL1.addDischarge(PLCM, 202, 0)
    tp = self.TPCL1.addTP(4, 244, 70, 74)
    tp.addVan(PLCM, 202, 5)
    tp.addVan(PLCM, 203, 2, 50, 10)    
    self.TPCL1.addDischarge(PLCM, 202, 1)
    tp = self.TPCL1.addTP(8, 248, 78, 82)
    tp.addVan(PLCM, 202, 6)
    tp.addVan(PLCM, 203, 3, 50, 10)    
    self.TPCL1.addDischarge(PLCM, 202, 2)
    self.TPCL1.TTCanDu = 9

    self.TPXM1 = tpcan.CTPCan("XM1", DB43, DB26, 252, 254, 258, 50)
    tp = self.TPXM1.addTP(24, 264, 110)
    tp.addVan(PLCM, 200, 4)
    tp = self.TPXM1.addTP(28, 268, 118)
    tp.addVan(PLCM, 203, 6)
    self.TPXM1.addDischarge(PLCM, 200, 5)

    # self.TPXM2 = tpcan.CTPCan("XM2", DB43, DB26, 90, 86, DB68, 24, 10)
    # tp = self.TPXM2.addTP(32, 126)
    # tp.addVan(PLCM, 203, 7)
    # tp = self.TPXM2.addTP(36, 134)
    # tp.addVan(PLCM, 204, 0)
    # self.TPXM2.addDischarge(PLCM, 205, 3)

    self.TPPG1 = tpcan.CTPCan("PG1", DB43, DB26, 276, 278, 282, 0.3)
    tp = self.TPPG1.addTP(44, 284, 150)
    tp.addVan(PLCM, 205, 5, 0.3, 0.1)
    tp = self.TPPG1.addTP(48, 288, 158)
    tp.addVan(PLCM, 205, 6, 0.3, 0.1)
    self.TPPG1.addDischarge(PLCM, 203, 1)

  def process(self, delta):
    bStart = PLCM[1000] & 1 == 1
    bReset = PLCM[11] & 2 == 2

    if self.bRunning:
      self.TPCL1.run(delta)
      self.TPXM1.run(delta)
      self.TPNuoc.run(delta)
      self.TPPG1.run(delta, 0)

      # quá trình xả:
      self.nap_nguyen_lieu(delta)

      if not bStart:
        self.dung_he_thong()

      self.hoan_thanh()
    else:
      if not self.bReset0 and bReset:
        self.TPCL1.reset()
        self.TPXM1.reset()
        self.TPNuoc.reset()
        self.TPPG1.reset(0.2)
        self.Coi.reset()
      if not self.bStart0 and bStart:
        print('Bắt đầu chạy')
        m3 = utils.db2float(DB09, 0)
        soMe = utils.db2int16(DB09, 4)
        self.TPCL1.start(m3, soMe)
        self.TPXM1.start(m3, soMe)
        self.TPNuoc.start(m3, soMe)
        self.TPPG1.start(m3, soMe)
        
        utils.setbit(PLCM, 200, 3, 1)                # chạy băng tải xiên

        self.bRunning = 1

    self.VanChuyenCL.run(delta)
    if self.VanChuyenCL.get_nap_lieu_xong():
      self.Coi.start()
    self.Coi.run(delta)

    # ghi mô phỏng & cối trộn
    utils.setbit(PLCM, 11, 0, 1)
    utils.setbit(PLCM, 100, 1, 1)

    self.bReset0 = bReset
    self.bStart0 = bStart
  
    #self.AICL1.process(delta)
    #self.AIXI1.process(delta)

  def dung_he_thong(self):    
    self.TPCL1.stop()
    self.TPXM1.stop()
    self.TPNuoc.stop()
    self.TPPG1.stop()
    self.xacl = False
    print("Dừng chạy")

    utils.setbit(PLCM, 200, 3, 0)                # dừng băng tải xiên
    self.VanChuyenCL.stop()
    self.bRunning = 0

  def hoan_thanh(self):
    if self.TPCL1.check_hoanthanh():
      if self.TPXM1.check_hoanthanh():
        if self.TPNuoc.check_hoanthanh() and self.TPPG1.check_hoanthanh():
          self.dung_he_thong()
          utils.setbit(PLCM, 1000, 0, 0)        # xóa bStart

  def nap_nguyen_lieu(self, delta):
    if not self.xacl and not self.VanChuyenCL.is_run() and not self.Coi.is_mixing():
      if (self.TPCL1.check_can_du() or self.TPCL1.soMeDat == 0):
        self.xacl = True
        self.VanChuyenCL.start()
        # print(f'   Nạp cốt liệu: mẻ: {self.TPCL1.meht}, {self.TPCL2.meht}, {self.TPCL3.meht}')
        print(f'   Nạp cốt liệu: mẻ: {self.TPCL1.meht}')
    
    if self.xacl:
      if not self.TPCL1.vanxa:
        self.TPCL1.start_discharge()

      if (self.TPCL1.check_xa_xong() or self.TPCL1.soMeDat == 0):
        self.TPCL1.stop_discharge()
        
        # reset trạng thái -> cân mẻ mới
        self.TPCL1.me_moi()
        
        self.xacl = False

    mecl = self.TPCL1.meht

    if self.TPXM1.check_xa(2) and self.xacl and self.TPXM1.check_can_du() and self.TPXM1.meht <= mecl:
      print(f'   XM1 -> xả mẻ {self.TPXM1.meht}')
      self.TPXM1.start_discharge()

    # if self.TPXM2.check_xa(2) and self.xacl and self.TPXM2.check_can_du() and self.TPXM2.meht <= mecl:
    #   print(f'   XM2 -> xả mẻ {self.TPXM2.meht}')
    #   self.TPXM2.start_discharge()

    if self.TPNuoc.check_xa(2) and self.xacl and self.TPNuoc.check_can_du() and self.TPNuoc.meht <= mecl:
      print(f'   Nước -> xả mẻ {self.TPNuoc.meht}')
      self.TPNuoc.start_discharge()

    if self.TPPG1.check_xa(0.1) and self.TPNuoc.vanxa and self.TPPG1.check_can_du() and self.TPPG1.meht <= mecl:
      print(f'   PG1 -> xả mẻ {self.TPPG1.meht}')
      self.TPPG1.start_discharge(0)

    if self.TPXM1.check_xa_xong():
      self.TPXM1.stop_discharge()
      self.TPXM1.me_moi()

    # if self.TPXM2.check_xa_xong():
    #   self.TPXM2.stop_discharge()
    #   self.TPXM2.me_moi()

    if self.TPNuoc.check_xa_xong():
      self.TPNuoc.stop_discharge()
      self.TPNuoc.me_moi()
    
    if self.TPPG1.check_xa_xong(0.1):
      self.TPPG1.stop_discharge()
      self.TPPG1.me_moi()
