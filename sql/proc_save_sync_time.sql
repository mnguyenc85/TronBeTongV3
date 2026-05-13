BEGIN
	DECLARE existing_id INT;

   -- Tìm id nếu đã tồn tại văn bản
   SELECT id INTO existing_id
   	FROM sync_log
	   WHERE tblindex = p_index
	   LIMIT 1;

   IF existing_id IS NOT NULL THEN
      UPDATE sync_log
      	SET sync_at = CURRENT_TIMESTAMP()
      	WHERE id = existing_id;
   ELSE
      INSERT INTO sync_log (tblindex, sync_at)
      VALUES (p_index, CURRENT_TIMESTAMP());
   END IF;
END