SELECT a.id, a.ma,
       a.kh_id, a.da_id, a.ct_id, a.hm_id, a.diachi_id,
       a.thetichht, a.klht, a.meht, a.tght,
       a.trangthai, a.ghichu, a.created_at,
       COUNT(b.id) AS sophieu, SUM(b.medat) AS tmedat, SUM(b.thetichdat) AS tttdat, SUM(b.kldat) AS tkldat,
       SUM(b.meht) AS tmeht, SUM(b.thetichht) AS tttht, SUM(b.klht) AS tklht
       FROM ht_donhang AS a LEFT JOIN ht_phieu AS b ON a.id=b.donhang_id GROUP BY a.id;
