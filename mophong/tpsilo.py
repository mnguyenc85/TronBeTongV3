import utils

class CTPSilo:
  def __init__(self, cpAddr: int, closeAddr: int):
    self.klCPAddr = cpAddr
    self.klCloseAddr = closeAddr
    
    self.klCP = 0
    self.klMe = 0
    self.klClose = 0

    self.lstVan = []
    self.bKL0 = 0
    self.kl0 = 0

    self.klroi_tho = 10
    self.klroi_tinh = 3

  def reset(self):
    self.bKL0 = 0

  def addVan(self, db, byteAddr, bitAddr, klroi_tho = 20, klroi_tinh = 3):
    self.klroi_tho = klroi_tho
    self.klroi_tinh = klroi_tinh
    self.lstVan.append([db, byteAddr, bitAddr])
    
  def setVan(self, state) -> float:
    ''' Đặt trạng thái van và trả về khối lượng rơi '''
    n = len(self.lstVan)
    klroi = 0
    
    if n > 0:
      v1 = self.lstVan[0]
      utils.setbit(v1[0], v1[1], v1[2], state == 1)
    if n > 1:
      v2 = self.lstVan[1]      
      utils.setbit(v2[0], v2[1], v2[2], state == 2)

    if n == 2:
      if state == 1: klroi += self.klroi_tho
      elif state == 2: klroi += self.klroi_tinh
    elif n == 1:
      klroi += self.klroi_tho

    return klroi