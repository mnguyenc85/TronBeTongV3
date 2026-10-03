import random
import utils

class CAnalogInput:
    def __init__(self, db, aiAddr, zeroAddr, spanAddr, klAddr):
        '''
        
        '''
        self.db = db
        self.ai = aiAddr
        self.zero = zeroAddr
        self.span = spanAddr
        self.kl = klAddr

    def process(self, delta):
        ai = int(random.random() * 100)

        zero = utils.db2int16(self.db, self.zero)
        span = utils.db2float(self.db, self.span)

        kl = (ai - zero) * span

        utils.float2db(kl, self.db, self.kl)
        utils.int162db(ai, self.db, self.ai)