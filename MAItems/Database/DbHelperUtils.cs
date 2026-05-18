using Microsoft.Data.Sqlite;
using System;

namespace MAItems.Database
{
    public static class DbHelperUtils
    {
        public static string StrD(SqliteDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? string.Empty : r.GetString(r.GetOrdinal(col));
        public static double? RealN(SqliteDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetDouble(r.GetOrdinal(col));

        public static bool HasColumn(SqliteDataReader r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++) if (r.GetName(i).Equals(col, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static void BindNullableReal(SqliteCommand cmd, string paramName, string rawValue)
        {
            double? converted = NumericConverter.Convert(rawValue);
            if (converted.HasValue) cmd.Parameters.AddWithValue(paramName, converted.Value); else cmd.Parameters.AddWithValue(paramName, DBNull.Value);
        }

        public static void BindReal(SqliteCommand cmd, string param, double? value)
        {
            if (value.HasValue) cmd.Parameters.AddWithValue(param, value.Value); else cmd.Parameters.AddWithValue(param, DBNull.Value);
        }

        public static string EscapeCsv(string value)
        {
            if (value == null) return string.Empty;
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}