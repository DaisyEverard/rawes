using Google.Cloud.BigQuery.V2;

namespace MainApi.Data.Queries;

static class SurveyQueries
{
    public static string GetMetadataSql(BigQueryTable allSurveysTable, int surveyID)
    {
        return $"""
            SELECT
                wetland_name,
                gps_coordinates.x as gps_x,
                gps_coordinates.y as gps_y,
                date_completed
            FROM {allSurveysTable}
            WHERE survey_id = {surveyID}
            LIMIT 1
        """;
    }

    public static string GetAssessorsSql(BigQueryTable allSurveysTable, int surveyID)
    {
        return $"""
            SELECT 
                a
            FROM {allSurveysTable} s, UNNEST(s.assessors) AS a
            WHERE survey_id = {surveyID}
        """;
    }

    public static string GetRowsSql(BigQueryTable allRowsTable, int surveyID)
    {
        return $"""
            SELECT
                benefit,
                benefit_type,
                description,
                importance
            FROM {allRowsTable}
            WHERE survey_id = {surveyID}
        """;
    }

    public static string GetScalesSql(BigQueryTable allRowsTable, int surveyID)
    {
        return $"""
            SELECT
                benefit,
                s
            FROM {allRowsTable} r, UNNEST(r.scale) AS s
            WHERE survey_id = {surveyID}
        """;
    }
}
