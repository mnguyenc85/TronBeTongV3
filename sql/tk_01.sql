CREATE PROCEDURE tk_phieu (
    IN p_kh_info VARCHAR(63)
)
BEGIN
	CREATE TEMPORARY TABLE tmp_don_ids
   SELECT DISTINCT id
   	FROM ht_donhang
   	WHERE kh_id IN (SELECT id FROM kd_khachhang WHERE ten LIKE p_kh_info);
	
	SELECT a.*, 
		b.ma AS don_ma, b.kh_id AS don_kh_id,  b.da_id AS don_da_id, b.ct_id AS don_ct_id, b.hm_id AS don_hm_id, b.diachi_id AS don_diachi_id, b.ghichu AS don_ghichu
		FROM ht_phieu AS a LEFT JOIN ht_donhang AS b ON a.donhang_id=b.id 
		WHERE donhang_id IN (SELECT id FROM tmp_don_ids);
	
	DROP TEMPORARY TABLE IF EXISTS tmp_don_ids;
END