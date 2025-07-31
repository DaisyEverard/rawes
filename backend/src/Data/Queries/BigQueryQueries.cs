using Google.Cloud.BigQuery.V2;

namespace MainApi.Data.Queries;

static class BigQueryQueries
{
    public static string GetMetadataSql(BigQueryTable allSurveysTable)
    {
        return $"""
            SELECT
                wetland_name,
                gps_coordinates.x as gps_x,
                gps_coordinates.y as gps_y,
                date_completed
            FROM {allSurveysTable}
            WHERE survey_id = @survey_id
            LIMIT 1
        """;
    }

    public static string GetAssessorsSql(BigQueryTable allSurveysTable)
    {
        return $"""
            SELECT 
                a
            FROM {allSurveysTable} s, UNNEST(s.assessors) AS a
            WHERE survey_id = @survey_id
        """;
    }

    public static string GetRowsSql(BigQueryTable allRowsTable)
    {
        return $"""
            SELECT
                benefit,
                benefit_type,
                description,
                importance
            FROM {allRowsTable}
            WHERE survey_id = @survey_id
        """;
    }

    public static string GetScalesSql(BigQueryTable allRowsTable)
    {
        return $"""
            SELECT
                benefit,
                s
            FROM {allRowsTable} r, UNNEST(r.scale) AS s
            WHERE @survey_id = @survey_id
        """;
    }
}
