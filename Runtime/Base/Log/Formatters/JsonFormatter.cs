using System.Text;

namespace CnoomFrameWork.Base.Log.Formatters
{
    /// <summary>
    /// JSON格式化器
    /// </summary>
    public class JsonFormatter : ILogFormatter
    {
        /// <summary>
        /// 格式化日志条目为JSON字符串
        /// </summary>
        /// <param name="entry">日志条目</param>
        /// <returns>格式化后的JSON字符串</returns>
        public string Format(LogEntry entry)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"timestamp\":\"{entry.Timestamp:o}\",");
            sb.Append($"\"level\":\"{entry.Level.ToString().ToUpper()}\",");
            sb.Append($"\"category\":\"{Escape(entry.Category)}\",");
            sb.Append($"\"message\":\"{Escape(entry.Message)}\",");
            sb.Append($"\"threadId\":{entry.ThreadId},");
            sb.Append("\"location\":{");
            sb.Append($"\"file\":\"{Escape(entry.CallerFile)}\",");
            sb.Append($"\"line\":{entry.CallerLine},");
            sb.Append($"\"member\":\"{Escape(entry.CallerMember)}\"");
            sb.Append("}");

            if (entry.Exception != null)
            {
                sb.Append($",\"exception\":\"{Escape(entry.Exception.ToString())}\"");
            }

            if (entry.CustomFields != null && entry.CustomFields.Count > 0)
            {
                sb.Append(",\"customFields\":{");
                var first = true;
                foreach (var field in entry.CustomFields)
                {
                    if (!first) sb.Append(",");
                    // 简单处理，实际可能需要更复杂的序列化
                    var valueStr = field.Value?.ToString() ?? "null";
                    sb.Append($"\"{Escape(field.Key)}\":\"{Escape(valueStr)}\"");
                    first = false;
                }
                sb.Append("}");
            }

            sb.Append("}");
            return sb.ToString();
        }

        private string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
    }
}