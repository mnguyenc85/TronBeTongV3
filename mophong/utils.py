import struct, random
from datetime import datetime

rnd = random.Random(datetime.now().second)

def float2db(v, db, i):
    buf = struct.pack(">f", v)
    db[i]       = buf[0]
    db[i + 1]   = buf[1]
    db[i + 2]   = buf[2]
    db[i + 3]   = buf[3]

def int162db(v, db, i):
    buf = struct.pack(">h", v)
    db[i]       = buf[0]
    db[i + 1]   = buf[1]

def int82db(v, db, i):
    buf = struct.pack("<h", v)
    db[i] = buf[0]

def db2float(db, i):
    return struct.unpack(">f", bytes(db[i:i+4]))[0]

def db2int16(db, i):
    return struct.unpack(">h", bytes(db[i:i+2]))[0]

def setbit(db, byte, bit, value):
    mask = 1 << bit
    if value:
        db[byte] |= mask
    else:
        db[byte] &= ~mask