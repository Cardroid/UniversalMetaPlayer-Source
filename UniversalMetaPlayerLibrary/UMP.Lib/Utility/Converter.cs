using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UMP.Lib.Utility
{
    public static class Converter
    {
        /// <summary>
        /// 시간을 문자열로 변환합니다
        /// </summary>
        /// <param name="value">시간으로 바꿀 TimeSpan</param>
        /// <returns>문자열</returns>
        public static string TimeSpanStringConverter(TimeSpan value)
        {
            if (value == TimeSpan.Zero)
                return "00:00";
            if (value.TotalDays > 1)
                return value.ToString(@"d\:hh\:mm\:ss");
            else if (value.TotalHours > 1)
                return value.ToString(@"h\:mm\:ss");
            else if (value.TotalSeconds > 1)
                return value.ToString(@"mm\:ss");
            else
                //return value.ToString(@"s\:FFF");
                return "00:00";
        }

        /// <summary>
        /// ZIP 파일 추출합니다
        /// </summary>
        /// <param name="zipFilePath">ZIP 파일 경로</param>
        /// <param name="targetFilePath">백업 폴더</param>
        public static async Task<bool> ExtractZIPFileAsync(string zipFilePath, string targetFilePath)
        {
            bool result = false;
            await Task.Run(() =>
            {
                if (File.Exists(zipFilePath))
                {
                    try
                    {
                        using (ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath))
                        {
                      // 압축 풀 폴더에 이미 존재하면 삭제
                      if (Directory.Exists(Path.Combine(targetFilePath, zipArchive.Entries[0].FullName)))
                                Directory.Delete(Path.Combine(targetFilePath, zipArchive.Entries[0].FullName), true);

                            foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                            {
                                string folderPath = Path.GetDirectoryName(Path.Combine(targetFilePath, zipArchiveEntry.FullName));

                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);

                          // 경로명은 제외
                          if (!string.IsNullOrWhiteSpace(zipArchiveEntry.FullName) && !zipArchiveEntry.FullName.EndsWith("\\") && !zipArchiveEntry.FullName.EndsWith("/"))
                                    zipArchiveEntry.ExtractToFile(Path.Combine(targetFilePath, zipArchiveEntry.FullName));
                            }
                        }
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
                else
                    result = false;
            });
            return result;
        }

        /// <summary>
        /// SHA256 알고리즘으로 HASH값을 구합니다
        /// </summary>
        /// <param name="data">원본 데이터</param>
        /// <returns>HASH 값</returns>
        public static string SHA256Hash(string data)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in hash)
                stringBuilder.AppendFormat("{0:x2}", b);

            return stringBuilder.ToString();
        }
    }
}
