import utils

TG_BTN_RUN = 3

class CVanChuyenCL:
    def __init__(self, dbMem):
        self.dbMem = dbMem
        self.btn_run = False
        self.btn_t = 0
        self.skip_state = 0
        self.skip_dir = 0                   # 0: stop; 1: up; 2: down
        self.skip_t = 0
        self.napLieuXong = False

    def start(self):
        self.btn_run = True
        self.btn_t = 0
        utils.setbit(self.dbMem, 200, 1, 1)                # chạy băng tải ngang

        self.skip_dir = 1
        self.skip_t = 0
        utils.setbit(self.dbMem, 107, 6, 1)                # Xe skip lên

    def stop(self):
        self.btn_stop()
        self.skip_stop()

    def btn_stop(self):
        self.btn_run = False
        utils.setbit(self.dbMem, 200, 1, 0)                # dừng băng tải ngang

    def skip_stop(self):
        self.dir = 0

    def is_run(self) -> bool:
        return self.skip_dir != 0 or self.btn_run

    def run(self, delta):
        if self.is_run():
            self.btn_t += delta
            if self.btn_t > TG_BTN_RUN:
                self.btn_stop()

            self.skip_t += delta
            if self.skip_dir == 1:
                match self.skip_state:
                    case 0:
                        if self.skip_t > 1:
                            self.skip_state = 1
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 1, 0)                # rời DT0
                    case 1:
                        if self.skip_t > 1:
                            self.skip_state = 2
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 2, 1)                # đến DT1
                    case 2:
                        if self.skip_t > 1:
                            self.skip_state = 3
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 2, 0)                # rời DT1
                    case 3:
                        if self.skip_t > 1:
                            self.skip_state = 4
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 3, 1)                # đến DT2
                            utils.setbit(self.dbMem, 107, 6, 0)                # Xe skip lên
                    case 4:
                        if self.skip_t > 2:
                            self.skip_dir = 2
                            utils.setbit(self.dbMem, 107, 5, 1)                # Xe skip xuống
                            self.skip_t = 0                     
                            self.napLieuXong = True
            elif self.skip_dir == 2:
                match self.skip_state:
                    case 4:
                        if self.skip_t > 1:
                            self.skip_state = 3
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 3, 0)                # rời DT2
                    case 3:
                        if self.skip_t > 1:
                            self.skip_state = 2
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 2, 1)                # đến DT1
                    case 2:
                        if self.skip_t > 1:
                            self.skip_state = 1
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 2, 0)                # rời DT1
                    case 1:
                        if self.skip_t > 1:
                            self.skip_state = 0
                            self.skip_t = 0
                            utils.setbit(self.dbMem, 107, 1, 1)                # đến DT0
                            self.skip_dir = 0
                            utils.setbit(self.dbMem, 107, 5, 0)                # Xe skip dừng

    def get_nap_lieu_xong(self) -> bool:
        if self.napLieuXong:
            nx = self.napLieuXong
            self.napLieuXong = False
            return nx
        return False