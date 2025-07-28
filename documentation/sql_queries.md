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