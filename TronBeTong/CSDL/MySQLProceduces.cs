using MySqlConnector;

namespace TronBeTongV3.CSDL
{
    public class MySQLProceduces
    {
        #region Phần mềm
        public static readonly string QueryUpsertString = @"
-- Xóa procedure nếu đã tồn tại
DROP PROCEDURE IF EXISTS pm_upsert_string;

CREATE PROCEDURE pm_upsert_string (
    IN p_vanban VARCHAR(255),
    IN p_phanloai INT,
    OUT p_id INT
)
BEGIN
    DECLARE existing_id INT;

    IF p_vanban IS NOT NULL THEN
        -- Tìm id nếu đã tồn tại văn bản
        SELECT id INTO existing_id
        FROM pm_strings
        WHERE vanban = p_vanban AND phanloai = p_phanloai
        LIMIT 1;

        IF existing_id IS NOT NULL THEN
            -- Nếu tồn tại: tăng sudung lên 1
            UPDATE pm_strings
            SET sudung = sudung + 1
            WHERE id = existing_id;

            -- Trả về id
            SET p_id = existing_id;
        ELSE
            -- Nếu chưa có: thêm mới với sudung = 1
            INSERT INTO pm_strings (vanban, phanloai, sudung)
            VALUES (p_vanban, p_phanloai, 1);

            -- Lấy id vừa chèn
            SET p_id = LAST_INSERT_ID();
        END IF;
    ELSE
        SET p_id = -1;
    END IF;
END;";
        #endregion

        #region Kinh doanh
        public static readonly string QueryUpsertDuAn = @"
DROP PROCEDURE IF EXISTS kd_upsert_duan;

CREATE PROCEDURE kd_upsert_duan (
    IN p_inid INT,
    IN p_duan VARCHAR(255),
    IN p_congtrinh VARCHAR(255),
    IN p_hangmuc VARCHAR(255),
    IN p_diachi VARCHAR(255),
    IN p_ghichu VARCHAR(255),
    IN p_khid INT,
    OUT p_id INT
)
BEGIN
    DECLARE duan_id INT;
    DECLARE congtrinh_id INT;
    DECLARE hangmuc_id INT;
    DECLARE diachi_id INT;

    -- Gọi procedure pm_upsert_string để lấy các id tương ứng
    CALL pm_upsert_string(p_duan, 1, duan_id);
    CALL pm_upsert_string(p_congtrinh, 2, congtrinh_id);
    CALL pm_upsert_string(p_hangmuc, 3, hangmuc_id);
    CALL pm_upsert_string(p_diachi, 4, diachi_id);

    -- Nếu p_inid > 0 thì update
    IF p_inid > 0 THEN
        UPDATE kd_duan
        SET duan = duan_id,
            congtrinh = congtrinh_id,
            hangmuc = hangmuc_id,
            diachi = diachi_id,
            ghichu = p_ghichu,
            kh_id = p_khid
        WHERE id = p_inid;

        SET p_id = p_inid;
    ELSE
        -- Nếu p_inid <= 0 thì insert mới
        INSERT INTO kd_duan (duan,congtrinh,hangmuc,diachi,ghichu,kh_id)
        VALUES (duan_id,congtrinh_id,hangmuc_id,diachi_id,p_ghichu,p_khid);

        SET p_id = LAST_INSERT_ID();
    END IF;
END;";
        #endregion

        #region Công thức
        public static readonly string QueryCTSaveCongThucUniqueMa = @"
-- Xóa procedure nếu đã tồn tại
DROP PROCEDURE IF EXISTS ct_save_nguyenlieu_unique_ma;

-- Tạo procedure mới
CREATE PROCEDURE ct_save_nguyenlieu_unique_ma (
    IN p_inid INT,
    IN p_ma VARCHAR(63),
    IN p_ten VARCHAR(63),
    IN p_pl INT,
    IN p_doam DOUBLE,
    OUT p_id INT
)
BEGIN
    DECLARE dup_ma_id INT;
    
    -- Tìm bản ghi có các giá trị giống hệt
    SELECT id INTO dup_ma_id FROM ct_nguyenlieu WHERE ma = p_ma LIMIT 1;

    IF dup_ma_id IS NULL THEN
        -- Nếu không bị trung mã
        IF p_inid > 0 THEN
            UPDATE ct_nguyenlieu SET
                ma = p_ma,
                ten = p_ten,
                phanloai = p_pl,
                doam = p_doam
            WHERE id=p_inid;
            
            SET p_id = p_inid;
        ELSE
            INSERT INTO ct_nguyenlieu (ma,ten,phanloai,doam)
            VALUES (p_ma, p_ten, p_pl, p_doam);
            -- Lấy ID của bản ghi vừa thêm
            SET p_id = LAST_INSERT_ID();
        END IF;
    ELSE
        IF dup_ma_id = p_inid THEN
            UPDATE ct_nguyenlieu SET
                ma = p_ma,
                ten = p_ten,
                phanloai = p_pl,
                doam = p_doam
            WHERE id=p_inid;
            
            SET p_id = p_inid;
        ELSE
            SET p_id = -1;
        END IF;
    END IF;

    -- Trả về ID
    SELECT p_id AS id;
END";

        public static readonly string QueryCTDelete = @"
-- Xóa procedure nếu đã tồn tại
DROP PROCEDURE IF EXISTS ct_delete_by_id;

-- Tạo procedure mới
CREATE PROCEDURE ct_delete_by_id (
    IN p_id INT
)
BEGIN
    DELETE FROM ct_thanhphan WHERE ct_id=p_id;

    DELETE FROM ct_congthuc WHERE id=p_id;
END";
        #endregion

        #region Đơn hàng
        public static readonly string QueryUpsertHTDonHang = @"
DROP PROCEDURE IF EXISTS ht_upsert_donhang;

CREATE PROCEDURE ht_upsert_donhang (
    IN p_inid INT,
    IN p_ma VARCHAR(63),
    IN p_duan VARCHAR(255),
    IN p_congtrinh VARCHAR(255),
    IN p_hangmuc VARCHAR(255),
    IN p_thetich DOUBLE,
    IN p_diachi VARCHAR(255),
    IN p_ghichu VARCHAR(255),
    IN p_khid INT,
    OUT p_id INT
)
BEGIN
    DECLARE sda_id INT;
    DECLARE sct_id INT;
    DECLARE shm_id INT;
    DECLARE sdc_id INT;

    -- Gọi procedure kd_upsert_ht_string để lấy các id tương ứng
    CALL pm_upsert_string(p_duan, 1, sda_id);
    CALL pm_upsert_string(p_congtrinh, 2, sct_id);
    CALL pm_upsert_string(p_hangmuc, 3, shm_id);
    CALL pm_upsert_string(p_diachi, 4, sdc_id);

    -- Nếu p_inid > 0 thì update
    IF p_inid > 0 THEN
        UPDATE ht_donhang
        SET ma = p_ma,
            da_id = sda_id,
            ct_id = sct_id,
            hm_id = shm_id,
            diachi_id = sdc_id,
            thetichdh = p_thetich,
            ghichu = p_ghichu,
            kh_id = p_khid
        WHERE id = p_inid;

        SET p_id = p_inid;
    ELSE
        -- Nếu p_inid <= 0 thì insert mới
        INSERT INTO ht_donhang (ma,da_id,ct_id,hm_id,diachi_id,thetichdh,ghichu,kh_id,thetichht,klht,meht,trangthai)
        VALUES (p_ma,sda_id,sct_id,shm_id,sdc_id,p_thetich,p_ghichu,p_khid,0,0,0,0);

        SET p_id = LAST_INSERT_ID();
    END IF;
END;";

        public static readonly string QueryUpsertCongThucThanhPhanRel = @"
-- Xóa procedure nếu đã tồn tại
DROP PROCEDURE IF EXISTS ht_upsert_congthuc_thanhphan_rel;

-- Tạo procedure mới
CREATE PROCEDURE ht_upsert_congthuc_thanhphan_rel (
    IN p_ctid INT,
    IN p_ma VARCHAR(63),
    IN p_ten VARCHAR(127),
    IN p_pl INT,
    IN p_silo INT,
    IN p_klcongthuc DOUBLE,
    IN p_kltong DOUBLE,
    IN p_klme DOUBLE,
    OUT p_id INT
)
BEGIN
    -- Tìm bản ghi có các giá trị giống hệt
    SELECT id INTO p_id
    FROM ht_thanhphan
    WHERE ma = p_ma
        AND ten = p_ten
        AND phanloai = p_pl
        AND silo = p_silo
        AND klcongthuc = p_klcongthuc
        AND kltong = p_kltong
        AND klme = p_klme
    LIMIT 1;

    -- Nếu không tìm thấy bản ghi, thêm mới
    IF p_id IS NULL THEN
        INSERT INTO ht_thanhphan (ma, ten, phanloai, silo, klcongthuc, kltong, klme)
        VALUES (p_ma, p_ten, p_pl, p_silo, p_klcongthuc, p_kltong, p_klme);
        
        -- Lấy ID của bản ghi vừa thêm
        SET p_id = LAST_INSERT_ID();
    END IF;

    INSERT IGNORE INTO ht_congthuc_thanhphan (ct_id, tp_id)
    VALUES (p_ctid,p_id);

    -- Trả về ID
    SELECT p_id AS id;
END;";

        public static readonly string QueryUpsertCongThucThanhPhan = @"
-- Xóa procedure nếu đã tồn tại
DROP PROCEDURE IF EXISTS ht_upsert_congthuc_thanhphan;

-- Tạo procedure mới
CREATE PROCEDURE ht_upsert_congthuc_thanhphan (
    IN p_ma VARCHAR(63),
    IN p_ten VARCHAR(127),
    IN p_pl INT,
    IN p_silo INT,
    IN p_klcongthuc DOUBLE,
    IN p_kltong DOUBLE,
    IN p_klme DOUBLE,
    OUT p_id INT
)
BEGIN
    -- Tìm bản ghi có các giá trị giống hệt
    SELECT id INTO p_id
    FROM ht_thanhphan
    WHERE ma = p_ma
        AND ten = p_ten
        AND phanloai = p_pl
        AND silo = p_silo
        AND klcongthuc = p_klcongthuc
        AND kltong = p_kltong
        AND klme = p_klme
    LIMIT 1;

    -- Nếu không tìm thấy bản ghi, thêm mới
    IF p_id IS NULL THEN
        INSERT INTO ht_thanhphan (ma, ten, phanloai, silo, klcongthuc, kltong, klme)
        VALUES (p_ma, p_ten, p_pl, p_silo, p_klcongthuc, p_kltong, p_klme);
        
        -- Lấy ID của bản ghi vừa thêm
        SET p_id = LAST_INSERT_ID();
    END IF;

    -- Trả về ID
    SELECT p_id AS id;
END;";

        public static readonly string QueryUpsertCongThuc = @"
-- Xóa procedure nếu đã tồn tại
DROP PROCEDURE IF EXISTS ht_upsert_congthuc;

-- Tạo procedure mới
CREATE PROCEDURE ht_upsert_congthuc (
    IN p_ma VARCHAR(63),
    IN p_mac VARCHAR(63),
    IN p_slump VARCHAR(31),
    IN p_wcratio DOUBLE,
    IN p_klnuoc DOUBLE,
    IN p_sotp INT,
    IN p_dstp VARCHAR(255),
    OUT p_id INT
)
BEGIN
    DECLARE is_match BOOL DEFAULT FALSE;

   -- Tìm bản ghi có các giá trị giống hệt
	SELECT c.id
		INTO p_id
		FROM ht_congthuc c
		WHERE c.ma = p_ma
			AND c.mac = p_mac
			AND c.slump = p_slump
			AND c.sotp = p_sotp
			-- điều kiện kiểm tra thành phần
			AND NOT EXISTS (
      		-- tìm xem có tp nào trong công thức nhưng không có trong p_dstp
	      	SELECT 1
		      FROM ht_congthuc_thanhphan t
      		WHERE t.ct_id = c.id
        			AND NOT FIND_IN_SET(t.tp_id, REPLACE(p_dstp, ' ', ''))
  			)
			AND (SELECT COUNT(*) 
       		FROM ht_congthuc_thanhphan t2
				WHERE t2.ct_id = c.id) = p_sotp
		LIMIT 1;

    -- Nếu không tìm thấy bản ghi, thêm mới
    IF p_id IS NULL THEN
        INSERT INTO ht_congthuc (ma,mac,slump,wcratio,klnuoc,sotp)
        VALUES (p_ma, p_mac, p_slump, p_wcratio, p_klnuoc, p_sotp);
        
        -- Lấy ID của bản ghi vừa thêm
        SET p_id = LAST_INSERT_ID();
    END IF;

    -- Trả về ID
    SELECT p_id AS id;
END;";

        private static string QueryUpsertCongThucThanhPhanRelOnly = @"
DROP PROCEDURE IF EXISTS ht_upsert_ct_tp_rel;

CREATE PROCEDURE ht_upsert_ct_tp_rel (
    IN p_ct_id INT,
    IN p_tp_id INT
)
BEGIN
    DECLARE existing_id INT;

    SELECT id INTO existing_id
   	    FROM ht_congthuc_thanhphan
	    WHERE ct_id=p_ct_id AND tp_id=p_tp_id
	    LIMIT 1;

    IF existing_id IS NULL THEN
        INSERT INTO ht_congthuc_thanhphan (ct_id,tp_id) VALUES (p_ct_id,p_tp_id);
    END IF;
END";
        #endregion

        #region Sync
        private static string QuerySyncSaveTime = @"
DROP PROCEDURE IF EXISTS sync_save_time;

CREATE PROCEDURE sync_save_time (
    IN p_index INT
)
BEGIN
    DECLARE existing_id INT;

    SELECT id INTO existing_id
   	    FROM sync_log
	    WHERE tblindex = p_index
	    LIMIT 1;

    IF existing_id IS NOT NULL THEN
        UPDATE sync_log SET sync_at = CURRENT_TIMESTAMP() WHERE id = existing_id;
    ELSE
        INSERT INTO sync_log (tblindex, sync_at) VALUES (p_index, CURRENT_TIMESTAMP());
    END IF;
END";
        #endregion

        public static async Task ExecuteAddProcs(MySqlCommand cmd)
        {
            cmd.CommandText = QueryUpsertString;
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = QueryUpsertDuAn;
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = QueryCTSaveCongThucUniqueMa;
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = QueryCTDelete;
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = QueryUpsertHTDonHang;
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = QueryUpsertCongThuc;
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = QueryUpsertCongThucThanhPhan;
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = QueryUpsertCongThucThanhPhanRelOnly;
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = QuerySyncSaveTime;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
