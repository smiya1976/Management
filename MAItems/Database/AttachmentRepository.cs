using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace MAItems.Database
{
    public class AttachmentRepository
    {
        private readonly DatabaseContext _context;

        public AttachmentRepository(DatabaseContext context)
        {
            _context = context;
        }

        public List<Attachment> GetAttachments(long dealId)
        {
            var list = new List<Attachment>();
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("SELECT * FROM Attachments WHERE DealId = @DealId ORDER BY Id;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Attachment
                {
                    Id = reader.GetInt64(0),
                    DealId = reader.GetInt64(1),
                    FileName = reader.GetString(2),
                    FilePath = reader.GetString(3),
                    Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    UploadedAt = reader.GetString(5)
                });
            }
            return list;
        }

        public void SaveAttachment(Attachment a)
        {
            using var conn = _context.GetConnection();
            string sql = @"INSERT OR REPLACE INTO Attachments (Id, DealId, FileName, FilePath, Description, UploadedAt) VALUES (@Id, @DealId, @FileName, @FilePath, @Description, @UploadedAt);";
            using var cmd = new SqliteCommand(sql, conn);
            if (a.Id > 0) cmd.Parameters.AddWithValue("@Id", a.Id); else cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            cmd.Parameters.AddWithValue("@DealId", a.DealId);
            cmd.Parameters.AddWithValue("@FileName", a.FileName);
            cmd.Parameters.AddWithValue("@FilePath", a.FilePath);
            cmd.Parameters.AddWithValue("@Description", a.Description ?? "");
            cmd.Parameters.AddWithValue("@UploadedAt", string.IsNullOrEmpty(a.UploadedAt) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : a.UploadedAt);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAttachment(long id)
        {
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("DELETE FROM Attachments WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}