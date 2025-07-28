using System.Text;

namespace CnoomFrameWork.Base.Log.Formatters
{
    /// <summary>
    /// 默认的文本格式化器
    /// </summary>
    public class TextFormatter : ILogFormatter
    {
        /// <summary>
        /// 格式化日志条目为纯文本
        /// </summary>
        /// <param name="entry">日志条目</param>
        /// <returns>格式化后的字符串</returns>
        public string Format(LogEntry entry)
        {
            var builder = new StringBuilder();
            builder.Append($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}]");
            builder.Append($" [{entry.Level.ToString().ToUpper()}]");
            if (!string.IsNullOrEmpty(entry.Category))
            {
                builder.Append($" [{entry.Category}]");
            }
            builder.Append($" [TID:{entry.ThreadId}]");
            if (!string.IsNullOrEmpty(entry.CallerFile))
            {
                builder.Append($" [{entry.CallerFile}:{entry.CallerLine} {entry.CallerMember}]");
            }
            
            builder.Append($" - {entry.Message}");

            if (entry.Exception != null)
            {
                builder.AppendLine();
                builder.Append(entry.Exception);
            }

            if (entry.CustomFields != null && entry.CustomFields.Count > 0)
            {
                builder.Append(" | ExtraData: {");
                foreach (var field in entry.CustomFields)
                {
                    builder.Append($" \"{field.Key}\": \"{field.Value}\"");
                }
                builder.Append(" }");
            }

            return builder.ToString();
        }
    }
}