from __future__ import annotations
import random
import tpsilo
import utils

KL_ZERO = 1

class CTPCan:
  def __init__(self, ten, dbCP, dbCanTT, ttAddr, klAddr, dbCanMe, meAddr, klxa = 20):
    self.ten = ten
    self.DbCapphoi = dbCP
    self.DbCanTT = dbCanTT
    self.TTAddr = ttAddr
    self.klAddr = klAddr
    self.DbCanMe = dbCanMe
    self.meAddr = meAddr
    self.thanhPhan: list[tpsilo.CTPSilo] = []
    self.soMeDat = 0
    self.tp_i = 0
    self.running = 0
    self.kl = 0
    self.trangthai = 0
    self.meht = 0
    self.vanxa = 0
    self.DbVan = None
    self.TTCanDu = 1
    self.klxa = klxa

  def addTP(self, cpAddr, closeAddr) -> tpsilo.CTPSilo:
    tp = tpsilo.CTPSilo(cpAddr, closeAddr)
    self.thanhPhan.append(tp)
    self.TTCanDu = 3 * len(self.thanhPhan) + 1
    return tp  

  def addDischarge(self, dbVan, byte, bit):
    self.DbVan = dbVan
    self.vanByteAddr = byte
    self.vanBitAddr = bit
    self.tgxa = 0

  def reset(self, kldu = 5):
    self.tp_i = 0
    self.kl = random.random() * kldu - kldu / 2
    self.trangthai = 0
    self.meht = 0
    utils.float2db(self.kl, self.DbCanTT, self.klAddr)                          # update kl trên cân
    utils.int162db(self.trangthai, self.DbCanTT, self.TTAddr)                   # update trạng thái cân
    utils.int162db(self.meht, self.DbCanMe, self.meAddr)                        # update mẻ hiện tại    

  def start(self, m3: float, some: int):
    self.soMeDat = some
    tongklcp = 0
    if some <= 0: return
    for tp in self.thanhPhan:
      if tp.klCPAddr >= 0:
        tp.klCP = utils.db2float(self.DbCapphoi, tp.klCPAddr)
      tongklcp += tp.klCP
      tp.klMe = tp.klCP * m3 / some
    if tongklcp > 0:
      self.running = 1
    else:
      self.soMeDat = 0

  def stop(self):
    for tp in self.thanhPhan:
      tp.setVan(0)
    self.running = 0
    utils.float2db(self.kl, self.DbCanTT, self.klAddr)
    self.stop_discharge()

  def start_discharge(self, kl_zero = KL_ZERO):
    if self.DbVan and self.kl > kl_zero:
      self.vanxa = 1
      self.tgxa = 0
      utils.setbit(self.DbVan, self.vanByteAddr, self.vanBitAddr, 1)

  def stop_discharge(self):
    if self.DbVan:
      self.vanxa = 0
      utils.setbit(self.DbVan, self.vanByteAddr, self.vanBitAddr, 0)

  def check_can_du(self) -> bool:
    return self.trangthai == self.TTCanDu and self.soMeDat > 0

  def check_xa(self, klzero = KL_ZERO) -> bool:
    '''
    Kiểm tra có thể xả không
    
    :param klzero: Description
    :return: True nếu có thể
    :rtype: bool
    '''
    return not self.vanxa and self.kl > klzero

  def check_xa_xong(self, klzero = KL_ZERO) -> bool:
    '''
    Kiểm tra đã xả xong?
    
    :param klzero: Description
    :return: True nếu xong
    :rtype: bool
    '''
    return self.vanxa and self.kl < klzero

  def check_hoanthanh(self, klzero = KL_ZERO):
    return not self.running or (self.soMeDat == 0) or (self.trangthai == self.TTCanDu and self.kl < klzero and self.meht == self.soMeDat)

  def me_moi(self):
    if self.meht < self.soMeDat:
      print(f'   {self.ten} -> mẻ mới: soMeDat = {self.soMeDat}')
      self.tp_i = 0
      self.trangthai = 0
      utils.int162db(self.trangthai, self.DbCanTT, self.TTAddr)                   # update trạng thái cân
      return True
    else:
      print(f'   {self.ten} -> dừng chạy')
      self.running = 0
      return False

  def run(self, delta, klzero = KL_ZERO):
    if self.vanxa:
      self.tgxa += delta
      if self.kl > klzero:
        self.kl -= self.klxa * delta

    if self.running:
      if self.trangthai == 0 and self.meht < self.soMeDat:
        self.meht += 1
        for tp in self.thanhPhan:
          tp.reset()
          # xóa chốt kl
          utils.float2db(0, self.DbCapphoi, tp.klCloseAddr)
        self.trangthai = 1

      if self.trangthai > 0:
        if self.tp_i < len(self.thanhPhan):
          tp = self.thanhPhan[self.tp_i]
          if not tp.bKL0:
            # Chốt kl ban đầu
            tp.bKL0 = 1
            tp.kl0 = self.kl
            print(f"   {self.ten} -> cân mẻ {self.meht} tp {self.tp_i}")
          else:
            # Cân thành phần
            deltakl = self.kl - tp.kl0
            if deltakl >= tp.klMe:
              # Cân đủ -> đến thành phần tiếp theo
              tp.setVan(0)
              tp.klClose = deltakl
              utils.float2db(tp.klClose, self.DbCapphoi, tp.klCloseAddr)            # update kl chốt
              self.tp_i += 1
            elif deltakl < tp.klMe - tp.klroi_tho:
              # cân thô
              self.kl += tp.setVan(1) * delta
              self.trangthai = self.tp_i * 3 + 1
            else:
              # cân tinh
              self.kl += tp.setVan(2) * delta
              self.trangthai = self.tp_i * 3 + 2
        else:
          # cân đủ
          self.trangthai = self.tp_i * 3 + 1

        utils.float2db(self.kl, self.DbCanTT, self.klAddr)                          # update kl trên cân
        utils.int162db(self.trangthai, self.DbCanTT, self.TTAddr)                   # update trạng thái cân
        utils.int162db(self.meht, self.DbCanMe, self.meAddr)                        # update mẻ hiện tại