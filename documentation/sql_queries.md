## all_rows VIEW
```
CREATE OR REPLACE VIEW `peerless-garage-466612-a2.rawes.all_rows`
AS
  SELECT
  ANY_VALUE(s.survey_id) AS survey_id,
  ANY_VALUE(s.wetland_name) AS wetland_name,
  ANY_VALUE(s.gps_coordinates) AS gps_coordinates,
  ANY_VALUE(s.assessors) AS assessors,
  ANY_VALUE(s.date_completed) AS date_completed,
  ARRAY_AGG(STRUCT(sc.scale)) AS scale,
  b.benefit,
  bt.benefit_type,
  sr.description,
  sr.importance
FROM `peerless-garage-466612-a2.rawes.surveys` s
LEFT JOIN `peerless-garage-466612-a2.rawes.survey_rows` sr on s.survey_id = sr.survey_id
LEFT JOIN UNNEST(sr.scale_ids) AS scale_id
LEFT JOIN `peerless-garage-466612-a2.rawes.scales` sc on sc.scale_id = scale_id
LEFT JOIN `peerless-garage-466612-a2.rawes.benefits` b on sr.benefit_id = b.benefit_id
LEFT JOIN `peerless-garage-466612-a2.rawes.benefit_types` bt on b.benefit_type_id = bt.benefit_type_id
GROUP BY benefit, benefit_type, description, importance;
```

## all_surveys VIEW
```
CREATE OR REPLACE VIEW `peerless-garage-466612-a2.rawes.all_surveys`
AS
WITH all_rows AS (
 SELECT
  ANY_VALUE(s.survey_id) AS survey_id,
  ANY_VALUE(s.wetland_name) AS wetland_name,
  ANY_VALUE(s.gps_coordinates) AS gps_coordinates,
  ANY_VALUE(s.assessors) AS assessors,
  ANY_VALUE(s.date_completed) AS date_completed,
  ARRAY_AGG(STRUCT(sc.scale)) AS scale,
  b.benefit,
  bt.benefit_type,
  sr.description,
  sr.importance
FROM `peerless-garage-466612-a2.rawes.surveys` s
LEFT JOIN `peerless-garage-466612-a2.rawes.survey_rows` sr on s.survey_id = sr.survey_id
LEFT JOIN UNNEST(sr.scale_ids) AS scale_id
LEFT JOIN `peerless-garage-466612-a2.rawes.scales` sc on sc.scale_id = scale_id
LEFT JOIN `peerless-garage-466612-a2.rawes.benefits` b on sr.benefit_id = b.benefit_id
LEFT JOIN `peerless-garage-466612-a2.rawes.benefit_types` bt on b.benefit_type_id = bt.benefit_type_id
GROUP BY benefit, benefit_type, description, importance
)
SELECT 
survey_id,
wetland_name,
gps_coordinates,
assessors,
date_completed,
ARRAY_AGG(STRUCT(
  scale,
  benefit,
  benefit_type,
  description,
  importance
)) AS survey_row
FROM all_rows
GROUP BY survey_id,
wetland_name,
gps_coordinates,
assessors,
date_completed
```

## Stored procedure to insert a new survey
```
CREATE OR REPLACE PROCEDURE rawes.new_survey(
  new_benefit STRING,
  new_importance FLOAT64,
  scales ARRAY<STRING>,
  description STRING
)
BEGIN
  DECLARE new_row_id INT64;
  DECLARE new_survey_id INT64;
  DECLARE new_benefit_id INT64;
  DECLARE scale_ids ARRAY<INT64>;

  SET new_row_id = (
    SELECT IFNULL(MAX(row_id), 0) + 1 FROM `rawes.survey_rows`
  );
  SET new_survey_id = (
    SELECT IFNULL(MAX(survey_id), 0) + 1 FROM `rawes.surveys`
  );
  SET new_benefit_id = (
    SELECT benefit_id FROM `rawes.benefits`
    WHERE benefit = new_benefit
    LIMIT 1
  );
  SET scale_ids = (
    SELECT ARRAY_AGG(scale_id)
    FROM `rawes.scales`
    WHERE scale IN UNNEST(scales)
  );

  INSERT INTO `rawes.survey_rows` (row_id, survey_id, benefit_id, importance, scale_ids, description)
  VALUES (new_row_id, new_survey_id, new_benefit_id, new_importance, scale_ids, description);
END;
```
