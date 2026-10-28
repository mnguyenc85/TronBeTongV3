import utils

# DB26
# Coi.TGTron    284
# Coi.TGXaNua   288
# Coi.TGXa      286
# Coi.MeHT      290
# DB29
# TGTron: 76    TGXaNua: 78     TGXa: 80
# Memory
# Close: 102.5  Open: 102.6     HalfOpen: 102.7

class CCoiTron:
    def __init__(self, dbm, db26, db29):
        self.dbm = dbm
        self.db26 = db26
        self.db29 = db29
        self.meht = 0
        self.state = 0              # 0; 1: tron; 2: xa; 4: xa nua        
        self.setTGTron = 0
        self.setTGXa = 0
        self.setTGXaNua = 0
        self.tgTron = 0
        self.tgXa = 0
        self.tgXaNua = 0

    def start(self):
        self.state = 1
        self.setTGTron = utils.db2int16(self.db29, 76)
        self.setTGXaNua = utils.db2int16(self.db29, 78)
        self.setTGXa = utils.db2int16(self.db29, 80)
        self.tgTron = 0
        self.tgXa = 0
        self.tgXaNua = 0
        utils.int162db(int(self.tgTron * 10), self.db26, 284)
        utils.int162db(int(self.tgXaNua), self.db26, 288)
        utils.int162db(int(self.tgXa), self.db26, 286)
        self.meht += 1
        utils.int162db(self.meht, self.db26, 290)
        self._discharge_valve(0)

    def run(self, delta):
        if self.state == 1:
            self.tgTron += delta
            utils.int162db(int(self.tgTron * 10), self.db26, 284)
            if self.tgTron > self.setTGTron:
                self.state = 4
                self._discharge_valve(1)
        elif self.state == 4:
            self.tgXaNua += delta
            self.tgXa += delta
            utils.int162db(int(self.tgXaNua), self.db26, 288)
            utils.int162db(int(self.tgXa), self.db26, 286)
            if self.tgXaNua > self.setTGXaNua:
                self.state = 2
                self._discharge_valve(2)
        elif self.state == 2:
            self.tgXa += delta
            utils.int162db(int(self.tgXa), self.db26, 286)
            if self.tgXa > self.setTGXa:
                self.state = 0
                utils.int162db(self.meht, self.db26, 290)
                self._discharge_valve(0)

    def reset(self):
        self.state = 0
        self.meht = 0
        self._discharge_valve(0)
        utils.int162db(self.meht, self.db26, 290)

    def is_mixing(self) -> bool:
        return self.state > 0

    def _discharge_valve(self, v):
        vc = 1 if v == 0 else 0
        vo = 1 if v == 2 else 0
        vho = 1 if v == 1 else 0
        utils.setbit(self.dbm, 102, 5, vc)
        utils.setbit(self.dbm, 102, 6, vo)
        utils.setbit(self.dbm, 102, 7, vho)